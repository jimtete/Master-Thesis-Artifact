using OlympusVMS.Services.Interfaces;
using OlympusVMS.Services.Repositories.MeetingRepository;
using OlympusVMS.Utils.DTOs;
using OlympusVMS.Utils.DTOs.Meeting;
using OlympusVMS.Utils.Models;

namespace OlympusVMS.Services.Services;

public class MeetingService : IMeetingService
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IMeetingCache _meetingCache;

    public MeetingService(IMeetingRepository meetingRepository, IMeetingCache meetingCache)
    {
        _meetingRepository = meetingRepository;
        _meetingCache = meetingCache;
    }

    public async Task<CreateOneMeetingResponse> RegisterGuest(RegisterGuestForm form)
    {
        var meetingTime = new DateTimeOffset(DateTime.SpecifyKind(form.MeetingTime, DateTimeKind.Local));

        var guestRecord = new GuestRecord
        {
            EmailAddress = form.EmailAddress,
            FirstName = form.FirstName,
            LastName = form.LastName,
            MeetingTime = meetingTime,
            MeetingDurationInHours = form.MeetingDurationInHours,
            RegisteredBy = form.RegisteredBy
        };
        
        var insertedRecord = await _meetingRepository.RegisterGuestAsync(guestRecord);
        _meetingCache.UpsertMeeting(new CalendarMeetingDto(
            insertedRecord.RecordId,
            insertedRecord.FirstName,
            insertedRecord.LastName,
            insertedRecord.MeetingTime,
            insertedRecord.Visited));
        _meetingCache.RefreshInBackground();

        return new CreateOneMeetingResponse
        {
            FirstName = insertedRecord.FirstName,
            LastName = insertedRecord.LastName,
            MeetingTime = insertedRecord.MeetingTime,
            RecordId = insertedRecord.RecordId,
            RegisteredSuccessfully = true,
        };
    }

    public async Task SetVisitedStatusAsync(Guid recordId, bool visited, CancellationToken token = default)
    {
        var updatedRecord = await _meetingRepository.UpdateVisitedStatusAsync(recordId, visited, token);

        _meetingCache.UpsertMeeting(new CalendarMeetingDto(
            updatedRecord.RecordId,
            updatedRecord.FirstName,
            updatedRecord.LastName,
            updatedRecord.MeetingTime,
            updatedRecord.Visited));
    }
}
