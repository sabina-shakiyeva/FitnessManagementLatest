using AutoMapper;
using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Models;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using FitnessManagement.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class EquipmentService:IEquipmentService
    {
        private readonly IEquipmentDal _equipmentDal;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        public EquipmentService(IEquipmentDal equipmentDal, IMapper mapper, IFileService fileService)
        {
            _equipmentDal = equipmentDal;
            _mapper = mapper;
            _fileService = fileService;
        }
        public async Task AddEquipment(EquipmentDto equipmentDto)
        {
            var equipment = _mapper.Map<Equipment>(equipmentDto);

            if (equipmentDto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(equipmentDto.ImageUrl);
                equipment.ImageUrl = imageUrl;
            }

            await _equipmentDal.Add(equipment);
        }
        public async Task DeleteEquipment(int equipmentId)
        {
            var equipment = await _equipmentDal.Get(e => e.Id == equipmentId);
            if (equipment == null)
            {
                throw new Exception("Equipment not found!");
            }

            await _equipmentDal.Delete(equipment);
        }

        public async Task UpdateEquipment(int equipmentId, EquipmentDto equipmentDto)
        {
            var equipment = await _equipmentDal.Get(e => e.Id == equipmentId);
            if (equipment == null)
            {
                throw new Exception("Equipment not found!");
            }

            _mapper.Map(equipmentDto, equipment);

            if (equipmentDto.ImageUrl != null)
            {
                string imageUrl = await _fileService.UploadFileAsync(equipmentDto.ImageUrl);
                equipment.ImageUrl = imageUrl;
            }
            equipment.UpdatedDate = DateTime.Now;
            await _equipmentDal.Update(equipment);
        }

        public async Task<List<EquipmentGetDto>> GetAllEquipment()
        {

            var equipments = await _equipmentDal.GetList();

            var equipmentDtos = equipments.Select(equipment => new EquipmentGetDto
            {
                Id = equipment.Id,
                Name = equipment.Name,      
                Description = equipment.Description,
                IsAvailable = equipment.IsAvailable,
                Price = equipment.Price,
                Unit = equipment.Unit,
                ImageUrl = equipment.ImageUrl != null ? _fileService.GetFileUrl(equipment.ImageUrl) : null
            }).ToList();

            return equipmentDtos;
         

        }

        public async Task<EquipmentGetDto> GetEquipmentById(int id)
        {
          
            var equipment = await _equipmentDal.Get(u => u.Id == id);


            var equipmentDto = new EquipmentGetDto
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Description = equipment.Description,
                IsAvailable = equipment.IsAvailable,
                Price = equipment.Price,
                Unit = equipment.Unit,
                ImageUrl = equipment.ImageUrl != null ? _fileService.GetFileUrl(equipment.ImageUrl) : null
            };

            return equipmentDto;
        }
      
    }
}
