using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.User;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IUserService
    {
        Task AddUser(UserDto userDto);
        //Task<List<UserGetDto>> GetAllUsers();
        Task<List<UserGetDto>> GetAllUsers(string searchTerm = null);
        Task<UserGetDto> GetUserById(int id);
        Task DeleteUser(int id);
        Task UpdateUser(int userId,UserUpdateDto userUpdateDto);
        Task<List<ApplicationUser>> GetPendingUsers(string search = null);
        Task ApproveUser(string userId);
        Task DeclineUser(string userId);
        Task<List<TopUserDto>> GetTop10UsersByPointsAsync();
        Task<UserPackageTrainerDto> GetUserPackageTrainer(int id);
        Task UpdateUserPackageTrainer(int id, UserPackageTrainerUpdateDto dto);
        Task<List<UserPackageTrainerDto>> GetPayments();
        Task<List<UserPackageTrainerDto>> GetAllUserPackageTrainer(string searchTerm = null);
        Task<UserProfileDto> GetUserProfile(string identityUserId);
        Task<StatisticsDto> GetStatisticsForUser();
        Task UpdateUserProfile(int userId, UserProfileUpdateDto userProfileUpdateDto);
        Task<List<TopUserDto>> GetTopUsersByGroupAsync(string identityUserId);
        Task<UserPackageInfoDto> GetUserPackageInfoAsync(string identityUserId);
        Task<User> GetByIdentityUserIdAsync(string identityUserId);
        Task<decimal> GetBalanceAsync(string identityUserId);








    }
}
