using Fitness.Core.Abstraction;
using System.ComponentModel.DataAnnotations;

namespace FitnessManagement.Entities
{
    public class Attendance: IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; }
        public AttendanceStatus  Status{ get; set; }
    }
    public enum AttendanceStatus
    {
        Absent = 0,
        Present = 1
    }
}
