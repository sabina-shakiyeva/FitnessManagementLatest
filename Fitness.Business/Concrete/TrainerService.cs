using AutoMapper;
using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Trainer;
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
    public class TrainerService : ITrainerService
    {
        private readonly ITrainerDal _trainerDal;
        private readonly IUserDal _userDal;
        private readonly IEquipmentDal _equipmentDal;
        private readonly IPackageDal _packageDal;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IAttendanceDal _attendanceDal;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TrainerService(ITrainerDal trainerDal, IMapper mapper, IFileService fileService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserDal userDal, IEquipmentDal equipmentDal, IPackageDal packageDal, IAttendanceDal attendanceDal)
        {
            _trainerDal = trainerDal;
            _mapper = mapper;
            _fileService = fileService;
            _userManager = userManager;
            _roleManager = roleManager;
            _userDal = userDal;
            _equipmentDal = equipmentDal;
            _packageDal = packageDal;
            _attendanceDal = attendanceDal;
        }

		public async Task<TrainerProfileDto> GetTrainerProfile(string identityTrainerId)
		{
			var trainer = await _trainerDal.Get(t => t.IdentityTrainerId == identityTrainerId);
			if (trainer == null)
				throw new Exception("Trainer not found");

			var identityUser = await _userManager.FindByIdAsync(identityTrainerId);
			if (identityUser == null)
				throw new Exception("Identity user not found");

			return new TrainerProfileDto
			{
				Id = trainer.Id,
				Name = trainer.Name,
				Email = identityUser.Email,
				ImageUrl = trainer.ImageUrl != null ? _fileService.GetFileUrl(trainer.ImageUrl) : null,
				Experience = trainer.Experience,
				Salary = trainer.Salary,
				CreatedDate = trainer.CreatedDate,
				MobileTelephone = trainer.MobileTelephone
			};
		}
        public async Task UpdateTrainerProfile(int trainerId, TrainerUpdateProfileDto trainerUpdateDto)
        {
            var trainer = await _trainerDal.Get(t => t.Id == trainerId);
            if (trainer == null)
            {
                throw new Exception("Trainer not found");
            }

            var identityUser = await _userManager.FindByIdAsync(trainer.IdentityTrainerId);
            if (identityUser == null)
            {
                throw new Exception("Identity user not found");
            }
            if (trainerUpdateDto.MobileTelephone != null)
            {
                trainer.MobileTelephone = trainerUpdateDto.MobileTelephone;

            }

            if (!string.IsNullOrEmpty(trainerUpdateDto.Name))
            {
                trainer.Name = trainerUpdateDto.Name;
                identityUser.FullName = trainerUpdateDto.Name;
            }

            if (!string.IsNullOrEmpty(trainerUpdateDto.Email))
            {
                trainer.Email = trainerUpdateDto.Email;
                identityUser.Email = trainerUpdateDto.Email;
                identityUser.UserName = trainerUpdateDto.Email;

            }
            if (!string.IsNullOrEmpty(trainerUpdateDto.Description))
            {
                trainer.Description = trainerUpdateDto.Description;
            }

            if (!string.IsNullOrEmpty(trainerUpdateDto.Experience))
            {
                trainer.Experience = trainerUpdateDto.Experience;
            }

            if (!string.IsNullOrEmpty(trainerUpdateDto.Speciality))
            {
                trainer.Speciality = trainerUpdateDto.Speciality;
            }

            if (trainerUpdateDto.Salary.HasValue)
            {
                trainer.Salary = trainerUpdateDto.Salary.Value;
            }

          


            if (trainerUpdateDto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(trainerUpdateDto.ImageUrl);
                trainer.ImageUrl = imageUrl;
            }
         
            if (!string.IsNullOrEmpty(trainerUpdateDto.NewPassword))
            {
                if (!string.IsNullOrEmpty(trainerUpdateDto.CurrentPassword))
                {

                    var checkPassword = await _userManager.CheckPasswordAsync(identityUser, trainerUpdateDto.CurrentPassword);

                    if (!checkPassword)
                    {
                        throw new Exception("Current password is incorrect!");
                    }
                    var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                    var result = await _userManager.ResetPasswordAsync(identityUser, token, trainerUpdateDto.NewPassword);
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
            trainer.UpdatedDate = DateTime.Now;
            await _userManager.UpdateAsync(identityUser);
            await _trainerDal.Update(trainer);
        }

        //Say statistikasi
        public async Task<TrainerStatisticsDto> GetTrainerStatisticsAsync(string trainerIdentityId)
        {
            var trainer = await _trainerDal.Get(t => t.IdentityTrainerId == trainerIdentityId);

            if (trainer == null)
                throw new Exception("Trainer not found");

            var myUsers = await _userDal.GetList(u => u.TrainerId == trainer.Id);

           
            var allUsers = await _userDal.GetList();

            
            var allEquipments = await _equipmentDal.GetList();

            
            var allPackages = await _packageDal.GetList();

            return new TrainerStatisticsDto
            {
                TotalMember = allUsers.Count,
                TotalMyMember = myUsers.Count,
                TotalEquipment = allEquipments.Count,
                NumberOfPackages = allPackages.Count
            };
        }
        //Trainer-in oz user attendance lerini gorur
        public async Task<List<AttendanceGetDto>> GetTrainerAttendanceListAsync(string trainerIdentityId)
        {
            var trainer = await _trainerDal.Get(t => t.IdentityTrainerId == trainerIdentityId);

            if (trainer == null)
                throw new Exception("Trainer not found");

            var users = await _userDal.GetList(
                filter: u => u.IsActive && u.TrainerId == trainer.Id,
                include: q => q.Include(u => u.Package)
            );

            return users.Select(user => new AttendanceGetDto
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null,
                PackageName = user.Package?.PackageName
            }).ToList();
        }
        //Trainer oz userlerini attendance-lerini qeyd ede bilir burada
        public async Task TakeAttendanceByTrainerAsync(string trainerIdentityId, TakeAttendanceDto dto)
        {
           
            var trainer = await _trainerDal.Get(t => t.IdentityTrainerId == trainerIdentityId);

            if (trainer == null)
                throw new Exception("Trainer not found");

           
            var user = await _userDal.Get(u => u.Id == dto.UserId && u.TrainerId == trainer.Id);

            if (user == null)
                throw new Exception("You are not authorized to take attendance for this user.");

           
            var existingAttendance = await _attendanceDal.GetList(
                a => a.UserId == dto.UserId && a.AttendanceDate.Date == dto.AttendanceDate.Date
            );

            if (existingAttendance.Any())
            {
                var attendance = existingAttendance.First();
                attendance.Status = dto.Status;
                await _attendanceDal.Update(attendance);
            }
            else
            {
                var attendance = _mapper.Map<Attendance>(dto);
                attendance.AttendanceDate = dto.AttendanceDate.Date;
                await _attendanceDal.Add(attendance);
            }
        }

        //


        //public async Task<StatisticsDto> GetStatisticsAsync()
        //{
        //    var userCount = await _userDal.GetList();
        //    var trainerCount = await _trainerDal.GetList();
        //    var equipmentCount = await _equipmentDal.GetList();
        //    var packageCount = await _packageDal.GetList();

        //    return new StatisticsDto
        //    {
        //        NumberOfUsers = userCount.Count,
        //        NumberOfTrainers = trainerCount.Count,
        //        NumberOfEquipments = equipmentCount.Count,
        //        NumberOfPackages = packageCount.Count
        //    };
        //}
        public async Task ApproveTrainer(string trainerId)
        {
            
            var trainer = await _userManager.Users
                .Where(u => u.Id == trainerId && !u.IsApproved)  
                .FirstOrDefaultAsync();

            if (trainer == null)
            {
                throw new Exception("Trainer tapılmadı və ya artıq təsdiqlənib.");
            }

           
            trainer.IsApproved = true;

            var identityUser = await _userManager.FindByIdAsync(trainerId);
            if (identityUser == null)
            {
                throw new Exception("IdentityUser tapılmadı.");
            }

            var passwordHash = identityUser.PasswordHash;
            var passwordSalt = identityUser.SecurityStamp;

            var saltBytes = Encoding.UTF8.GetBytes(passwordSalt);
            var hashBytes = Encoding.UTF8.GetBytes(passwordHash);

         
            var existingTrainer = await _trainerDal.Get(t => t.IdentityTrainerId == trainer.Id);
            if (existingTrainer == null)
            {
                var newTrainer = new Trainer
                {
                    IdentityTrainerId = identityUser.Id,
                    Name = identityUser.FullName,
                    Email = identityUser.Email,
                    IsActive = true,
                    IsApproved = true,
                    PasswordHash = hashBytes,
                    PasswordSalt = saltBytes,
                };
                await _trainerDal.Add(newTrainer);
            }

           
            var result = await _userManager.UpdateAsync(identityUser);
            if (!result.Succeeded)
            {
                throw new Exception("Trainer məlumatları yenilənərkən səhv baş verdi.");
            }
        }

        public async Task<List<ApplicationUser>> GetPendingTrainers()
        {
            var allUsers = await _userManager.Users
                .Where(u => !u.IsApproved)
                .ToListAsync();

            var pendingTrainers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Trainer"))
                {
                    pendingTrainers.Add(user);
                }
            }

            return pendingTrainers;
        }
        public async Task DeclineTrainer(string trainerId)
        {

            var trainer = await _userManager.Users.FirstOrDefaultAsync(t => t.Id == trainerId && !t.IsApproved);

            if (trainer != null)
            {

                await _userManager.DeleteAsync(trainer);
            }
            else
            {
                throw new Exception("Trainer not found in pending trainers!");
            }
        }


        public async Task AddTrainer(TrainerDto trainerDto)
        {
           
            var trainer = _mapper.Map<Trainer>(trainerDto);

            
            var identityTrainer = new ApplicationUser
            {
                FullName = trainerDto.Name,
                UserName = trainerDto.Email,
                Email = trainerDto.Email,
                IsApproved = trainerDto.IsApproved,
            };

            var result = await _userManager.CreateAsync(identityTrainer, trainerDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception("Təlimçi əlavə edilə bilmədi: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

           
            trainer.IdentityTrainerId = identityTrainer.Id;

            
            var passwordHash = identityTrainer.PasswordHash;
            var passwordSalt = identityTrainer.SecurityStamp;
            var saltBytes = Encoding.UTF8.GetBytes(passwordSalt);
            var hashBytes = Encoding.UTF8.GetBytes(passwordHash);
            trainer.PasswordHash = hashBytes;
            trainer.PasswordSalt = saltBytes;

       
            if (trainerDto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(trainerDto.ImageUrl);
                trainer.ImageUrl = imageUrl;
            }

           
            if (trainerDto.IsApproved != null) 
            {
                trainer.IsApproved = trainerDto.IsApproved;
            }
            trainer.Description = trainerDto.Description;
            trainer.Experience = trainerDto.Experience;
            trainer.Salary = trainerDto.Salary ?? 0; 
            //trainer.JoinedDate = trainerDto.JoinedDate ?? DateTime.UtcNow; 
            trainer.MobileTelephone = trainerDto.MobileTelephone;
            await _trainerDal.Add(trainer);


            if (!await _roleManager.RoleExistsAsync("Trainer"))
            {
                
                await _roleManager.CreateAsync(new IdentityRole("Trainer"));
            }
            await _userManager.AddToRoleAsync(identityTrainer, "Trainer");
        }


        public async Task DeleteTrainer(int trainerId)
        {
            var trainer = await _trainerDal.Get(t => t.Id == trainerId);
            if (trainer == null)
            {
                throw new Exception("Trainer not found");
            }

            var identityUser = await _userManager.FindByIdAsync(trainer.IdentityTrainerId);
            if (identityUser != null)
            {

                var result = await _userManager.DeleteAsync(identityUser);
                if (!result.Succeeded)
                {
                    throw new Exception("Error occurred during deletion: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            await _trainerDal.Delete(trainer);
        }
        public async Task<List<TrainerGetDto>> GetAllTrainers()
        {
            var trainers = await _trainerDal.GetList();

            var trainerDtos = trainers.Select(trainer => new TrainerGetDto
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                MobileTelephone = trainer.MobileTelephone,
                CreatedDate = trainer.CreatedDate,
                Experience=trainer.Experience,
                Salary=trainer.Salary,
                ImageUrl = trainer.ImageUrl != null ? _fileService.GetFileUrl(trainer.ImageUrl) : null
            }).ToList();

            return trainerDtos;
        }
        //TRAINER PROFILE
        public async Task<TrainerGetDto> GetTrainerById(int id)
        {
            var trainer = await _trainerDal.Get(t => t.Id == id);


            var trainerDto = new TrainerGetDto
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                MobileTelephone = trainer.MobileTelephone,
                CreatedDate = trainer.CreatedDate,
                Experience = trainer.Experience,
                Salary = trainer.Salary,
                ImageUrl = trainer.ImageUrl != null ? _fileService.GetFileUrl(trainer.ImageUrl) : null
            };

            return trainerDto;
        }

        public async Task UpdateTrainer(int trainerId,TrainerUpdateDto trainerUpdateDto)
        {
            var trainer = await _trainerDal.Get(t => t.Id == trainerId);
            if (trainer == null)
            {
                throw new Exception("Trainer not found");
            }

            var identityUser = await _userManager.FindByIdAsync(trainer.IdentityTrainerId);
            if (identityUser == null)
            {
                throw new Exception("Identity user not found");
            }
            if (trainerUpdateDto.MobileTelephone != null)
            {
                trainer.MobileTelephone = trainerUpdateDto.MobileTelephone;

            }

            if (!string.IsNullOrEmpty(trainerUpdateDto.Name))
            {
                trainer.Name = trainerUpdateDto.Name;
                identityUser.FullName = trainerUpdateDto.Name;
            }
 
            if (!string.IsNullOrEmpty(trainerUpdateDto.Email))
            {
                trainer.Email = trainerUpdateDto.Email;
                identityUser.Email = trainerUpdateDto.Email;
                identityUser.UserName = trainerUpdateDto.Email;

            }
            if (!string.IsNullOrEmpty(trainerUpdateDto.Description))
            {
                trainer.Description = trainerUpdateDto.Description;
            }

            if (!string.IsNullOrEmpty(trainerUpdateDto.Experience))
            {
                trainer.Experience = trainerUpdateDto.Experience;
            }

            if (!string.IsNullOrEmpty(trainerUpdateDto.Speciality))
            {
                trainer.Speciality = trainerUpdateDto.Speciality;
            }

            if (trainerUpdateDto.Salary.HasValue)
            {
                trainer.Salary = trainerUpdateDto.Salary.Value;
            }

            if (trainerUpdateDto.IsApproved)
            {
                trainer.IsApproved = trainerUpdateDto.IsApproved;
                identityUser.IsApproved = trainerUpdateDto.IsApproved;
            }


            if (trainerUpdateDto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(trainerUpdateDto.ImageUrl);
                trainer.ImageUrl = imageUrl;
            }
            if (trainerUpdateDto.IsActive)
            {
                trainer.IsActive = trainerUpdateDto.IsActive;

            }
            if (!string.IsNullOrEmpty(trainerUpdateDto.NewPassword))
            {
                if (!string.IsNullOrEmpty(trainerUpdateDto.CurrentPassword))
                {

                    var checkPassword = await _userManager.CheckPasswordAsync(identityUser, trainerUpdateDto.CurrentPassword);

                    if (!checkPassword)
                    {
                        throw new Exception("Current password is incorrect!");
                    }
                    var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                    var result = await _userManager.ResetPasswordAsync(identityUser, token,trainerUpdateDto.NewPassword);
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
            trainer.UpdatedDate = DateTime.Now;
            await _userManager.UpdateAsync(identityUser);
            await _trainerDal.Update(trainer);
        }

        //asagida yazdiqlarim sirf trainer id-ye gorer userlerin get,update,delete olacaq

        //MY MEMBERS


        //Get All My members
        public async Task<List<UserGetDto>> GetUsersByTrainerId(string trainerIdentityId)
        {
            var trainer = await _trainerDal.Get(t => t.IdentityTrainerId == trainerIdentityId);

            if (trainer == null)
                throw new Exception("Trainer not found");

            //var users = await _userDal.GetList(u => u.TrainerId == trainer.Id);
            var users = await _userDal.GetList(
    u => u.TrainerId == trainer.Id,
    include: q => q.Include(u => u.Package)
);


            var userDtos = users.Select(user => new UserGetDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                CreatedDate = user.CreatedDate,
                DateOfBirth = user.DateOfBirth,
                PackageName = user.Package?.PackageName,
                TrainerId = user.TrainerId,
                ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null
            }).ToList();

            return userDtos;
        }


        //public async Task<List<UserGetDto>> GetAllUsersByTrainer(int trainerId)
        //{
        //    var users = await _userDal.GetList(u => u.TrainerId == trainerId);

        //    var userDtos = users.Select(user => new UserGetDto
        //    {
        //        Id = user.Id,
        //        Name = user.Name,
        //        Email = user.Email,
        //        Phone = user.Phone,
        //        CreatedDate = user.CreatedDate,
        //        DateOfBirth = user.DateOfBirth,
        //        ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null
        //    }).ToList();

        //    return userDtos;
        //}

        public async Task<Trainer> GetTrainerByIdentityId(string identityId)
        {
            var trainer = await _trainerDal.Get(t => t.IdentityTrainerId == identityId);

            if (trainer == null)
                throw new Exception("Trainer not found");

            return trainer;
        }

        public async Task<UserGetDto> GetUserByIdForTrainer(int userId, string trainerIdentityId)
        {
            var trainer = await _trainerDal.Get(t => t.IdentityTrainerId == trainerIdentityId);
            if (trainer == null)
                throw new Exception("Trainer not found");

            var user = await _userDal.Get(
                u => u.Id == userId && u.TrainerId == trainer.Id,
                include: q => q.Include(u => u.Package)
            );

            if (user == null)
                throw new Exception("User not found or not associated with this trainer");

            return new UserGetDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                CreatedDate = user.CreatedDate,
                DateOfBirth = user.DateOfBirth,
                PackageName = user.Package?.PackageName,
                TrainerId = user.TrainerId,
                ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null
            };
        }

        //Trainere aid userlerin update olunmasi
        public async Task UpdateUserByTrainer(int userId, int trainerId, UserUpdateDto userUpdateDto)
        {
            var user = await _userDal.Get(u => u.Id == userId && u.TrainerId == trainerId);
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




            if (userUpdateDto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(userUpdateDto.ImageUrl);
                user.ImageUrl = imageUrl;
            }
            if (userUpdateDto.IsActive)
            {
                user.IsActive = userUpdateDto.IsActive;

            }

            

            // TrainerId ve PackageId 
            if (userUpdateDto.TrainerId.HasValue)
            {
                user.TrainerId = userUpdateDto.TrainerId;
            }

            if (userUpdateDto.PackageId.HasValue)
            {
                user.PackageId = userUpdateDto.PackageId;
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
        //trainere aid userlerin delete olunmasi
        public async Task DeleteUserByTrainer(int userId, int trainerId)
        {
            var user = await _userDal.Get(u => u.Id == userId && u.TrainerId == trainerId);
            if (user == null)
                throw new Exception("This user does not belong to you.");

            var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            if (identityUser != null)
            {
                var result = await _userManager.DeleteAsync(identityUser);
                if (!result.Succeeded)
                    throw new Exception("Error occurred during deletion: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userDal.Delete(user);
        }

       


    }
}
