namespace OlympusVMS.Utils.DTOs.Meeting;

public class CreateOneMeetingResponse
{
    public Guid RecordId { get; set; }
    public DateTimeOffset MeetingTime { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool RegisteredSuccessfully { get; set; }
}