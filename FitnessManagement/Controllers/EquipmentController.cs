using Fitness.Business.Abstract;
using FitnessManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }
        [HttpGet("equipments")]
        public async Task<IActionResult> GetAllEquipment()
        {
            var equipmentList = await _equipmentService.GetAllEquipment();
            return Ok(equipmentList);
        }
        [HttpPost("add-equipment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEquipment([FromForm] EquipmentDto equipmentDto)
        {
           
            await _equipmentService.AddEquipment(equipmentDto);
            return Ok("Equipment is added");
        }

        [HttpGet("equipment/{id}")]
        public async Task<IActionResult> GetEquipmentById(int id)
        {
            var equipment = await _equipmentService.GetEquipmentById(id);
            return Ok(equipment);
        }

        [HttpPut("update-equipment/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEquipment(int id, [FromForm] EquipmentDto equipmentDto)
        {
            await _equipmentService.UpdateEquipment(id, equipmentDto);
            return Ok("Equipment updated.");
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            await _equipmentService.DeleteEquipment(id);
            return Ok("Equipment deleted.");
        }
    }
}
