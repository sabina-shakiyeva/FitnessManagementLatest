using Fitness.Business.Abstract;
using Fitness.Entities.Models;
using FitnessManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;


        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet("packages")]
        public async Task<IActionResult> GetAllPackages()
        {
            var result = await _packageService.GetAllPackages();
            return Ok(result);
        }

        [HttpGet("package/{id}")]
        public async Task<IActionResult> GetPackageById(int id)
        {
            var result = await _packageService.GetPackageById(id);
            return Ok(result);
        }

        [HttpPost("add-package")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPackage([FromBody] PackageDto packageDto)
        {
            await _packageService.AddPackage(packageDto);
            return Ok("Package added successfully.");
        }

        [HttpPut("update-package/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePackage(int id, [FromBody] PackageDto packageDto)
        {
            await _packageService.UpdatePackage(id, packageDto);
            return Ok("Package updated successfully.");
        }

        [HttpDelete("delete-package/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            await _packageService.DeletePackage(id);
            return Ok("Package deleted successfully.");
        }

        [HttpPost("suggest-package")]
        public async Task<IActionResult> SuggestPackageFromMeasurements([FromBody] UserBmiDto request)
        {
            var (bmi, package) = await _packageService.SuggestPackageFromMeasurementsAsync(request.WeightKg, request.HeightCm);
            return Ok(new
            {
                CalculatedBmi = bmi,
                SuggestedPackage = package
            });
        }


        




    }
}
