using Microsoft.AspNetCore.Components;
// using OlympusVMS.Services.Interfaces; // Muted for the test
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    // 1. MUTE THE INJECTION: Stop asking for the missing database service
    // [Inject] public IMeetingService MeetingService { get; set; } = default;

    private readonly CancellationTokenSource _cts = new();

    private static string TodayText => DateTime.Today.ToShortDateString();
    private static bool IsLoading { get; set; } = true;
    private static string ErrorMessage { get; set; } = string.Empty;
    private static IReadOnlyList<LoadTodayMeetingDto> Meetings { get; set; } = Array.Empty<LoadTodayMeetingDto>();

    protected override async Task OnInitializedAsync() // Removed 'async' since we aren't awaiting anything now, but keeping it async to match your signature is fine
    {
        IsLoading = true;

        // 2. MUTE THE DATABASE CALL: 
        // Meetings = await MeetingService.LoadTodayMeetings(_cts.Token);

        // Let it just be empty for the sake of starting the app
        Meetings = new List<LoadTodayMeetingDto>();

        if (Meetings is null || Meetings.Count == 0)
        {
            ErrorMessage = "Database bypassed for Microsoft Graph test.";
        }

        IsLoading = false;
        await Task.CompletedTask; // Just to satisfy the async signature
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}