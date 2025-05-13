using Fitness.Business.Abstract;
using Fitness.Entities.Models.Group;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GroupCreateDto dto)
        {
            var group = await _groupService.CreateGroupAsync(dto);
            return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, group);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var result = await _groupService.GetGroupByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpGet("all-groups")]
        public async Task<IActionResult> GetAllGroups()
        {
            var result = await _groupService.GetAllGroupsAsync();
            return Ok(result);
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] GroupUpdateDto dto)
        {
            var updated = await _groupService.UpdateGroupAsync(dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _groupService.DeleteGroupAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

      
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromBody] AddUserToGroupDto dto)
        {
            var result = await _groupService.AddUserToGroupAsync(dto);
            if (!result) return BadRequest("Could not be added to group (user may not exist or has already been added)");
            return Ok("User added group successfully");
        }
        //groupdaki userleri gostermek ucundu
        [HttpGet("{groupId}/users")]
        public async Task<IActionResult> GetUsersByGroupId(int groupId)
        {
            var users = await _groupService.GetUsersByGroupIdAsync(groupId);
            return Ok(users);
        }

        [HttpDelete("{groupId}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromGroup(int groupId, int userId)
        {
            var result = await _groupService.RemoveUserFromGroupAsync(groupId, userId);
            if (!result)
                return NotFound("User or Group not found, or the connection is not available.");

            return Ok("User deleted succesfully.");
        }

    }
}
