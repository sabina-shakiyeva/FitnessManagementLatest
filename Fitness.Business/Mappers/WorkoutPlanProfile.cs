using AutoMapper;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Workout;
using System.Linq;

namespace Fitness.Business.Mappers
{
    public class WorkoutPlanProfile : Profile
    {
        public WorkoutPlanProfile()
        {
            CreateMap<WorkoutExerciseDto, WorkoutExercise>().ReverseMap();

            CreateMap<WorkoutDayDto, WorkoutDay>()
                .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises))
                .ReverseMap();

            CreateMap<WorkoutPlanDto, WorkoutPlan>()
                .ForMember(dest => dest.WorkoutDays, opt => opt.MapFrom(src => src.Days))
                .ForMember(dest => dest.PackageWorkouts, opt => opt.MapFrom(src =>
                    src.PackageIds.Select(pid => new PackageWorkout { PackageId = pid })));

            CreateMap<WorkoutPlan, WorkoutPlanGetDto>()
                .ForMember(dest => dest.Days, opt => opt.MapFrom(src => src.WorkoutDays))
                .ForMember(dest => dest.PackageIds, opt => opt.MapFrom(src => src.PackageWorkouts.Select(p => p.PackageId)))
                .ReverseMap();
        }
    }
}


//using AutoMapper;
//using Fitness.Entities.Concrete;
//using Fitness.Entities.Models.Workout;


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Fitness.Business.Mappers
//{
//    public class WorkoutPlanProfile:Profile
//    {
//        public WorkoutPlanProfile()
//        {
//            CreateMap<WorkoutExerciseDto, WorkoutExercise>().ReverseMap();
//            CreateMap<WorkoutDayDto, WorkoutDay>()
//                .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises))
//                .ReverseMap();
//            CreateMap<WorkoutPlanDto, WorkoutPlan>()
//                .ForMember(dest => dest.WorkoutDays, opt => opt.MapFrom(src => src.Days))
//                .ForMember(dest => dest.PackageWorkouts, opt => opt.Ignore());

//            CreateMap<WorkoutPlan, WorkoutPlanGetDto>()
//                .ForMember(dest => dest.Days, opt => opt.MapFrom(src => src.WorkoutDays))
//                .ForMember(dest => dest.PackageIds, opt => opt.MapFrom(src => src.PackageWorkouts.Select(p => p.PackageId)));

//        }
//    }
//}
