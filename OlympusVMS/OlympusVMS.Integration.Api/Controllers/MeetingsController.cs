using Microsoft.AspNetCore.Mvc;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Integration.Api.Controllers;

[ApiController]
[Route(("s2s-api/v1/[controller]"))]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingService _meetingService;
    
    public MeetingsController(IMeetingService meetingService)
    {
        _meetingService = meetingService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOne([FromBody] CreateOneMeetingRequest request)
    {
        var form = new RegisterGuestForm
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MeetingTime = request.MeetingTime,
            EmailAddress = request.EmailAddress,
            MeetingDurationInHours = request.MeetingDurationInHours,
            RegisteredBy = request.RegisteredBy
        };
        
        var response = await _meetingService.RegisterGuest(form);
        
        return Ok(response);
    }
}