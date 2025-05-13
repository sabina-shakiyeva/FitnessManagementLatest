using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class MessageService : IMessageService
{
    private readonly ConcurrentDictionary<string, List<MessageDto>> _messages = new();
    private readonly HashSet<string> _guestIds = new();

    public void SaveMessage(MessageDto message)
    {
        _guestIds.Add(message.Sender == "admin" ? message.Receiver : message.Sender);

        var key = message.Sender == "admin" ? message.Receiver : message.Sender;

        if (!_messages.ContainsKey(key))
            _messages[key] = new List<MessageDto>();

        _messages[key].Add(message);
    }

    public List<MessageDto> GetMessages(string user)
    {
        _messages.TryGetValue(user, out var msgs);
        return msgs ?? new List<MessageDto>();
    }

    public List<string> GetAllGuests()
    {
        return _guestIds.ToList();
    }
}
