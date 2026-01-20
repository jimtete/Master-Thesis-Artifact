using System.ComponentModel.DataAnnotations;

namespace OlympusVMS.Utils.DTOs.Meeting;

public class CreateOneMeetingRequest
{
    [Required, MinLength(3), MaxLength(100)]
    public string FirstName { get; set; }
    
    [Required, MinLength(3), MaxLength(100)]
    public string LastName { get; set; }
    
    [Required, MinLength(3), MaxLength(200)]
    public string EmailAddress { get; set; }
    
    [Required, MinLength(3), MaxLength(200)]
    public string RegisteredBy { get; set; }
    
    [Required]
    public DateTimeOffset MeetingTime { get; set; }

    public int? MeetingDurationInHours { get; set; }
}