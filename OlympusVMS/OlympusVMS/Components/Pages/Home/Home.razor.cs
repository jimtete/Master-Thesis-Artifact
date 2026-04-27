using Microsoft.AspNetCore.Components;
using OlympusVMS.Integration.Microsoft.Services;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] public IMicrosoftCalendarService CalendarService { get; set; } = default!;

    private readonly CancellationTokenSource _cts = new();

    private static string TodayText => DateTime.Today.ToShortDateString();
    private static bool IsLoading { get; set; } = true;
    private static string ErrorMessage { get; set; } = string.Empty;
    private static IReadOnlyList<LoadTodayMeetingDto> Meetings { get; set; } = Array.Empty<LoadTodayMeetingDto>();

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        Meetings = new List<LoadTodayMeetingDto>();

        try
        {
            var start = DateTime.Now;
            var end = DateTime.Now.AddDays(7);

            await CalendarService.GetConfiguredRoomMeetingsAsync(start, end);
            ErrorMessage = "Authentication successful. Room meetings cache preloaded.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Preload failed: {ex.Message}";
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
}