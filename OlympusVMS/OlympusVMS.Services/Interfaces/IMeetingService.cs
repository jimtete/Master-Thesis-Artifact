using OlympusVMS.Utils.DTOs;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Services.Interfaces;

public interface IMeetingService
{
    Task<List<LoadTodayMeetingDto>> LoadTodayMeetings(CancellationToken token);
    Task<CreateOneMeetingResponse> RegisterGuest(RegisterGuestForm form);
}