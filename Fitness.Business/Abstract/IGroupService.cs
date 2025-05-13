using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IGroupService
    {
        Task<Group> CreateGroupAsync(GroupCreateDto dto);
        Task<GroupGetDto> GetGroupByIdAsync(int id);
        Task<Group> UpdateGroupAsync(GroupUpdateDto dto);
        Task<bool> DeleteGroupAsync(int id);
        Task<bool> AddUserToGroupAsync(AddUserToGroupDto dto);
        Task<List<GroupGetDto>> GetAllGroupsAsync();
        Task<List<UserGetDto>> GetUsersByGroupIdAsync(int groupId);
        Task<bool> RemoveUserFromGroupAsync(int groupId, int userId);

    }
}
