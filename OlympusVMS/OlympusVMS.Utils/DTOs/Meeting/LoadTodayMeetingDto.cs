namespace OlympusVMS.Utils.DTOs.Meeting;

public record LoadTodayMeetingDto(
    string FirstName,
    string LastName,
    DateTimeOffset MeetingTime
);