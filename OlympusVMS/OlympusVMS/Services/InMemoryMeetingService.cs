using System.Collections.Concurrent;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Services;

public class InMemoryMeetingService : IMeetingService
{
    private static readonly ConcurrentDictionary<string, RegisterGuestForm> _guestStore = new(StringComparer.OrdinalIgnoreCase);

    public Task<List<LoadTodayMeetingDto>> LoadTodayMeetings(CancellationToken token)
    {
        var now = DateTimeOffset.Now;

        var meetings = _guestStore.Values
            .Where(x => x.MeetingTime >= now)
            .OrderBy(x => x.MeetingTime)
            .Select(x => new LoadTodayMeetingDto(x.FirstName, x.LastName, x.MeetingTime))
            .ToList();

        return Task.FromResult(meetings);
    }

    public Task<CreateOneMeetingResponse> RegisterGuest(RegisterGuestForm form)
    {
        var normalized = Normalize(form);
        _guestStore[BuildTripletKey(normalized.FirstName, normalized.LastName, normalized.MeetingTime)] = normalized;

        return Task.FromResult(new CreateOneMeetingResponse
        {
            RecordId = Guid.NewGuid(),
            FirstName = normalized.FirstName,
            LastName = normalized.LastName,
            MeetingTime = normalized.MeetingTime,
            RegisteredSuccessfully = true
        });
    }

    public Task<int> RegisterGuestsFromFormsAsync(IEnumerable<RegisterGuestForm> forms, CancellationToken token = default)
    {
        var added = 0;

        foreach (var form in forms)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(form.FirstName) ||
                string.IsNullOrWhiteSpace(form.LastName) ||
                form.MeetingTime == default)
            {
                continue;
            }

            var normalized = Normalize(form);
            var key = BuildTripletKey(normalized.FirstName, normalized.LastName, normalized.MeetingTime);

            if (_guestStore.TryAdd(key, normalized))
            {
                added++;
            }
        }

        return Task.FromResult(added);
    }

    private static RegisterGuestForm Normalize(RegisterGuestForm form)
    {
        return new RegisterGuestForm
        {
            FirstName = form.FirstName.Trim(),
            LastName = form.LastName.Trim(),
            EmailAddress = form.EmailAddress?.Trim() ?? string.Empty,
            RegisteredBy = string.IsNullOrWhiteSpace(form.RegisteredBy) ? "MockGraph" : form.RegisteredBy.Trim(),
            MeetingTime = form.MeetingTime,
            MeetingDurationInHours = form.MeetingDurationInHours
        };
    }

    private static string BuildTripletKey(string firstName, string lastName, DateTimeOffset meetingTime)
    {
        return $"{firstName.Trim().ToUpperInvariant()}|{lastName.Trim().ToUpperInvariant()}|{meetingTime:O}";
    }
}
