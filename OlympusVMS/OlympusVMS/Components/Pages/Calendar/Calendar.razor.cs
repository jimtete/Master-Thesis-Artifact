using System.Globalization;
using Microsoft.AspNetCore.Components;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Components.Pages.Calendar;

public partial class Calendar : ComponentBase, IDisposable
{
    [Inject]
    public IMeetingCache MeetingCache { get; set; } = default!;
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    protected static readonly string[] DayHeaders = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    protected int Year { get; set; } = DateTime.Today.Year;
    protected int Month { get; set; } = DateTime.Today.Month;
    protected List<DateOnly?> Cells { get; set; } = new();
    protected int Weeks { get; set; }
    protected bool IsLoading { get; set; }
    protected Dictionary<DateOnly, List<CalendarMeetingDto>> MeetingsByDate { get; set; } = new();

    protected string CurrentMonthTitle =>
        new DateTime(Year, Month, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);

    protected override async Task OnInitializedAsync()
    {
        MeetingCache.Changed += HandleCacheChanged;
        BuildCalendar();
        SyncMeetingsFromCache();

        if (!MeetingCache.IsInitialized)
        {
            IsLoading = true;
            await MeetingCache.EnsureLoadedAsync();
        }

        SyncMeetingsFromCache();
    }

    protected void BuildCalendar()
    {
        var first = new DateOnly(Year, Month, 1);
        var daysInMonth = DateTime.DaysInMonth(Year, Month);
        var startOffset = (((int)first.DayOfWeek) + 6) % 7;
        var totalCells = startOffset + daysInMonth;

        Weeks = (int)Math.Ceiling(totalCells / 7.0);
        Weeks = Math.Max(5, Weeks);
        Weeks = Math.Min(6, Weeks);

        Cells = new List<DateOnly?>(Weeks * 7);

        for (var i = 0; i < Weeks * 7; i++)
        {
            var dayNumber = i - startOffset + 1;
            Cells.Add(dayNumber < 1 || dayNumber > daysInMonth
                ? null
                : new DateOnly(Year, Month, dayNumber));
        }
    }

    protected void PrevMonth()
    {
        var dt = new DateTime(Year, Month, 1).AddMonths(-1);
        Year = dt.Year;
        Month = dt.Month;
        RefreshCalendarView();
    }

    protected void NextMonth()
    {
        var dt = new DateTime(Year, Month, 1).AddMonths(1);
        Year = dt.Year;
        Month = dt.Month;
        RefreshCalendarView();
    }

    protected void PrevYear()
    {
        Year--;
        RefreshCalendarView();
    }

    protected void NextYear()
    {
        Year++;
        RefreshCalendarView();
    }

    protected void GoToday()
    {
        Year = DateTime.Today.Year;
        Month = DateTime.Today.Month;
        RefreshCalendarView();
    }

    protected string GetCellClass(DateOnly? date)
    {
        if (date is null)
        {
            return "cal-empty";
        }

        var isToday = date.Value == DateOnly.FromDateTime(DateTime.Today);
        var isWeekend = date.Value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        var hasMeetings = GetMeetings(date.Value).Count > 0;

        return $"{(isWeekend ? "cal-weekend " : string.Empty)}{(isToday ? "cal-today-cell " : string.Empty)}{(hasMeetings ? "cal-has-meetings" : string.Empty)}".Trim();
    }

    protected IReadOnlyList<CalendarMeetingDto> GetMeetings(DateOnly date)
    {
        return MeetingsByDate.TryGetValue(date, out var meetings)
            ? meetings
            : Array.Empty<CalendarMeetingDto>();
    }

    protected string GetGuestDisplayName(CalendarMeetingDto meeting)
    {
        return string.Join(" ", new[] { meeting.FirstName, meeting.LastName }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    protected string GetMeetingPreview(CalendarMeetingDto meeting)
    {
        return $"{meeting.MeetingTime:HH:mm} {GetGuestDisplayName(meeting)}";
    }

    protected void OpenDayPage(DateOnly date)
    {
        NavigationManager.NavigateTo($"/calendar/date/{date:yyyy-MM-dd}");
    }

    public void Dispose()
    {
        MeetingCache.Changed -= HandleCacheChanged;
    }

    private void HandleCacheChanged()
    {
        _ = InvokeAsync(() =>
        {
            SyncMeetingsFromCache();
            StateHasChanged();
        });
    }

    private void RefreshCalendarView()
    {
        BuildCalendar();
        SyncMeetingsFromCache();
    }

    private void SyncMeetingsFromCache()
    {
        var meetings = MeetingCache.GetMeetingsForMonth(Year, Month);

        MeetingsByDate = meetings
            .GroupBy(meeting => DateOnly.FromDateTime(meeting.MeetingTime.Date))
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(meeting => meeting.MeetingTime).ToList());

        IsLoading = !MeetingCache.IsInitialized;
    }
}
