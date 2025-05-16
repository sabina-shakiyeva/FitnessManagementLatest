using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Group;
using FitnessManagement.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class GroupService : IGroupService
    {
        private readonly IGroupDal _groupDal;
        private readonly IUserDal _userDal;
        private readonly IGroupUserDal _groupUserDal;
        private readonly ITrainerScheduleDal _trainerScheduleDal;
        private readonly IFileService _fileService;
        public GroupService(IGroupDal groupDal, IUserDal userDal, IGroupUserDal groupUserDal, ITrainerScheduleDal trainerScheduleDal)
        {
            _groupDal = groupDal;
            _userDal = userDal;
            _groupUserDal = groupUserDal;
            _trainerScheduleDal = trainerScheduleDal;
        }


        //Groupdaki userleri gormek ucun
        public async Task<List<UserGetDto>> GetUsersByGroupIdAsync(int groupId)
        {
            var groupUsers = await _groupUserDal.GetList(gu => gu.GroupId == groupId, include: q => q.Include(gu => gu.User));

            return groupUsers.Select(gu => new UserGetDto
            {
                Id = gu.User.Id,
                Name = gu.User.Name,
                Email = gu.User.Email,
            }).ToList();
        }

        public async Task<Group> CreateGroupAsync(GroupCreateDto dto)
        {
            var group = new Group
            {
                Name = dto.Name,
                PackageId = dto.PackageId,
                TrainerId = dto.TrainerId
            };

            await _groupDal.Add(group);
            return group;
        }

        public async Task<List<GroupGetDto>> GetAllGroupsAsync(string? search = null)
        {
            var groups = await _groupDal.GetList(
                g => !g.IsDeleted &&
                    (string.IsNullOrEmpty(search) ||
                     g.Name.Contains(search) ||
                     g.Package.PackageName.Contains(search) ||
                     g.Trainer.Name.Contains(search)),
                include: query => query.Include(g => g.Package)
                                       .Include(g => g.Trainer)
            );

            return groups.Select(g => new GroupGetDto
            {
                Id = g.Id,
                Name = g.Name,
                PackageId = g.PackageId,
                PackageName = g.Package?.PackageName,
                TrainerId = g.TrainerId,
                TrainerName = g.Trainer?.Name
            }).ToList();
        }



        public async Task<GroupGetDto> GetGroupByIdAsync(int id)
        {
            //return await _groupDal.GetByIdAsync(id);
            var group = await _groupDal.Get(
        g => g.Id == id && !g.IsDeleted,
        include: query => query.Include(g => g.Package)
                               .Include(g => g.Trainer)
    );

            if (group == null) return null;

            return new GroupGetDto
            {
                Id = group.Id,
                Name = group.Name,
                PackageId = group.PackageId,
                PackageName = group.Package?.PackageName,
                TrainerId = group.TrainerId,
                TrainerName = group.Trainer?.Name
            };
        }

        public async Task<Group> UpdateGroupAsync(GroupUpdateDto dto)
        {
            var group = await _groupDal.GetByIdAsync(dto.Id);
            if (group == null) return null;

            group.Name = dto.Name;
            group.PackageId = dto.PackageId;
            group.TrainerId = dto.TrainerId;

            await _groupDal.Update(group);
            return group;
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            var group = await _groupDal.GetByIdAsync(id);
            if (group == null) return false;

            await _groupDal.Delete(group);
            return true;
        }
        public async Task<bool> RemoveUserFromGroupAsync(int groupId, int userId)
        {
            var groupUser = await _groupUserDal.Get(gu => gu.GroupId == groupId && gu.UserId == userId);

            if (groupUser == null)
                return false;

            await _groupUserDal.Delete(groupUser);
            return true;
        }

        public async Task<bool> AddUserToGroupAsync(AddUserToGroupDto dto)
        {
            var group = await _groupDal.GetByIdAsync(dto.GroupId);
            var user = await _userDal.GetByIdAsync(dto.UserId);

            if (group == null || user == null)
                return false;

            //var alreadyExists = await _groupUserDal.AnyAsync(gu => gu.GroupId == dto.GroupId && gu.UserId == dto.UserId);
            //if (alreadyExists)
            //    return false;
            var alreadyInAnotherGroup = await _groupUserDal.AnyAsync(gu => gu.UserId == dto.UserId);
            if (alreadyInAnotherGroup)
                return false;

            var groupUser = new GroupUser
            {
                GroupId = dto.GroupId,
                UserId = dto.UserId
            };

            await _groupUserDal.Add(groupUser);
            return true;
        }


    }

}