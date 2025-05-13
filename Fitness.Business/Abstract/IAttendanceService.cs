using Fitness.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IAttendanceService
    {
        Task TakeAttendanceAsync(TakeAttendanceDto dto);
        Task<List<AttendanceViewDto>> GetAttendanceByUserIdAsync(int userId);
        Task<List<AttendanceGetDto>> GetAttendanceList();
    }
}
