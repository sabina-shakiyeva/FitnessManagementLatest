using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Feedback
{
    public class TrainerToUserFeedbackCreateDto
    {
        
        public int UserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsPositive { get; set; }
    }
}
