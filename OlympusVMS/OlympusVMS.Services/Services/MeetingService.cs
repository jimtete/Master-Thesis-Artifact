using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Services.Services;

public class MeetingService : IMeetingService
{
    public async Task<List<LoadTodayMeetingDto>> LoadTodayMeetings(CancellationToken token = default)
    {
        await Task.Delay(500, token);

        return new List<LoadTodayMeetingDto>
        {
            new("Anna", "Berlin", DateTimeOffset.Now.AddHours(1)),
            new("Dimitrios", "Papadopoulos", DateTimeOffset.Now.AddHours(2)),
            new("Maria", "Khan", DateTimeOffset.Now.AddHours(3)),
        };
    }
}