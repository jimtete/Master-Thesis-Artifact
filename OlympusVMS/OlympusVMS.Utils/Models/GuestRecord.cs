namespace OlympusVMS.Utils.Models;

public class GuestRecord
{
    public Guid RecordId { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string EmailAddress { get; set; } = "";
    public string RegisteredBy { get; set; } = "";

    public DateTimeOffset MeetingTime { get; set; }
    public int? MeetingDurationInHours { get; set; }
}