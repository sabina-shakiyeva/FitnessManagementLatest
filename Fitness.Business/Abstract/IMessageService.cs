using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IMessageService
    {
        void SaveMessage(MessageDto message);
        List<MessageDto> GetMessages(string user);
        List<string> GetAllGuests();
    }
}
