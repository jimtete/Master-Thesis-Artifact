using OlympusVMS.Utils.Models;

namespace OlympusVMS.Services.Repositories.MeetingRepository;

public interface IMeetingRepository
{
    Task<GuestRecord> RegisterGuestAsync(GuestRecord record);
    Task<List<GuestRecord>> GetUpcomingGuestRecordsAsync(DateTimeOffset fromInclusive, CancellationToken token = default);
    Task<List<GuestRecord>> GetGuestRecordsInRangeAsync(DateTimeOffset fromInclusive, DateTimeOffset toInclusive, CancellationToken token = default);
    Task AddGuestRecordsAsync(List<GuestRecord> records, CancellationToken token = default);
}