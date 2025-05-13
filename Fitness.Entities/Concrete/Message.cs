using Fitness.Core.Abstraction;

namespace FitnessManagement.Entities
{
  

    public class Message:IEntity
    {
        public int Id { get; set; }
        public string SenderId { get; set; } 
        public string ReceiverId { get; set; } 
        public string Content { get; set; }  
        public DateTime SentAt { get; set; } 
    }
}
