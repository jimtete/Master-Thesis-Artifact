using Microsoft.EntityFrameworkCore;
using OlympusVMS.Data;
using OlympusVMS.Utils.Models;

namespace OlympusVMS.Services.Repositories.MeetingRepository;

public class MeetingRepository : IMeetingRepository
{
    private readonly OlympusContext _context;

    public MeetingRepository(OlympusContext context)
    {
        _context = context;
    }
    
    public async Task<GuestRecord> RegisterGuestAsync(GuestRecord record)
    {
        await _context.GuestRecords.AddAsync(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<List<GuestRecord>> GetUpcomingGuestRecordsAsync(DateTimeOffset fromInclusive, CancellationToken token = default)
    {
        return await _context.GuestRecords
            .Where(x => x.MeetingTime >= fromInclusive)
            .OrderBy(x => x.MeetingTime)
            .ToListAsync(token);
    }

    public async Task<List<GuestRecord>> GetGuestRecordsInRangeAsync(DateTimeOffset fromInclusive, DateTimeOffset toInclusive, CancellationToken token = default)
    {
        return await _context.GuestRecords
            .Where(x => x.MeetingTime >= fromInclusive && x.MeetingTime <= toInclusive)
            .ToListAsync(token);
    }

    public async Task AddGuestRecordsAsync(List<GuestRecord> records, CancellationToken token = default)
    {
        if (records.Count == 0)
        {
            return;
        }

        await _context.GuestRecords.AddRangeAsync(records, token);
        await _context.SaveChangesAsync(token);
    }
}