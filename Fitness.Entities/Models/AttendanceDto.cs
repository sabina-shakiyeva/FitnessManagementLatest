namespace FitnessManagement.Dtos
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsPresent { get; set; }
        public DateTime AttendanceDate { get; set; }

    }
}
