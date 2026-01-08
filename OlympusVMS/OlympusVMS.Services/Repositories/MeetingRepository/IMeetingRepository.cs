using OlympusVMS.Utils.Models;

namespace OlympusVMS.Services.Repositories.MeetingRepository;

public interface IMeetingRepository
{
    Task RegisterGuestAsync(GuestRecord record);
}