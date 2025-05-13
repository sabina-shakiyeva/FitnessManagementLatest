using Fitness.Entities.Models;
using FitnessManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IEquipmentService
    {
        Task AddEquipment(EquipmentDto equipmentDto);
        Task<List<EquipmentGetDto>> GetAllEquipment();
        Task<EquipmentGetDto> GetEquipmentById(int id);
        Task DeleteEquipment(int equipmentId);
        Task UpdateEquipment(int equipmentId, EquipmentDto equipmentUpdateDto);
    }
}
