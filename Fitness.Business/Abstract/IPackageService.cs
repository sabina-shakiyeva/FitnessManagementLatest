using Fitness.Entities.Models;
using FitnessManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IPackageService
    {
        Task AddPackage(PackageDto packageDto);
        Task<List<PackageGetDto>> GetAllPackages();
        Task<PackageGetDto> GetPackageById(int id);
        Task UpdatePackage(int id, PackageDto packageDto);
        Task DeletePackage(int id); 
        Task<(double Bmi, PackageDto Package)> SuggestPackageFromMeasurementsAsync(double weightKg, double heightCm);


    }
}
