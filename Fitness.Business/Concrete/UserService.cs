using AutoMapper;
using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.User;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using FitnessManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ITrainerDal _trainerDal;
        private readonly IEquipmentDal _equipmentDal;
        private readonly IPackageDal _packageDal;
        private readonly IGroupUserDal _groupUserDal;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPurchaseHistoryService _purchaseHistoryService;




        public UserService(IUserDal userDal, IMapper mapper, IFileService fileService, UserManager<ApplicationUser> userManager, ITrainerDal trainerDal, RoleManager<IdentityRole> roleManager, IEquipmentDal equipmentDal, IPackageDal packageDal, IGroupUserDal groupUserDal, IPurchaseHistoryService purchaseHistoryService)
        {
            _userDal = userDal;
            _mapper = mapper;
            _fileService = fileService;
            _userManager = userManager;
            _trainerDal = trainerDal;
            _roleManager = roleManager;
            _equipmentDal = equipmentDal;
            _packageDal = packageDal;
            _groupUserDal= groupUserDal;
            _purchaseHistoryService = purchaseHistoryService;

        }
        public async Task<User> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _userDal.Get(u => u.IdentityUserId == identityUserId);
        }

        public async Task<decimal> GetBalanceAsync(string identityUserId)
        {
            var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
            if (user == null)
                throw new Exception("User not found.");

            return user.Balance??0;
        }
        public async Task<UserPackageInfoDto> GetUserPackageInfoAsync(string identityUserId)
        {
            var user = await _userDal.Get(
                u => u.IdentityUserId == identityUserId,
                include: u => u
                    .Include(u => u.Package)
                    .Include(u => u.Trainer)
                    .Include(u => u.GroupUsers)
                        .ThenInclude(gu => gu.Group)
            );

            if (user == null)
            {
                return null;
            }

            var dto = new UserPackageInfoDto
            {
                PackageName = user.Package?.PackageName,
                PackagePrice = user.Package?.Price,
                TrainerName = user.Trainer?.Name,
                GroupName = user.GroupUsers.FirstOrDefault()?.Group?.Name
            };

            return dto;
        }


        public async Task<List<TopUserDto>> GetTopUsersByGroupAsync(string identityUserId)
        {
            var groupUsers = await _groupUserDal.GetList(filter: null, q => q.Include(gu => gu.User)); 
            var currentGroupUser = groupUsers.FirstOrDefault(gu => gu.User.IdentityUserId == identityUserId);

            if (currentGroupUser == null)
                return new List<TopUserDto>(); 

            var groupId = currentGroupUser.GroupId;

            var groupMembers = groupUsers
                .Where(gu => gu.GroupId == groupId)
                .Select(gu => gu.User)
                .OrderByDescending(u => u.Point)
                .Select(u => new TopUserDto
                {
                    UserId = u.Id,
                    Name = u.Name,
                    Point = u.Point,
                    ImageUrl = u.ImageUrl != null ? _fileService.GetFileUrl(u.ImageUrl) : null
                })
                .ToList();

            return groupMembers;
        }

        public async Task UpdateUserProfile(int userId, UserProfileUpdateDto userProfileUpdateDto)
        {
            var user = await _userDal.Get(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            if (identityUser == null)
            {
                throw new Exception("Identity user not found");
            }
            if (userProfileUpdateDto.Phone != null)
            {
                user.Phone = userProfileUpdateDto.Phone;

            }

            if (!string.IsNullOrEmpty(userProfileUpdateDto.Name))
            {
                user.Name = userProfileUpdateDto.Name;
                identityUser.FullName = userProfileUpdateDto.Name;
            }
            if (userProfileUpdateDto.DateOfBirth.HasValue)
            {
                user.DateOfBirth = userProfileUpdateDto.DateOfBirth.Value;
            }


            if (!string.IsNullOrEmpty(userProfileUpdateDto.Email))
            {
                user.Email = userProfileUpdateDto.Email;
                identityUser.Email = userProfileUpdateDto.Email;
                identityUser.UserName = userProfileUpdateDto.Email;
            }

   

            if (userProfileUpdateDto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(userProfileUpdateDto.ImageUrl);
                user.ImageUrl = imageUrl;
            }
        
            
            if (!string.IsNullOrEmpty(userProfileUpdateDto.NewPassword))
            {
                if (!string.IsNullOrEmpty(userProfileUpdateDto.CurrentPassword))
                {

                    var checkPassword = await _userManager.CheckPasswordAsync(identityUser, userProfileUpdateDto.CurrentPassword);

                    if (!checkPassword)
                    {
                        throw new Exception("Current password is incorrect!");
                    }
                    var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                    var result = await _userManager.ResetPasswordAsync(identityUser, token, userProfileUpdateDto.NewPassword);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to change password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }

                }

                else
                {
                    throw new Exception("Current password is required to change the password!");
                }


            }
            user.UpdatedDate = DateTime.Now;
            await _userManager.UpdateAsync(identityUser);
            await _userDal.Update(user);
        }

        //STATISTICS USER
        public async Task<StatisticsDto> GetStatisticsForUser()
        {
          
            var trainerCount = await _trainerDal.GetList();
            var equipmentCount = await _equipmentDal.GetList();
            var packageCount = await _packageDal.GetList();

            return new StatisticsDto
            {              
                NumberOfTrainers = trainerCount.Count,
                NumberOfEquipments = equipmentCount.Count,
                NumberOfPackages = packageCount.Count
            };
        }
        public async Task<UserProfileDto> GetUserProfile(string identityUserId)
		{
			
			var user = await _userDal.Get(u => u.IdentityUserId == identityUserId, include: q => q
			 .Include(u => u.Package)
			 .Include(u => u.Trainer));

			if (user == null)
				throw new Exception("User not found");

			var identityUser = await _userManager.FindByIdAsync(identityUserId);
			if (identityUser == null)
				throw new Exception("Identity user not found");

			return new UserProfileDto
			{
				Id = user.Id,
				Name = user.Name,
				Email = identityUser.Email,
				Phone = identityUser.PhoneNumber,
				ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null,
				CreatedDate = user.CreatedDate,
				DateOfBirth = user.DateOfBirth,
				TrainerId = user.TrainerId,
				PackageId = user.PackageId,
                Point=user.Point,
				TrainerName = user.Trainer != null ? user.Trainer.Name : null,
				PackageName = user.Package != null ? user.Package.PackageName: null
			};
		}

		public async Task ApproveUser(string userId)
        {
           
            var user = await _userManager.Users
                .Where(u => u.Id == userId && !u.IsApproved) 
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("İstifadəçi tapılmadı və ya artıq təsdiqlənib.");
            }

            
            user.IsApproved = true;

         
            var identityUser = await _userManager.FindByIdAsync(userId);
            if (identityUser == null)
            {
                throw new Exception("IdentityUser tapılmadı.");
            }

            var passwordHash = identityUser.PasswordHash;
            var passwordSalt = identityUser.SecurityStamp;

            var saltBytes = Encoding.UTF8.GetBytes(passwordSalt);
            var hashBytes = Encoding.UTF8.GetBytes(passwordHash);

            var existingUser = await _userDal.Get(u => u.IdentityUserId == user.Id);
            if (existingUser == null)
            {
                var newUser = new User
                {
                    IdentityUserId = identityUser.Id,
                    Name = identityUser.FullName,
                    Email = identityUser.Email,
                    IsActive = true,
                    IsApproved = true,
                    PasswordHash = hashBytes,
                    PasswordSalt = saltBytes,
                };
                await _userDal.Add(newUser);
            }

            var result = await _userManager.UpdateAsync(identityUser);
            if (!result.Succeeded)
            {
                throw new Exception("İstifadəçi məlumatları yenilənərkən səhv baş verdi.");
            }
        }

        public async Task<List<ApplicationUser>> GetPendingUsers(string search = null)
        {
            var allUsers = await _userManager.Users
                .Where(u => !u.IsApproved && (search == null ||
                                              u.FullName.Contains(search) ||
                                              u.Email.Contains(search)))
                .ToListAsync();

            var pendingUsers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("User"))
                {
                    pendingUsers.Add(user);
                }
            }

            return pendingUsers;
        }


        public async Task DeclineUser(string userId)
        {
           
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsApproved);

            if (user != null)
            {

                await _userManager.DeleteAsync(user);
            }
            else
            {
                throw new Exception("User not found in pending users!");
            }
        }


        public async Task AddUser(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            var identityUser = new ApplicationUser
            {
                FullName=userDto.Name,
                UserName = userDto.Email,
                Email = userDto.Email,
                IsApproved= userDto.IsApproved,
                
            };

            var result = await _userManager.CreateAsync(identityUser, userDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception("İstifadəçi əlavə edilə bilmədi: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            user.IdentityUserId = identityUser.Id;
            var passwordHash = identityUser.PasswordHash;
            var passwordSalt = identityUser.SecurityStamp;
            var saltBytes = Encoding.UTF8.GetBytes(passwordSalt);
            var hashBytes = Encoding.UTF8.GetBytes(passwordHash);
            user.PasswordHash = hashBytes;
            user.PasswordSalt= saltBytes;
            user.Phone = userDto.Phone;
            user.DateOfBirth=userDto.DateOfBirth;

            if (userDto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(userDto.ImageUrl);
                user.ImageUrl = imageUrl;
            }

            if (userDto.IsApproved != null)
            {
                user.IsApproved = userDto.IsApproved;  
            }
            else
            {
                user.IsApproved = false; 
            }


            await _userDal.Add(user);

            //await _userManager.AddToRoleAsync(identityUser,"User");

            if (!await _roleManager.RoleExistsAsync("User"))
            {

                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            await _userManager.AddToRoleAsync(identityUser, "User");



        }
        public async Task DeleteUser(int userId)
        {
            var user = await _userDal.Get(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            if (identityUser != null)
            {
                
                var result = await _userManager.DeleteAsync(identityUser);
                if (!result.Succeeded)
                {
                    throw new Exception("İstifadəçi silinərkən xəta baş verdi: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            await _userDal.Delete(user);
        }
        //trainer ve paket id yenile
        public async Task UpdateUser(int userId, UserUpdateDto userUpdateDto)
        {
            var user = await _userDal.Get(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            if (identityUser == null)
            {
                throw new Exception("Identity user not found");
            }
            if (userUpdateDto.Phone != null)
            {
                user.Phone = userUpdateDto.Phone;
               
            }

            if (!string.IsNullOrEmpty(userUpdateDto.Name))
            {
                user.Name = userUpdateDto.Name;
                identityUser.FullName = userUpdateDto.Name;
            }
            if (userUpdateDto.DateOfBirth.HasValue)
            {
                user.DateOfBirth = userUpdateDto.DateOfBirth.Value;
            }


            if (!string.IsNullOrEmpty(userUpdateDto.Email))
            {
                user.Email = userUpdateDto.Email;
                identityUser.Email = userUpdateDto.Email;
                identityUser.UserName = userUpdateDto.Email;
            }

            if (userUpdateDto.IsApproved)
            {
                user.IsApproved = userUpdateDto.IsApproved;
                identityUser.IsApproved = userUpdateDto.IsApproved;
            }
            if (userUpdateDto.IsBlocked)
            {
                user.IsBlocked = userUpdateDto.IsBlocked;
                identityUser.IsBlocked = userUpdateDto.IsBlocked;
            }
            if (userUpdateDto.PackageId.HasValue)
            {
                user.PackageId = userUpdateDto.PackageId.Value;
            }

            if (userUpdateDto.TrainerId.HasValue)
            {
                user.TrainerId = userUpdateDto.TrainerId.Value;
            }




            if (userUpdateDto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(userUpdateDto.ImageUrl);
                user.ImageUrl = imageUrl;
            }
            if (userUpdateDto.IsActive)
            {
                user.IsActive = userUpdateDto.IsActive;
               
            }
            if (!string.IsNullOrEmpty(userUpdateDto.NewPassword))
            {
                if (!string.IsNullOrEmpty(userUpdateDto.CurrentPassword))
                {

                    var checkPassword = await _userManager.CheckPasswordAsync(identityUser, userUpdateDto.CurrentPassword);

                    if (!checkPassword)
                    {
                        throw new Exception("Current password is incorrect!");
                    }
                    var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                    var result = await _userManager.ResetPasswordAsync(identityUser, token, userUpdateDto.NewPassword);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to change password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }

                }
             
                else
                {
                    throw new Exception("Current password is required to change the password!");
                }


            }
            user.UpdatedDate = DateTime.Now;
            await _userManager.UpdateAsync(identityUser);
            await _userDal.Update(user);
        }
        public async Task<List<UserGetDto>> GetAllUsers(string searchTerm = null)
        {
            var users = await _userDal.GetList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string normalizedSearch = searchTerm.Trim().ToLower();
                users = users
                    .Where(u =>
                        (!string.IsNullOrEmpty(u.Name) && u.Name.ToLower().Contains(normalizedSearch)) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(normalizedSearch)) ||
                        (!string.IsNullOrEmpty(u.Phone) && u.Phone.ToLower().Contains(normalizedSearch)))
                    .ToList();
            }

            var userDtos = users.Select(user => new UserGetDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                CreatedDate = user.CreatedDate,
                DateOfBirth = user.DateOfBirth,
                PackageId = user.PackageId,
                TrainerId = user.TrainerId,
                ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null
            }).ToList();

            return userDtos;
        }



        //public async Task<List<UserGetDto>> GetAllUsers()
        //{

        //    var users = await _userDal.GetList(); 

        //    var userDtos = users.Select(user => new UserGetDto
        //    {
        //        Id= user.Id,
        //        Name = user.Name,
        //        Email = user.Email,
        //        Phone = user.Phone,
        //        CreatedDate = user.CreatedDate,
        //        DateOfBirth = user.DateOfBirth,
        //        PackageId= user.PackageId,
        //        TrainerId=user.TrainerId,
        //        ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null
        //    }).ToList();

        //    return userDtos;


        //}

        public async Task<UserGetDto> GetUserById(int id)
        {
            var user = await _userDal.Get(u => u.Id == id);

           
            var userDto = new UserGetDto
            {
                Id=user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                CreatedDate = user.CreatedDate,
                DateOfBirth = user.DateOfBirth,
                ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null
            };

            return userDto; 
        }

        public async Task<List<TopUserDto>> GetTop10UsersByPointsAsync()
        {
            var users = await _userDal.GetList();

            var topUsers = users
                .OrderByDescending(u => u.Point)
                .Take(10)
                .Select(u => new TopUserDto
                {

                    UserId = u.Id,
                    ImageUrl = u.ImageUrl != null ? _fileService.GetFileUrl(u.ImageUrl) : null,
                    Name = u.Name,
                    Point = u.Point,
                })
                .ToList();

            return topUsers;
        }
        //detayli sekilde gormek ucun asagidakilari yazdim hansiki user artiq hansi pakete uzv oldugunu qiymetini trainer name-i fln admin gore biler ve silib update ede biler
        public async Task<List<UserPackageTrainerDto>> GetAllUserPackageTrainer(string searchTerm = null)
        {
            var users = await _userDal.GetList(
                filter: null,
                include: q => q
                    .Include(u => u.Package)
                    .Include(u => u.Trainer)
            );

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string normalizedSearch = searchTerm.Trim().ToLower();
                users = users.Where(user =>
                    (!string.IsNullOrEmpty(user.Name) && user.Name.ToLower().Contains(normalizedSearch)) ||
                    (!string.IsNullOrEmpty(user.Phone) && user.Phone.ToLower().Contains(normalizedSearch)) ||
                    (!string.IsNullOrEmpty(user.Package?.PackageName) && user.Package.PackageName.ToLower().Contains(normalizedSearch)) ||
                    (!string.IsNullOrEmpty(user.Trainer?.Name) && user.Trainer.Name.ToLower().Contains(normalizedSearch))
                ).ToList();
            }

            return users.Select(user => new UserPackageTrainerDto
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null,
                PackageName = user.Package?.PackageName,
                PackagePrice = user.Package?.Price,
                TrainerName = user.Trainer?.Name
            }).ToList();
        }


        //PAYMENT
        public async Task<List<UserPackageTrainerDto>> GetPayments()
        {
            var users = await _userDal.GetList(
    filter: u => u.PackageId != null,
    include: q => q
        .Include(u => u.Package)
        .Include(u => u.Trainer)
);

            return users.Select(user => new UserPackageTrainerDto
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null,
                PackageName = user.Package?.PackageName,
                PackagePrice = user.Package?.Price,
                TrainerName = user.Trainer?.Name
            }).ToList();
        }
        public async Task<UserPackageTrainerDto> GetUserPackageTrainer(int id)
        {
            var user = await _userDal.Get(u => u.Id == id, include: q => q
                .Include(u => u.Package)
                .Include(u => u.Trainer));

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return new UserPackageTrainerDto
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null,
                PackageName = user.Package?.PackageName,
                PackagePrice = user.Package?.Price,
                TrainerName = user.Trainer?.Name
            };
        }
        //admin trainer ve paketi burada teyin edir
        public async Task UpdateUserPackageTrainer(int id, UserPackageTrainerUpdateDto dto)
        {
            var user = await _userDal.Get(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
                user.Phone = dto.Phone;

            if (dto.PackageId.HasValue)
                user.PackageId = dto.PackageId;

            if (dto.TrainerId.HasValue)
                user.TrainerId = dto.TrainerId;

            if (dto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(dto.ImageUrl);
                user.ImageUrl = imageUrl;
            }

            user.UpdatedDate = DateTime.Now;

            await _userDal.Update(user);
        }


     



    }
}
