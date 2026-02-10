namespace OlympusVMS.Integration.Microsoft.Services
{
    public interface IMicrosoftCalendarService
    {
        Task<List<MeetingDto>> GetUserMeetingsAsync(string userEmail);
    }
}
