using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlympusVMS.Integration.Microsoft;

namespace OlympusVMS.Integration.Microsoft.Services
{
    public interface IMicrosoftCalendarService
    {
        Task<List<MeetingDto>> GetMyMeetingsAsync(DateTime start, DateTime end);
    }
}
