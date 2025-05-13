using AutoMapper;
using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Models;
using FitnessManagement.Entities;
using FitnessManagement.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class AttendanceService:IAttendanceService
    {
        private readonly IAttendanceDal _attendanceDal;
        private readonly IFileService _fileService;
        private readonly IUserDal _userDal;
        private readonly IMapper _mapper;

        public AttendanceService(IAttendanceDal attendanceDal, IFileService fileService, IUserDal userDal, IMapper mapper)
        {
            _attendanceDal = attendanceDal;
            _fileService = fileService;
            _userDal = userDal;
            _mapper = mapper;
        }

        public async Task<List<AttendanceViewDto>> GetAttendanceByUserIdAsync(int userId)
        {
            var attendances = await _attendanceDal.GetList(a => a.UserId == userId);

            return attendances
                .Select(a => new AttendanceViewDto
                {
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status
                }).ToList();
        }

        //public async Task TakeAttendanceAsync(TakeAttendanceDto dto)
        //{
        //    var attendance = _mapper.Map<Attendance>(dto);
        //    attendance.AttendanceDate = dto.AttendanceDate.Date;
        //    await _attendanceDal.Add(attendance);
        //}


        public async Task TakeAttendanceAsync(TakeAttendanceDto dto)

        {

            var existingAttendance = await _attendanceDal.GetList(a => a.UserId == dto.UserId && a.AttendanceDate.Date == dto.AttendanceDate.Date);

            if (existingAttendance.Any())

            {

                var attendance = existingAttendance.First();

                attendance.Status = dto.Status;

                await _attendanceDal.Update(attendance);

            }
            if (dto.Status == AttendanceStatus.Present)
            {
                var user = await _userDal.GetByIdAsync(dto.UserId);
                user.Point += 1;
                await _userDal.Update(user);
            }

            else

            {

                var attendance = _mapper.Map<Attendance>(dto);

                attendance.AttendanceDate = dto.AttendanceDate.Date;

                await _attendanceDal.Add(attendance);
                


            }
        

        }


        public async Task<List<AttendanceGetDto>> GetAttendanceList()
        {
            var users = await _userDal.GetList(
                filter: u => u.IsActive,
                include: q => q.Include(u => u.Package)
            );

            return users.Select(user => new AttendanceGetDto
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                ImageUrl = user.ImageUrl != null ? _fileService.GetFileUrl(user.ImageUrl) : null,
                PackageName = user.Package?.PackageName
            }).ToList();
        }



    }
}
