using Fitness.Entities.Models;
using Fitness.Entities.Models.Group;
using Fitness.Entities.Models.Trainer;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface ITrainerScheduleService    {
        Task<TrainerScheduleDto> CreateScheduleAsync(TrainerScheduleCreateDto dto);
        Task<List<TrainerScheduleDetailedDto>> GetAllAsync();
        Task<TrainerScheduleDto> GetByIdAsync(int id);
        Task<List<TrainerScheduleDto>> GetByUserIdAsync(int userId);
        Task<List<TrainerScheduleDto>> GetByGroupIdAsync(int groupId);
        Task<bool> DeleteAsync(int id);
        Task<TrainerScheduleDto> UpdateAsync(TrainerScheduleUpdateDto dto);
        Task<List<TrainerScheduleDetailedDto>> GetSchedulesByTrainerIdentityIdAsync(string identityTrainerId);
        Task<List<GroupGetDto>> GetGroupsByTrainerIdentityIdAsync(string trainerIdentityId);
        Task<List<UserGetDto>> GetUsersInTrainerGroupAsync(string trainerIdentityId, int groupId);



    }
}
