using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Feedback
{
    public class UserReceivedFeedbackDto
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsPositive { get; set; }
        public DateTime GivenAt { get; set; }
        public int TrainerId { get; set; }
        public string TrainerFullName { get; set; } = string.Empty;
    }
}
