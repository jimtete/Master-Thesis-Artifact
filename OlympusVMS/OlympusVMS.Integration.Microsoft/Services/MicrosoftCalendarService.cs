
using Microsoft.Graph;

namespace OlympusVMS.Integration.Microsoft.Services
{
    public class MicrosoftCalendarService : IMicrosoftCalendarService
    {
        private readonly GraphServiceClient _graphClient;

        public MicrosoftCalendarService(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<List<MeetingDto>> GetUserMeetingsAsync(string userEmail)
        {
            var result = await _graphClient.Users[userEmail].Events.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select = new[] { "subject", "start", "end", "location" };
                requestConfiguration.QueryParameters.Top = 10;
                requestConfiguration.QueryParameters.Orderby = new[] { "start/dateTime DESC" };
            });

            return result?.Value?.Select(e => new MeetingDto
            {
                Id = e.Id,
                Subject = e.Subject,
                Start = e.Start?.DateTime != null ? DateTimeOffset.Parse(e.Start.DateTime) : null,
                End = e.End?.DateTime != null ? DateTimeOffset.Parse(e.End?.DateTime) : null,
                Location = e.Location?.DisplayName ?? string.Empty
            }).ToList() ?? new List<MeetingDto>();
        }
    }
}
