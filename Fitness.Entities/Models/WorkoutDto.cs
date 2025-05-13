namespace FitnessManagement.Dtos
{
    public class WorkoutDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime WorkoutDate { get; set; }
        public int TrainerId { get; set; }
    }
}
