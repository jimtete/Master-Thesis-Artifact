namespace OlympusVMS.Integration.Microsoft.Services
{
    public interface IMicrosoftCalendarService
    {
        Task<List<MeetingDto>> GetUserMeetingsAsync(DateTime start, DateTime end);
        Task<Dictionary<string, List<MeetingDto>> GetColleaguesMeetingsAsync(DateTime start, DateTime end);
    }
}
