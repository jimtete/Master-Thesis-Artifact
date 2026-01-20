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
        await Task.Delay(500, token);

        return new List<LoadTodayMeetingDto>
        {
            new("Anna", "Berlin", DateTimeOffset.Now.AddHours(1)),
            new("Dimitrios", "Papadopoulos", DateTimeOffset.Now.AddHours(2)),
            new("Maria", "Khan", DateTimeOffset.Now.AddHours(3)),
        };
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
        };
    }
}