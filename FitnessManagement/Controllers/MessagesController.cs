using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Fitness.Business.Abstract;
using FitnessManagement.Dtos;

[Route("api/[controller]")]
[ApiController]

public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _chatHubContext;

    public MessageController(IMessageService messageService, IHubContext<ChatHub> chatHubContext)
    {
        _messageService = messageService;
        _chatHubContext = chatHubContext;
    }

 
    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
      
        var message = new MessageDto
        {
            Sender = "admin",
            Receiver = dto.UserId,
            Text = dto.Message
        };

        _messageService.SaveMessage(message);

        await _chatHubContext.Clients.Group(dto.UserId).SendAsync("ReceiveMessage", message);

        return Ok("Mesaj uğurla göndərildi.");
    }

    
    [HttpGet("{userId}")]
    public IActionResult GetMessages(string userId)
    {
        var messages = _messageService.GetMessages(userId);
        return Ok(messages);
    }

   
    [HttpGet("guests")]
    public IActionResult GetAllGuests()
    {
        var guests = _messageService.GetAllGuests();
        return Ok(guests);
    }
}


public class SendMessageDto
{
    public string UserId { get; set; }
    public string Message { get; set; }
}
