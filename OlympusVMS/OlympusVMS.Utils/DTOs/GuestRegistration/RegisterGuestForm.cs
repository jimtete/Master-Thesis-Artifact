using System.ComponentModel.DataAnnotations;

namespace OlympusVMS.Utils.DTOs;

public class RegisterGuestForm
{
    [Required, MinLength(3), MaxLength(100)]
    public string FirstName { get; set; } = String.Empty;
    
    [Required, MinLength(3), MaxLength(100)]
    public string LastName { get; set; }  = String.Empty;
    
    [Required, MinLength(3), MaxLength(200)]
    public string EmailAddress { get; set; } = String.Empty;

    [Required, MinLength(3), MaxLength(200)]
    public string RegisteredBy { get; set; } = "DTE";
    
    [Required]
    public DateTimeOffset MeetingTime { get; set; }
    
    [Range(1, 23)]
    public int? MeetingDurationInHours { get; set; }
}