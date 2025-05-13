using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class UserEquipmentUsageService : IUserEquipmentUsageService
    {
        private readonly IUserEquipmentUsageDal _usageDal;
        private readonly IUserDal _userDal;
        private readonly IEquipmentDal _equipmentDal;

        public UserEquipmentUsageService(IUserEquipmentUsageDal usageDal, IUserDal userDal, IEquipmentDal equipmentDal)
        {
            _usageDal = usageDal;
            _userDal = userDal;
            _equipmentDal = equipmentDal;
        }

        private int CalculateUserPoints(EquipmentUsageStatDto usage)
        {
            int point = 0;

            if (usage.DurationInMinutes.HasValue)
                point += usage.DurationInMinutes.Value * 2;

            if (usage.Repetition.HasValue)
                point += usage.Repetition.Value;

            return point;
        }
        public async Task AddAsync(EquipmentUsageStatDto usage)
        {
            var userUsage = new UserEquipmentUsage
            {
                UserId = usage.UserId,
                EquipmentId = usage.EquipmentId,
                DurationInMinutes = usage.DurationInMinutes,
                Repetition = usage.Repetition,
                Date = usage.Date ?? DateTime.Now
            };

            await _usageDal.Add(userUsage);

            var user = await _userDal.Get(u => u.Id == usage.UserId);
            if (user != null)
            {
                int points = CalculateUserPoints(usage);
                user.Point += points;

                await _userDal.Update(user);
            }
        }
        public async Task<List<EquipmentUsageGetDto>> GetAllEquipmentUsages()
        {
            var usages = await _usageDal.GetList();

            var usageDtos = new List<EquipmentUsageGetDto>();

            foreach (var usage in usages)
            {
                var equipment = await _equipmentDal.Get(e => e.Id == usage.EquipmentId);
                var user = await _userDal.Get(u => u.Id == usage.UserId);

                usageDtos.Add(new EquipmentUsageGetDto
                {
                    UserId = usage.UserId,
                    EquipmentId = usage.EquipmentId,
                    DurationInMinutes = usage.DurationInMinutes,
                    Repetition = usage.Repetition,
                    Date = usage.Date,
                    EquipmentName = equipment?.Name,
                    UserName = user?.Name
                });
            }

            return usageDtos;
        }

        //bura bax
        public async Task<List<UserEquipmentUsage>> GetAllByUserIdAsync(int userId)
        {
            return await _usageDal.GetList(u => u.UserId == userId);
        }
        public async Task<List<EquipmentUsageStatDto>> GetUsageStatsByUserIdAsync(int userId)
        {
            var usages = await _usageDal.GetList(u => u.UserId == userId);

            var grouped = usages
                .Where(u => u.UserId != 0)
                .GroupBy(u => u.EquipmentId)
                .Select(g => new EquipmentUsageStatDto
                {
                    EquipmentId = g.Key,
                    DurationInMinutes = g.Sum(x => x.DurationInMinutes) ?? 0,
                    Repetition = g.Sum(x => x.Repetition) ?? 0,
                    UserId = g.FirstOrDefault()?.UserId ?? default(int)
                }).ToList();

            foreach (var stat in grouped)
            {
                var equipment = await _equipmentDal.Get(e => e.Id == stat.EquipmentId);
                stat.EquipmentName = equipment?.Name;
            }

            return grouped;
        }
    

}
}

