using Microsoft.AspNetCore.Components;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _cts = new();

    private static string TodayText => DateTime.Today.ToShortDateString();
    private static bool IsLoading { get; set; } = true;
    private static string ErrorMessage { get; set; } = string.Empty;
    private static IReadOnlyList<LoadTodayMeetingDto> Meetings { get; set; } = Array.Empty<LoadTodayMeetingDto>();

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        Meetings = new List<LoadTodayMeetingDto>();
        ErrorMessage = "Database bypassed for Microsoft Graph testing.";
        IsLoading = false;
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}