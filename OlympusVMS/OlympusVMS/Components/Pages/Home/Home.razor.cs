using Microsoft.AspNetCore.Components;
using OlympusVMS.Integration.Microsoft;
using OlympusVMS.Integration.Microsoft.Services;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    private const string CompanyDomain = "@capital-four.com";

    [Inject] public IMicrosoftCalendarService CalendarService { get; set; } = default!;
    [Inject] public IMeetingService MeetingService { get; set; } = default!;

    private readonly CancellationTokenSource _cts = new();

    private string TodayText => DateTime.Today.ToShortDateString();
    private bool IsLoading { get; set; } = true;
    private string ErrorMessage { get; set; } = string.Empty;
    private IReadOnlyList<LoadTodayMeetingDto> Meetings { get; set; } = Array.Empty<LoadTodayMeetingDto>();

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var start = DateTime.Now;
            var end = DateTime.Now.AddDays(7);

            var roomMeetings = await CalendarService.GetConfiguredRoomMeetingsAsync(start, end);

            var forms = BuildGuestRegistrationForms(roomMeetings);
            var inserted = await MeetingService.RegisterGuestsFromFormsAsync(forms, _cts.Token);

            Meetings = await MeetingService.LoadTodayMeetings(_cts.Token);
            ErrorMessage = $"Loaded {Meetings.Count} upcoming guest registrations. Added {inserted} new from room attendees.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load guest registrations: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    private static List<RegisterGuestForm> BuildGuestRegistrationForms(IEnumerable<MeetingDto> roomMeetings)
    {
        var result = new List<RegisterGuestForm>();

        foreach (var meeting in roomMeetings)
        {
            if (!meeting.Start.HasValue)
            {
                continue;
            }

            int? durationHours = null;
            if (meeting.Start.HasValue && meeting.End.HasValue)
            {
                var totalHours = (meeting.End.Value - meeting.Start.Value).TotalHours;
                if (totalHours > 0)
                {
                    durationHours = Math.Max(1, (int)Math.Ceiling(totalHours));
                }
            }

            foreach (var attendee in meeting.Attendees)
            {
                if (string.IsNullOrWhiteSpace(attendee.Email))
                {
                    continue;
                }

                if (attendee.Email.EndsWith(CompanyDomain, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var (firstName, lastName) = SplitName(attendee.Name, attendee.Email);

                result.Add(new RegisterGuestForm
                {
                    FirstName = firstName,
                    LastName = lastName,
                    EmailAddress = attendee.Email,
                    RegisteredBy = "AutoGraphSync",
                    MeetingTime = meeting.Start.Value,
                    MeetingDurationInHours = durationHours
                });
            }
        }

        return result;
    }

    private static (string FirstName, string LastName) SplitName(string? displayName, string email)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            var parts = displayName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length >= 2)
            {
                return (parts[0], string.Join(" ", parts.Skip(1)));
            }

            if (parts.Length == 1)
            {
                return (parts[0], "Unknown");
            }
        }

        var localPart = email.Split('@')[0];
        var localTokens = localPart
            .Split(new[] { '.', '_', '-', '+' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (localTokens.Length >= 2)
        {
            return (Cap(localTokens[0]), Cap(localTokens[1]));
        }

        return (Cap(localPart), "Unknown");
    }

    private static string Cap(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Unknown";
        }

        var trimmed = value.Trim();
        return trimmed.Length == 1
            ? trimmed.ToUpperInvariant()
            : char.ToUpperInvariant(trimmed[0]) + trimmed[1..].ToLowerInvariant();
    }
}