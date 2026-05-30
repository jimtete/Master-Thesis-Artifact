using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Services.Interfaces;

public interface IMeetingCache
{
    event Action? Changed;

    bool IsInitialized { get; }
    bool IsRefreshing { get; }

    Task EnsureLoadedAsync(CancellationToken token = default);
    IReadOnlyList<CalendarMeetingDto> GetTodayMeetings();
    IReadOnlyList<CalendarMeetingDto> GetMeetingsForMonth(int year, int month);
    void UpsertMeeting(CalendarMeetingDto meeting);
    void RefreshInBackground();
}
