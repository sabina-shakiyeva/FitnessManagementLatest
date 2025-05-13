using Fitness.Entities.Models.Workout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IWorkoutPlanService
    {
        Task<List<WorkoutPlanGetDto>> GetAllPlansAsync();
        Task<WorkoutPlanGetDto> GetPlanByIdAsync(int id);
        Task AddPlanAsync(WorkoutPlanDto dto);
        Task UpdatePlanAsync(int id, WorkoutPlanDto dto);
        Task DeletePlanAsync(int id);
        Task<WorkoutPlanGetDto> GetPlanByPackageIdAsync(int packageId);
    }
}
