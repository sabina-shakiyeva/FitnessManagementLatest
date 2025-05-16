using AutoMapper;
using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Models;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class PackageService : IPackageService
    {
        private readonly IPackageDal _packageDal;
        private readonly IMapper _mapper;
        private readonly IUserDal _userDal;
        private readonly ITrainerDal _trainerDal;
        private readonly INotificationService _notificationService;
        private readonly IGlobalNotificationService _globalNotificationService;

        public PackageService(IPackageDal packageDal, IMapper mapper, IUserDal userDal, ITrainerDal trainerDal, INotificationService notificationService, IGlobalNotificationService globalNotificationService)
        {
            _packageDal = packageDal;
            _mapper = mapper;
            _userDal = userDal;
            _trainerDal = trainerDal;
            _notificationService = notificationService;
            _globalNotificationService = globalNotificationService;
        }
        public async Task AddPackage(PackageDto packageDto)
        {
            var package = _mapper.Map<Package>(packageDto);
            await _packageDal.Add(package);

            var users = await _userDal.GetList();
            var trainers = await _trainerDal.GetList();

            string message = $"Yeni paket əlavə olundu: {package.PackageName}";

            //var allReceivers = users.Select(u => u.Id)
            //                        .Concat(trainers.Select(t => t.Id))
            //                        .ToList();

            //foreach (var id in allReceivers)
            //{
            //    await _notificationService.CreateNotificationAsync(id, message);
            //}

            // Global bildiriş əlavə olunur
            await _globalNotificationService.CreateGlobalNotificationAsync(message);
        }

        //public async Task AddPackage(PackageDto packageDto)
        //{
        //    var package = _mapper.Map<Package>(packageDto);
        //    await _packageDal.Add(package);

        //    var users = await _userDal.GetList();
        //    var trainers = await _trainerDal.GetList();

        //    string message = $"Yeni paket əlavə olundu: {package.PackageName}";

        //    var allReceivers = users.Select(u => u.Id)
        //                            .Concat(trainers.Select(t => t.Id))
        //                            .ToList();

        //    foreach (var id in allReceivers)
        //    {
        //        await _notificationService.CreateNotificationAsync(id, message);
        //    }
        //}

        public async Task DeletePackage(int id)
        {
            var package = await _packageDal.Get(p => p.Id == id);
            if (package == null)
                throw new Exception("Package not found!");

            await _packageDal.Delete(package);
        }


        public async Task<List<PackageGetDto>> GetAllPackages(string? search = null)
        {
            var packages = await _packageDal.GetList(
                p => string.IsNullOrEmpty(search) || p.PackageName.Contains(search)
            );

            return _mapper.Map<List<PackageGetDto>>(packages);
        }


        public async Task<PackageGetDto> GetPackageById(int id)
        {
            var package = await _packageDal.Get(p => p.Id == id);
            if (package == null)
                throw new Exception("Package not found!");

            return _mapper.Map<PackageGetDto>(package);
        }

        public async Task UpdatePackage(int id, PackageDto packageDto)
        {
            var package = await _packageDal.Get(p => p.Id == id);
            if (package == null)
                throw new Exception("Package not found!");

            _mapper.Map(packageDto, package);
            package.UpdatedDate = DateTime.Now;
            await _packageDal.Update(package);
        }

      
        public async Task<(double Bmi, PackageDto Package)> SuggestPackageFromMeasurementsAsync(double weightKg, double heightCm)
        {
            if (heightCm <= 0 || weightKg <= 0)
                throw new ArgumentException("Height and weight must be greater than zero.");

            double heightM = heightCm / 100;
            double bmi = weightKg / (heightM * heightM);
            string packageName;

            if (bmi < 18.5)
                packageName = "Muscle Builder";
            else if (bmi < 25)
                packageName = "Balance Body";
            else if (bmi < 30)
                packageName = "Fit Burn";
            else
                packageName = "Health Reset";

            var package = await _packageDal.Get(p => p.PackageName == packageName);

            if (package == null)
                throw new Exception("Package not found");

            var packageDto = _mapper.Map<PackageDto>(package);

            return (Math.Round(bmi, 2), packageDto);
        }


    }
}
