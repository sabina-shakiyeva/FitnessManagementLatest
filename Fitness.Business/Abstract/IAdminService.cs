using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IAdminService
    {
        Task<AdminProfileDto> GetAdminByIdAsync(string id);
        Task AddAdminAsync(ApplicationUser admin);
        Task UpdateAdminAsync(int adminId, AdminUpdateDto adminUpdateDto);
        Task<StatisticsDto> GetStatisticsAsync();
    }
}
