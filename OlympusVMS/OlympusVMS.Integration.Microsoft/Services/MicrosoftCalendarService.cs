using Microsoft.Graph;
using Microsoft.Graph.Models;
using OlympusVMS.Integration.Microsoft;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlympusVMS.Integration.Microsoft.Services
{
    public class MicrosoftCalendarService : IMicrosoftCalendarService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly ILogger<MicrosoftCalendarService> _logger;

        public MicrosoftCalendarService(GraphServiceClient graphClient, ILogger<MicrosoftCalendarService> logger)
        {
            _graphClient = graphClient;
            _logger = logger;
        }

        /// <summary>
        /// Fetch meetings from YOUR OWN calendar (rooms you're a delegate for appear here)
        /// </summary>
        public async Task<List<MeetingDto>> GetUserMeetingsAsync(DateTime start, DateTime end)
        {
            try
            {
                string startString = start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                string endString = end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                var result = await _graphClient.Me.CalendarView.GetAsync(config =>
                {
                    config.QueryParameters.StartDateTime = startString;
                    config.QueryParameters.EndDateTime = endString;
                    config.QueryParameters.Select = new[] { "id", "subject", "location", "start", "end", "attendees", "organizer" };
                    config.QueryParameters.Top = 999;
                    config.QueryParameters.Orderby = new[] { "start/dateTime asc" };
                });

                return result?.Value?.Select(e => new MeetingDto
                {
                    Id = e.Id,
                    Subject = e.Subject ?? "No Subject",
                    Location = e.Location?.DisplayName ?? "No Location",
                    Start = e.Start?.DateTime != null ? DateTimeOffset.Parse(e.Start.DateTime) : null,
                    End = e.End?.DateTime != null ? DateTimeOffset.Parse(e.End.DateTime) : null,
                    Organizer = e.Organizer?.EmailAddress?.Address ?? "Unknown",
                    Attendees = e.Attendees?.Select(a => new AttendeeDto
                    {
                        Email = a.EmailAddress?.Address ?? "",
                        Name = a.EmailAddress?.Name ?? "",
                        Type = a.Type?.ToString() ?? "unknown"
                    }).ToList() ?? new List<AttendeeDto>()
                }).ToList() ?? new List<MeetingDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user meetings");
                throw;
            }
        }

        /// <summary>
        /// Fetch meetings from a specific room mailbox you have delegate access to
        /// </summary>
        public async Task<List<MeetingDto>> GetRoomMeetingsAsync(string roomEmail, DateTime start, DateTime end)
        {
            try
            {
                string startString = start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                string endString = end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                var result = await _graphClient.Users[roomEmail].CalendarView.GetAsync(config =>
                {
                    config.QueryParameters.StartDateTime = startString;
                    config.QueryParameters.EndDateTime = endString;
                    config.QueryParameters.Select = new[] { "id", "subject", "location", "start", "end", "attendees", "organizer" };
                    config.QueryParameters.Top = 999;
                    config.QueryParameters.Orderby = new[] { "start/dateTime asc" };
                });

                return result?.Value?.Select(e => new MeetingDto
                {
                    Id = e.Id,
                    Subject = e.Subject ?? "No Subject",
                    Location = e.Location?.DisplayName ?? "No Location",
                    Start = e.Start?.DateTime != null ? DateTimeOffset.Parse(e.Start.DateTime) : null,
                    End = e.End?.DateTime != null ? DateTimeOffset.Parse(e.End.DateTime) : null,
                    Organizer = e.Organizer?.EmailAddress?.Address ?? "Unknown",
                    Attendees = e.Attendees?.Select(a => new AttendeeDto
                    {
                        Email = a.EmailAddress?.Address ?? "",
                        Name = a.EmailAddress?.Name ?? "",
                        Type = a.Type?.ToString() ?? "unknown"
                    }).ToList() ?? new List<AttendeeDto>()
                }).ToList() ?? new List<MeetingDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch meetings for room {RoomEmail}", roomEmail);
                throw;
            }
        }

        public async Task<List<RoomDto>> GetAvailableRoomsAsync()
        {
            // This won't work with delegated permissions for all org rooms
            // Instead, return empty or the rooms you know you have access to
            _logger.LogWarning("GetAvailableRoomsAsync requires app-wide permissions. Not available with delegated access.");
            return new List<RoomDto>();
        }

        public async Task<List<MeetingDto>> GetMyMeetingsAsync(DateTime start, DateTime end)
        {
            try
            {
                string startString = start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                string endString = end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                var result = await _graphClient.Me.CalendarView.GetAsync(config =>
                {
                    config.QueryParameters.StartDateTime = startString;
                    config.QueryParameters.EndDateTime = endString;
                    config.QueryParameters.Select = new[] { "id", "subject", "location", "start", "end", "attendees", "organizer" };
                    config.QueryParameters.Top = 999;
                    config.QueryParameters.Orderby = new[] { "start/dateTime asc" };
                });

                var meetings = result?.Value?.Select(e => new MeetingDto
                {
                    Id = e.Id,
                    Subject = e.Subject ?? "No Subject",
                    Location = e.Location?.DisplayName ?? "No Location",
                    Start = e.Start?.DateTime != null ? DateTimeOffset.Parse(e.Start.DateTime) : null,
                    End = e.End?.DateTime != null ? DateTimeOffset.Parse(e.End.DateTime) : null,
                    Organizer = e.Organizer?.EmailAddress?.Address ?? "Unknown",
                    Attendees = e.Attendees?.Select(a => new AttendeeDto
                    {
                        Email = a.EmailAddress?.Address ?? "",
                        Name = a.EmailAddress?.Name ?? "",
                        Type = a.Type?.ToString() ?? "unknown"
                    }).ToList() ?? new List<AttendeeDto>()
                }).ToList() ?? new List<MeetingDto>();

                _logger.LogInformation($"Successfully fetched {meetings.Count} meetings");
                return meetings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch my meetings");
                throw;
            }
        }
    }
}