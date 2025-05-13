using AutoMapper;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Product;
using Fitness.Entities.Models.Trainer;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Mappers
{
    public class Mapper:Profile
    {
        public Mapper()
        {
            CreateMap<Equipment, EquipmentDto>().ReverseMap();
            CreateMap<TakeAttendanceDto, Attendance>();
            CreateMap<Package, PackageDto>().ReverseMap();
            CreateMap<Package, PackageGetDto>().ReverseMap();
            CreateMap<TrainerScheduleCreateDto, TrainerSchedule>();
            CreateMap<TrainerSchedule, TrainerScheduleDto>();
			CreateMap<Product, ProductCreateDto>().ReverseMap();
			CreateMap<Product, ProductUpdateDto>().ReverseMap();
			
		}
    }
}
