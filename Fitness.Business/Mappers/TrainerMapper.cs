using AutoMapper;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Mappers
{
    public class TrainerMapper:Profile
    {
        public TrainerMapper()
        {
            CreateMap<Trainer, TrainerDto>().ReverseMap();
        }
    }
}
