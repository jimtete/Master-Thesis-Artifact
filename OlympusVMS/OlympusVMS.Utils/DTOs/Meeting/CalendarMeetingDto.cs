namespace OlympusVMS.Utils.DTOs.Meeting;

public record CalendarMeetingDto(
    Guid RecordId,
    string FirstName,
    string LastName,
    DateTimeOffset MeetingTime,
    bool Visited
);
