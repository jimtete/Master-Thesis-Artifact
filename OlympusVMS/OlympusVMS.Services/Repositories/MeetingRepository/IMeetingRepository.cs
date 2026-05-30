using OlympusVMS.Utils.Models;

namespace OlympusVMS.Services.Repositories.MeetingRepository;

public interface IMeetingRepository
{
    Task<List<GuestRecord>> LoadAllMeetingsAsync(CancellationToken token = default);
    Task<GuestRecord> RegisterGuestAsync(GuestRecord record);
    Task<GuestRecord> UpdateVisitedStatusAsync(Guid recordId, bool visited, CancellationToken token = default);
}
