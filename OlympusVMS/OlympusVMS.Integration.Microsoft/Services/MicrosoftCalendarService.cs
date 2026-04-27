using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using OlympusVMS.Integration.Microsoft.Configuration;
using System.Security.Claims;

namespace OlympusVMS.Integration.Microsoft.Services
{
    public class MicrosoftCalendarService : IMicrosoftCalendarService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly ILogger<MicrosoftCalendarService> _logger;
        private readonly MeetingRoomOptions _meetingRoomOptions;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MicrosoftCalendarService(
            GraphServiceClient graphClient,
            ILogger<MicrosoftCalendarService> logger,
            IOptions<MeetingRoomOptions> meetingRoomOptions,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor)
        {
            _graphClient = graphClient;
            _logger = logger;
            _meetingRoomOptions = meetingRoomOptions.Value;
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<MeetingDto>> GetMyMeetingsAsync(DateTime start, DateTime end)
        {
            try
            {
                var startString = start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                var endString = end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

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
                        Email = a.EmailAddress?.Address ?? string.Empty,
                        Name = a.EmailAddress?.Name ?? string.Empty,
                        Type = a.Type?.ToString() ?? "unknown"
                    }).ToList() ?? new List<AttendeeDto>()
                }).ToList() ?? new List<MeetingDto>();

                _logger.LogInformation("Successfully fetched {Count} personal meetings", meetings.Count);
                return meetings;
            }
            catch (Exception ex)    
            {
                _logger.LogError(ex, "Failed to fetch my meetings");
                throw;
            }
        }

        public async Task<List<MeetingDto>> GetConfiguredRoomMeetingsAsync(DateTime start, DateTime end)
        {
            var startUtc = start.ToUniversalTime();
            var endUtc = end.ToUniversalTime();

            var user = _httpContextAccessor.HttpContext?.User;
            var userKey = user?.FindFirstValue("oid")
                          ?? user?.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? "anonymous";

            var cacheKey = $"room-meetings:{userKey}:{startUtc:yyyyMMddHHmm}:{endUtc:yyyyMMddHHmm}";

            if (_memoryCache.TryGetValue(cacheKey, out List<MeetingDto>? cached) && cached is not null)
            {
                _logger.LogInformation("Room meetings cache hit. Count={Count}", cached.Count);
                return cached;
            }

            var roomEmails = (_meetingRoomOptions.Emails ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var startString = startUtc.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var endString = endUtc.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var allMeetings = new List<MeetingDto>();

            foreach (var roomEmail in roomEmails)
            {
                try
                {
                    var result = await _graphClient.Users[roomEmail].CalendarView.GetAsync(config =>
                    {
                        config.QueryParameters.StartDateTime = startString;
                        config.QueryParameters.EndDateTime = endString;
                        config.QueryParameters.Select = new[] { "id", "subject", "location", "start", "end", "attendees", "organizer" };
                        config.QueryParameters.Top = 999;
                        config.QueryParameters.Orderby = new[] { "start/dateTime asc" };
                    });

                    var roomMeetings = result?.Value?.Select(e => new MeetingDto
                    {
                        Id = e.Id,
                        Subject = e.Subject ?? "No Subject",
                        Location = e.Location?.DisplayName ?? roomEmail,
                        Start = e.Start?.DateTime != null ? DateTimeOffset.Parse(e.Start.DateTime) : null,
                        End = e.End?.DateTime != null ? DateTimeOffset.Parse(e.End.DateTime) : null,
                        Organizer = e.Organizer?.EmailAddress?.Address ?? "Unknown",
                        Attendees = e.Attendees?.Select(a => new AttendeeDto
                        {
                            Email = a.EmailAddress?.Address ?? string.Empty,
                            Name = a.EmailAddress?.Name ?? string.Empty,
                            Type = a.Type?.ToString() ?? "unknown"
                        }).ToList() ?? new List<AttendeeDto>()
                    }).ToList() ?? new List<MeetingDto>();

                    allMeetings.AddRange(roomMeetings);
                }
                catch (ODataError ex)
                {
                    _logger.LogWarning("Skipping room {RoomEmail}. Graph error: {Message}", roomEmail, ex.Error?.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Skipping room {RoomEmail} due to unexpected error.", roomEmail);
                }
            }

            var ordered = allMeetings
                .OrderBy(x => x.Start ?? DateTimeOffset.MaxValue)
                .ToList();

            _memoryCache.Set(
                cacheKey,
                ordered,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            _logger.LogInformation("Loaded {Count} meetings from {RoomCount} configured rooms", ordered.Count, roomEmails.Count);

            return ordered;
        }

        public Task<List<RoomDto>> GetAllMeetingRoomsAsync()
        {
            var rooms = (_meetingRoomOptions.Emails ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(email => new RoomDto
                {
                    Id = email,
                    Email = email,
                    DisplayName = email.Split('@')[0]
                })
                .OrderBy(x => x.DisplayName)
                .ToList();

            _logger.LogInformation("Loaded {Count} configured meeting rooms", rooms.Count);
            return Task.FromResult(rooms);
        }
    }
}