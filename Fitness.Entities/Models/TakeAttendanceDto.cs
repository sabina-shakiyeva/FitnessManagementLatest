using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class TakeAttendanceDto
    {
        public int UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
