using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OlympusVMS.Integration.Microsoft.Services
{
    public interface IMicrosoftCalendarService
    {
        Task<List<MeetingDto>> GetMyMeetingsAsync(DateTime start, DateTime end);
        Task<List<MeetingDto>> GetConfiguredRoomMeetingsAsync(DateTime start, DateTime end);
        Task<List<RoomDto>> GetAllMeetingRoomsAsync();
    }
}
