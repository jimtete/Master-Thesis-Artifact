using OlympusVMS.Services.Interfaces;
using OlympusVMS.Services.Repositories.MeetingRepository;
using OlympusVMS.Utils.DTOs;
using OlympusVMS.Utils.DTOs.Meeting;
using OlympusVMS.Utils.Models;

namespace OlympusVMS.Services.Services;

public class MeetingService : IMeetingService
{
    private readonly IMeetingRepository _meetingRepository;

    public MeetingService(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }
    
    public async Task<List<LoadTodayMeetingDto>> LoadTodayMeetings(CancellationToken token = default)
    {
        var records = await _meetingRepository.GetUpcomingGuestRecordsAsync(DateTimeOffset.Now, token);

        return records
            .OrderBy(x => x.MeetingTime)
            .Select(x => new LoadTodayMeetingDto(x.FirstName, x.LastName, x.MeetingTime))
            .ToList();
    }

    public async Task<CreateOneMeetingResponse> RegisterGuest(RegisterGuestForm form)
    {
        var guestRecord = new GuestRecord
        {
            EmailAddress = form.EmailAddress,
            FirstName = form.FirstName,
            LastName = form.LastName,
            MeetingTime = form.MeetingTime,
            MeetingDurationInHours = form.MeetingDurationInHours,
            RegisteredBy = form.RegisteredBy
        };
        
        var insertedRecord = await _meetingRepository.RegisterGuestAsync(guestRecord);

        return new CreateOneMeetingResponse
        {
            FirstName = insertedRecord.FirstName,
            LastName = insertedRecord.LastName,
            MeetingTime = insertedRecord.MeetingTime,
            RecordId = insertedRecord.RecordId,
            RegisteredSuccessfully = true,
        };
    }

    public async Task<int> RegisterGuestsFromFormsAsync(IEnumerable<RegisterGuestForm> forms, CancellationToken token = default)
    {
        var normalizedForms = forms
            .Where(f =>
                !string.IsNullOrWhiteSpace(f.FirstName) &&
                !string.IsNullOrWhiteSpace(f.LastName) &&
                f.MeetingTime != default)
            .Select(f => new RegisterGuestForm
            {
                FirstName = f.FirstName.Trim(),
                LastName = f.LastName.Trim(),
                EmailAddress = f.EmailAddress?.Trim() ?? string.Empty,
                RegisteredBy = string.IsNullOrWhiteSpace(f.RegisteredBy) ? "AutoGraphSync" : f.RegisteredBy.Trim(),
                MeetingTime = f.MeetingTime,
                MeetingDurationInHours = f.MeetingDurationInHours
            })
            .ToList();

        if (normalizedForms.Count == 0)
        {
            return 0;
        }

        var minMeetingTime = normalizedForms.Min(x => x.MeetingTime);
        var maxMeetingTime = normalizedForms.Max(x => x.MeetingTime);

        var existing = await _meetingRepository.GetGuestRecordsInRangeAsync(minMeetingTime, maxMeetingTime, token);

        var existingTriplets = new HashSet<string>(
            existing.Select(x => BuildTripletKey(x.FirstName, x.LastName, x.MeetingTime)),
            StringComparer.OrdinalIgnoreCase);

        var uniqueIncoming = new Dictionary<string, RegisterGuestForm>(StringComparer.OrdinalIgnoreCase);
        foreach (var form in normalizedForms)
        {
            var key = BuildTripletKey(form.FirstName, form.LastName, form.MeetingTime);
            if (!uniqueIncoming.ContainsKey(key))
            {
                uniqueIncoming[key] = form;
            }
        }

        var toInsert = uniqueIncoming
            .Where(x => !existingTriplets.Contains(x.Key))
            .Select(x => new GuestRecord
            {
                FirstName = x.Value.FirstName,
                LastName = x.Value.LastName,
                EmailAddress = x.Value.EmailAddress,
                MeetingTime = x.Value.MeetingTime,
                MeetingDurationInHours = x.Value.MeetingDurationInHours,
                RegisteredBy = x.Value.RegisteredBy
            })
            .ToList();

        await _meetingRepository.AddGuestRecordsAsync(toInsert, token);

        return toInsert.Count;
    }

    private static string BuildTripletKey(string firstName, string lastName, DateTimeOffset meetingTime)
    {
        return $"{firstName.Trim().ToUpperInvariant()}|{lastName.Trim().ToUpperInvariant()}|{meetingTime:O}";
    }
}