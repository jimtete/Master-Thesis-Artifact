using Microsoft.Graph;
// Make sure you have your DTO namespace here, e.g., using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Integration.Microsoft.Services
{
    public class MicrosoftCalendarService : IMicrosoftCalendarService
    {
        private readonly GraphServiceClient _graphClient;

        public MicrosoftCalendarService(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<List<MeetingDto>> GetUserMeetingsAsync(DateTime start, DateTime end)
        {
            // 1. Convert C# DateTimes to UTC ISO 8601 strings so Microsoft Graph understands them
            string startString = start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            string endString = end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            // 2. Use CalendarView to fetch specific occurrences within the time window
            var result = await _graphClient.Me.CalendarView.GetAsync(config =>
            {
                // Apply the date boundaries
                config.QueryParameters.StartDateTime = startString;
                config.QueryParameters.EndDateTime = endString;

                // Requesting the specific fields you need for the thesis
                config.QueryParameters.Select = new[] { "id", "subject", "location", "start", "end" };

                // Increase the Top count to 999 to fetch everything in the given window
                config.QueryParameters.Top = 999;

                // Order chronologically (oldest to newest)
                config.QueryParameters.Orderby = new[] { "start/dateTime asc" };
            });

            return result?.Value?.Select(e => new MeetingDto
            {
                Id = e.Id,
                Subject = e.Subject,
                // Access the DisplayName within the Location object
                Location = e.Location?.DisplayName ?? "No Location Set",
                Start = e.Start?.DateTime != null ? DateTimeOffset.Parse(e.Start.DateTime) : null,
                End = e.End?.DateTime != null ? DateTimeOffset.Parse(e.End.DateTime) : null
            }).ToList() ?? new List<MeetingDto>();
        }
    }
}