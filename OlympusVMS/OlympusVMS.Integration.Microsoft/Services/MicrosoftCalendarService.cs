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
        public async Task<Dictionary<string, List<MeetingDto>>> GetColleaguesMeetingsAsync(DateTime start, DateTime end)
        {
            var accessibleMeetings = new Dictionary<string, List<MeetingDto>>();

            // 1. Fetch a batch of users from your organization
            var usersResponse = await _graphClient.Users.GetAsync(config =>
            {
                config.QueryParameters.Select = new[] { "id", "userPrincipalName", "displayName" };
                config.QueryParameters.Top = 10; // Let's test with 10 people first
            });

            var users = usersResponse?.Value ?? new List<Microsoft.Graph.Models.User>();

            string startString = start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            string endString = end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            // 2. Loop through each user and try to fetch their calendar
            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.UserPrincipalName)) continue;

                try
                {
                    // Accessing another user's calendar using the Delegated Shared scope
                    var eventsResponse = await _graphClient.Users[user.UserPrincipalName].CalendarView.GetAsync(config =>
                    {
                        config.QueryParameters.StartDateTime = startString;
                        config.QueryParameters.EndDateTime = endString;
                        config.QueryParameters.Select = new[] { "id", "subject", "location", "start", "end" };
                        config.QueryParameters.Top = 3; // Only grab the top 3 results per person
                        config.QueryParameters.Orderby = new[] { "start/dateTime asc" };
                    });

                    var meetings = eventsResponse?.Value?.Select(e => new MeetingDto
                    {
                        Id = e.Id,
                        Subject = e.Subject,
                        Location = e.Location?.DisplayName ?? "No Location Set",
                        Start = e.Start?.DateTime != null ? DateTimeOffset.Parse(e.Start.DateTime) : null,
                        End = e.End?.DateTime != null ? DateTimeOffset.Parse(e.End.DateTime) : null
                    }).ToList() ?? new List<MeetingDto>();

                    // If it succeeds, add them to our dictionary
                    accessibleMeetings.Add(user.UserPrincipalName, meetings);
                }
                catch (Microsoft.Graph.Models.ODataErrors.ODataError)
                {
                    // A 403 Forbidden means you don't have permission to see this specific person's calendar.
                    // We safely swallow the error and move to the next colleague.
                }
            }

            return accessibleMeetings;
        }
    }
}