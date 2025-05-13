using AutoMapper;
using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Workout;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class WorkoutPlanService : IWorkoutPlanService
    {
        private readonly IWorkoutPlanDal _planDal;
        private readonly IWorkoutDayDal _dayDal;
        private readonly IWorkoutExerciseDal _exerciseDal;
        private readonly IPackageWorkoutDal _packageWorkoutDal;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IGlobalNotificationService _globalNotificationService;
        public WorkoutPlanService(
            IWorkoutPlanDal planDal,
            IWorkoutDayDal dayDal,
            IWorkoutExerciseDal exerciseDal,
            IPackageWorkoutDal packageWorkoutDal,
            IMapper mapper,
            INotificationService notificationService, IGlobalNotificationService globalNotificationService)
        {
            _planDal = planDal;
            _dayDal = dayDal;
            _exerciseDal = exerciseDal;
            _packageWorkoutDal = packageWorkoutDal;
            _mapper = mapper;
            _notificationService = notificationService;
            _globalNotificationService = globalNotificationService;
        }
        //public async Task<List<WorkoutPlanGetDto>> GetAllPlansAsync()
        //{
        //    var plans = await _planDal.GetList(
        //        filter: null,
        //        include: q => q.Include(p => p.WorkoutDays)
        //                       .ThenInclude(d => d.Exercises)
        //                       .Include(p => p.PackageWorkouts)
        //    );

        //    var result = plans.Select(plan => new WorkoutPlanGetDto
        //    {
        //        Id = plan.Id,
        //        Title = plan.Title,
        //        Description = plan.Description,
        //        TrainerId = plan.TrainerId,

        //        Days = plan.WorkoutDays?.Select(day => new WorkoutDayDto
        //        {
        //            DayNumber = day.DayNumber,
        //            Exercises = day.Exercises?.Select(ex => new WorkoutExerciseDto
        //            {
        //                EquipmentId = ex.EquipmentId,
        //                EquipmentName = ex.EquipmentName,
        //                Duration = ex.Duration,
        //                Repetitions = ex.Repetitions
        //            }).ToList()
        //        }).ToList(),

        //        PackageIds = plan.PackageWorkouts?.Select(p => p.PackageId).ToList()
        //    }).ToList();

        //    return result;
        //}

        public async Task<List<WorkoutPlanGetDto>> GetAllPlansAsync()
        {
            var plans = await _planDal.GetList(
                filter: null,
                include: q => q.Include(p => p.WorkoutDays)
                               .ThenInclude(d => d.Exercises)
                               .Include(p => p.PackageWorkouts)
            );
            return _mapper.Map<List<WorkoutPlanGetDto>>(plans);
        }

        public async Task<WorkoutPlanGetDto> GetPlanByIdAsync(int id)
        {
            var plan = await _planDal.Get(
                p => p.Id == id,
                include: q => q.Include(p => p.WorkoutDays)
                               .ThenInclude(d => d.Exercises)
                               .Include(p => p.PackageWorkouts)
            );

            if (plan == null)
                throw new Exception("Workout plan not found");

            return _mapper.Map<WorkoutPlanGetDto>(plan);
        }

    
        public async Task AddPlanAsync(WorkoutPlanDto dto)
        {
            var plan = new WorkoutPlan
            {
                Title = dto.Title,
                Description = dto.Description,
                TrainerId = dto.TrainerId,
            };

            await _planDal.Add(plan); 

            foreach (var dayDto in dto.Days)
            {
                var day = new WorkoutDay
                {
                    WorkoutPlanId = plan.Id,
                    DayNumber = dayDto.DayNumber
                };
                await _dayDal.Add(day);

                foreach (var exDto in dayDto.Exercises)
                {
                    var exercise = new WorkoutExercise
                    {
                        WorkoutDayId = day.Id,
                        EquipmentId = exDto.EquipmentId,
                        EquipmentName = exDto.EquipmentName,
                        Duration = exDto.Duration,
                        Repetitions = exDto.Repetitions
                    };
                    await _exerciseDal.Add(exercise);
                }
            }

            foreach (var packageId in dto.PackageIds)
            {
                var pw = new PackageWorkout
                {
                    WorkoutPlanId = plan.Id,
                    PackageId = packageId
                };
                await _packageWorkoutDal.Add(pw);
            }
            var message = $"Yeni workout plan əlavə olundu: {plan.Title}";
            await _globalNotificationService.CreateGlobalNotificationAsync(message);

            //await _notificationService.CreateNotificationForAllAsync(message);
        }


        public async Task UpdatePlanAsync(int id, WorkoutPlanDto dto)
        {
            var existing = await _planDal.Get(
                p => p.Id == id,
                include: q => q.Include(p => p.WorkoutDays)
                               .ThenInclude(d => d.Exercises)
                               .Include(p => p.PackageWorkouts)
            );

            if (existing == null)
                throw new Exception("Workout plan not found");

            existing.WorkoutDays.Clear();
            existing.PackageWorkouts.Clear();

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.TrainerId = dto.TrainerId;

            await _planDal.Update(existing);

            foreach (var dayDto in dto.Days)
            {
                var day = new WorkoutDay
                {
                    WorkoutPlanId = existing.Id,
                    DayNumber = dayDto.DayNumber
                };
                await _dayDal.Add(day);

                foreach (var exDto in dayDto.Exercises)
                {
                    var exercise = new WorkoutExercise
                    {
                        WorkoutDayId = day.Id,
                        EquipmentId = exDto.EquipmentId,
                        EquipmentName = exDto.EquipmentName,
                        Duration = exDto.Duration,
                        Repetitions = exDto.Repetitions
                    };
                    await _exerciseDal.Add(exercise);
                }
            }

            foreach (var packageId in dto.PackageIds)
            {
                var pw = new PackageWorkout
                {
                    WorkoutPlanId = existing.Id,
                    PackageId = packageId
                };
                await _packageWorkoutDal.Add(pw);
            }
        }
        public async Task<WorkoutPlanGetDto> GetPlanByPackageIdAsync(int packageId)
        {
            var plan = await _planDal.Get(
                filter: p => p.PackageWorkouts.Any(pw => pw.PackageId == packageId),
                include: q => q.Include(p => p.WorkoutDays)
                               .ThenInclude(d => d.Exercises)
                               .Include(p => p.PackageWorkouts)
            );

            if (plan == null)
                throw new Exception("Plan not found for this package");

            return _mapper.Map<WorkoutPlanGetDto>(plan);
        }


        public async Task DeletePlanAsync(int id)
        {
            var plan = await _planDal.Get(p => p.Id == id);
            if (plan == null)
                throw new Exception("Workout plan not found");

            await _planDal.Delete(plan);
        }
    }
}
