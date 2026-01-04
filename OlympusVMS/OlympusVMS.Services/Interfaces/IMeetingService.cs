using OlympusVMS.Utils.DTOs.Meeting;
using OlympusVMS.Utils.Models;

namespace OlympusVMS.Services.Interfaces;

public interface IMeetingService
{
    Task<List<LoadTodayMeetingDto>> LoadTodayMeetings(CancellationToken token);
}