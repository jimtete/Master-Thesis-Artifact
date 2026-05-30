using Microsoft.Extensions.Logging;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Services.Repositories.MeetingRepository;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Services.Services;

public sealed class MeetingCache : IMeetingCache, IDisposable
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly ILogger<MeetingCache> _logger;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private IReadOnlyList<CalendarMeetingDto> _allMeetings = Array.Empty<CalendarMeetingDto>();
    private bool _disposed;

    public MeetingCache(IMeetingRepository meetingRepository, ILogger<MeetingCache> logger)
    {
        _meetingRepository = meetingRepository;
        _logger = logger;
    }

    public event Action? Changed;

    public bool IsInitialized { get; private set; }
    public bool IsRefreshing { get; private set; }

    public async Task EnsureLoadedAsync(CancellationToken token = default)
    {
        if (IsInitialized)
        {
            return;
        }

        await RefreshAsync(waitForExistingRefresh: true, token);
    }

    public IReadOnlyList<CalendarMeetingDto> GetTodayMeetings()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return _allMeetings
            .Where(meeting => DateOnly.FromDateTime(meeting.MeetingTime.Date) == today)
            .OrderBy(meeting => meeting.MeetingTime)
            .ToList();
    }

    public IReadOnlyList<CalendarMeetingDto> GetMeetingsForMonth(int year, int month)
    {
        return _allMeetings
            .Where(meeting => meeting.MeetingTime.Year == year && meeting.MeetingTime.Month == month)
            .OrderBy(meeting => meeting.MeetingTime)
            .ThenBy(meeting => meeting.LastName)
            .ThenBy(meeting => meeting.FirstName)
            .ToList();
    }

    public void UpsertMeeting(CalendarMeetingDto meeting)
    {
        var updatedMeetings = _allMeetings
            .Where(existingMeeting => existingMeeting.RecordId != meeting.RecordId)
            .Append(meeting)
            .OrderBy(existingMeeting => existingMeeting.MeetingTime)
            .ThenBy(existingMeeting => existingMeeting.LastName)
            .ThenBy(existingMeeting => existingMeeting.FirstName)
            .ToList();

        _allMeetings = updatedMeetings;
        IsInitialized = true;
        NotifyChanged();
    }

    public void RefreshInBackground()
    {
        _ = RefreshInBackgroundAsync();
    }

    private async Task RefreshInBackgroundAsync()
    {
        try
        {
            await RefreshAsync(waitForExistingRefresh: false, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh meeting cache in the background.");
        }
    }

    private async Task RefreshAsync(bool waitForExistingRefresh, CancellationToken token)
    {
        if (waitForExistingRefresh)
        {
            await _refreshLock.WaitAsync(token);
        }
        else
        {
            var lockTaken = await _refreshLock.WaitAsync(0, token);
            if (!lockTaken)
            {
                return;
            }
        }

        try
        {
            IsRefreshing = true;
            NotifyChanged();

            var records = await _meetingRepository.LoadAllMeetingsAsync(token);
            _allMeetings = records
                .Select(record => new CalendarMeetingDto(
                    record.RecordId,
                    record.FirstName,
                    record.LastName,
                    record.MeetingTime,
                    record.Visited))
                .OrderBy(meeting => meeting.MeetingTime)
                .ThenBy(meeting => meeting.LastName)
                .ThenBy(meeting => meeting.FirstName)
                .ToList();

            IsInitialized = true;
        }
        finally
        {
            IsRefreshing = false;
            _refreshLock.Release();
            NotifyChanged();
        }
    }

    private void NotifyChanged()
    {
        if (_disposed)
        {
            return;
        }

        Changed?.Invoke();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _refreshLock.Dispose();
    }
}
