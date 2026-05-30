using System.Globalization;
using Microsoft.AspNetCore.Components;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Components.Pages.CalendarDate;

public partial class CalendarDate : ComponentBase, IDisposable
{
    [Inject] public IMeetingCache MeetingCache { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    [Parameter] public string DateText { get; set; } = string.Empty;

    private DateOnly SelectedDate { get; set; }
    private bool IsLoading { get; set; } = true;
    private string ErrorMessage { get; set; } = string.Empty;
    private string DisplayDateText { get; set; } = string.Empty;
    private string EmptyMessage => $"No meetings scheduled for {DisplayDateText}.";
    private IReadOnlyList<CalendarMeetingDto> Meetings { get; set; } = Array.Empty<CalendarMeetingDto>();

    protected override void OnInitialized()
    {
        MeetingCache.Changed += HandleCacheChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!TryParseDate())
        {
            return;
        }

        IsLoading = !MeetingCache.IsInitialized;
        await MeetingCache.EnsureLoadedAsync();
        SyncFromCache();
    }

    public void Dispose()
    {
        MeetingCache.Changed -= HandleCacheChanged;
    }

    private bool TryParseDate()
    {
        if (!DateOnly.TryParseExact(DateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            ErrorMessage = "The selected date is invalid.";
            IsLoading = false;
            return false;
        }

        ErrorMessage = string.Empty;
        SelectedDate = parsedDate;
        DisplayDateText = SelectedDate.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);
        return true;
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
        Meetings = MeetingCache.GetMeetingsForMonth(SelectedDate.Year, SelectedDate.Month)
            .Where(meeting => DateOnly.FromDateTime(meeting.MeetingTime.Date) == SelectedDate)
            .OrderBy(meeting => meeting.MeetingTime)
            .ToList();

        IsLoading = !MeetingCache.IsInitialized;
    }

    private void GoBack()
    {
        NavigationManager.NavigateTo("/calendar");
    }
}
