using Microsoft.AspNetCore.Components;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] public IMeetingCache MeetingCache { get; set; } = default!;

    private readonly CancellationTokenSource _cts = new();

    private string TodayText => DateTime.Today.ToShortDateString();
    private bool IsLoading { get; set; } = true;
    private string ErrorMessage { get; set; } = "No meetings scheduled for today.";
    private IReadOnlyList<CalendarMeetingDto> Meetings { get; set; } = Array.Empty<CalendarMeetingDto>();

    protected override async Task OnInitializedAsync()
    {
        MeetingCache.Changed += HandleCacheChanged;
        IsLoading = !MeetingCache.IsInitialized;

        await MeetingCache.EnsureLoadedAsync(_cts.Token);
        SyncFromCache();
    }

    public void Dispose()
    {
        MeetingCache.Changed -= HandleCacheChanged;
        _cts.Cancel();
        _cts.Dispose();
    }

    private void HandleCacheChanged()
    {
        _ = InvokeAsync(() =>
        {
            SyncFromCache();
            StateHasChanged();
        });
    }

    private void SyncFromCache()
    {
        Meetings = MeetingCache.GetTodayMeetings();
        IsLoading = !MeetingCache.IsInitialized;
    }
}
