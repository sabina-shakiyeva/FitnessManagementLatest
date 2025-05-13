using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IUserEquipmentUsageService
    {
        Task AddAsync(EquipmentUsageStatDto dto);
        Task<List<UserEquipmentUsage>> GetAllByUserIdAsync(int userId);
        Task<List<EquipmentUsageStatDto>> GetUsageStatsByUserIdAsync(int userId);

        //butun equipmentleri getirsin deye yazdim
        Task<List<EquipmentUsageGetDto>> GetAllEquipmentUsages();
    }
}
