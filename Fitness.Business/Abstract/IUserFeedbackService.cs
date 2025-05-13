using Fitness.Entities.Models.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IUserFeedbackService
    {
        Task<List<UserReceivedFeedbackDto>> GetMyFeedbacksAsync();
    }
}
