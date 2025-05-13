using AutoMapper;
using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Models.Group;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Trainer;
using FitnessManagement.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class TrainerScheduleService : ITrainerScheduleService
    {
        private readonly ITrainerScheduleDal _trainerScheduleDal;
        private readonly IMapper _mapper;
        private readonly IGroupUserDal _groupUserDal;

        public TrainerScheduleService(ITrainerScheduleDal trainerScheduleDal, IMapper mapper, IGroupUserDal groupUserDal)
        {
            _trainerScheduleDal = trainerScheduleDal;
            _mapper = mapper;
            _groupUserDal = groupUserDal;
        }


        //Admin hissede asagidakilar olacaq

        public async Task<TrainerScheduleDto> CreateScheduleAsync(TrainerScheduleCreateDto dto)
        {
            bool isUserProvided = dto.UserId.HasValue;
            bool isGroupProvided = dto.GroupId.HasValue;

            if (isUserProvided == isGroupProvided) 
                throw new InvalidOperationException("Yalnız UserId və ya GroupId təyin olunmalıdır.");

           
            var startTime = new TimeOnly(dto.StartTime.Hour, dto.StartTime.Minute);
            var endTime = new TimeOnly(dto.EndTime.Hour, dto.EndTime.Minute);

            var conflicts = await _trainerScheduleDal.GetList(s =>
                s.TrainerId == dto.TrainerId &&
                s.DayOfWeek == dto.DayOfWeek &&
                (
                    (startTime >= s.StartTime && startTime < s.EndTime) ||
                    (endTime > s.StartTime && endTime <= s.EndTime) ||
                    (startTime <= s.StartTime && endTime >= s.EndTime)
                )
            );

            if (conflicts.Any())
                throw new InvalidOperationException("Bu tarixdə artıq təyin olunmuş dərs mövcuddur.");

           
            var schedule = new TrainerSchedule
            {
                TrainerId = dto.TrainerId,
                UserId = dto.UserId,
                GroupId = dto.GroupId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                Description = dto.Description
            };

            await _trainerScheduleDal.Add(schedule);
            return _mapper.Map<TrainerScheduleDto>(schedule);
        }


        public async Task<List<TrainerScheduleDetailedDto>> GetAllAsync()
{
    var schedules = await _trainerScheduleDal.GetList(
        filter: null,
        include: q => q
            .Include(s => s.Trainer)
            .Include(s => s.Group)
            .Include(s => s.User)
    );

    var result = schedules.Select(s => new TrainerScheduleDetailedDto
    {
        Id = s.Id,
        TrainerName = s.Trainer?.Name,
        GroupName = s.Group?.Name,
        UserName = s.User?.Name,
        DayOfWeek = s.DayOfWeek.ToString(),
        StartTime = s.StartTime,
        EndTime = s.EndTime,
        Description = s.Description
    }).ToList();

    return result;
}


        public async Task<TrainerScheduleDto> GetByIdAsync(int id)
        {
            var schedule = await _trainerScheduleDal.GetByIdAsync(id);
            return _mapper.Map<TrainerScheduleDto>(schedule);
        }

        public async Task<List<TrainerScheduleDto>> GetByUserIdAsync(int userId)
        {
            var list = await _trainerScheduleDal.GetList(s => s.UserId == userId);
            return _mapper.Map<List<TrainerScheduleDto>>(list);
        }

        public async Task<List<TrainerScheduleDto>> GetByGroupIdAsync(int groupId)
        {
            var list = await _trainerScheduleDal.GetList(s => s.GroupId == groupId);
            return _mapper.Map<List<TrainerScheduleDto>>(list);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _trainerScheduleDal.GetByIdAsync(id);
            if (entity == null) return false;

            await _trainerScheduleDal.Delete(entity);
            return true;
        }

        public async Task<TrainerScheduleDto> UpdateAsync(TrainerScheduleUpdateDto dto)
        {
            var entity = await _trainerScheduleDal.GetByIdAsync(dto.Id);
            if (entity == null) return null;

           
            var startTime = new TimeOnly(dto.StartTime.Hour, dto.StartTime.Minute);
            var endTime = new TimeOnly(dto.EndTime.Hour, dto.EndTime.Minute);

          
            var conflicts = await _trainerScheduleDal.GetList(s =>
                s.Id != dto.Id &&
                s.TrainerId == entity.TrainerId &&
                s.DayOfWeek == dto.DayOfWeek &&
                (
                    (startTime >= s.StartTime && startTime < s.EndTime) ||
                    (endTime > s.StartTime && endTime <= s.EndTime) ||
                    (startTime <= s.StartTime && endTime >= s.EndTime)
                )
            );

            if (conflicts.Any())
                throw new InvalidOperationException("Bu vaxt aralığında artıq dərs təyin olunub.");

            entity.DayOfWeek = dto.DayOfWeek;
            entity.StartTime = startTime;
            entity.EndTime = endTime;
            entity.Description = dto.Description;

            await _trainerScheduleDal.Update(entity);
            return _mapper.Map<TrainerScheduleDto>(entity);
        }
      
        //TRAINER ucun ise asagidakini yazdim yeni trainer sirf kimlere ders deyirse onlari gore bilsin
        public async Task<List<TrainerScheduleDetailedDto>> GetSchedulesByTrainerIdentityIdAsync(string identityTrainerId)
        {
            var schedules = await _trainerScheduleDal.GetList(
                s => s.Trainer.IdentityTrainerId == identityTrainerId,
                include: q => q
                    .Include(s => s.Trainer)
                    .Include(s => s.Group)
                    .Include(s => s.User)
            );

            var result = schedules.Select(s => new TrainerScheduleDetailedDto
            {
                Id = s.Id,
                TrainerName = s.Trainer?.Name,
                GroupName = s.Group?.Name,
                UserName = s.User?.Name,
                DayOfWeek = s.DayOfWeek.ToString(), 
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Description = s.Description
            }).ToList();

            return result;
        }


        //Trainer-in id-ne gore ders kecdiyi qruplari getirir asagidaki funksiya

        public async Task<List<GroupGetDto>> GetGroupsByTrainerIdentityIdAsync(string trainerIdentityId)
        {
            var schedules = await _trainerScheduleDal.GetList(
                s => s.Trainer.IdentityTrainerId == trainerIdentityId && s.GroupId.HasValue,
                include: q => q.Include(s => s.Group)
                  .ThenInclude(g => g.Package) 
              .Include(s => s.Trainer)
               
            );

            var distinctGroups = schedules
                .Where(s => s.Group != null)
                .Select(s => s.Group)
                .DistinctBy(g => g.Id)
                .ToList();

            return distinctGroups.Select(g => new GroupGetDto
            {
                Id = g.Id,
                Name = g.Name,
                PackageId = g.PackageId,
                PackageName = g.Package?.PackageName
            }).ToList();
        }
        //bu funksiya ise trainerin ders kecdiyi qrupdaki usaqlari gormek ucundu
        public async Task<List<UserGetDto>> GetUsersInTrainerGroupAsync(string trainerIdentityId, int groupId)
        {
            bool teachesGroup = await _trainerScheduleDal.AnyAsync(
                s => s.Trainer.IdentityTrainerId == trainerIdentityId && s.GroupId == groupId
            );

            if (!teachesGroup)
                throw new InvalidOperationException("Trainer bu qrupa dərs demir.");


            var groupUsers = await _groupUserDal.GetList(
                gu => gu.GroupId == groupId,
                include: q => q.Include(gu => gu.User)
            );

            return groupUsers.Select(gu => new UserGetDto
            {
                Id = gu.User.Id,
                Name = gu.User.Name,
                Email = gu.User.Email

            }).ToList();
        }



    }
}







