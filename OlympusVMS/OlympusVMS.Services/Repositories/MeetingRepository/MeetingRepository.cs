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

    public async Task<List<GuestRecord>> LoadAllMeetingsAsync(CancellationToken token = default)
    {
        return await _context.GuestRecords
            .AsNoTracking()
            .OrderBy(record => record.MeetingTime)
            .ThenBy(record => record.LastName)
            .ThenBy(record => record.FirstName)
            .ToListAsync(token);
    }
    
    public async Task<GuestRecord> RegisterGuestAsync(GuestRecord record)
    {
        await _context.GuestRecords.AddAsync(record);
        
        await _context.SaveChangesAsync();
        
        return record;
    }

    public async Task<GuestRecord> UpdateVisitedStatusAsync(Guid recordId, bool visited, CancellationToken token = default)
    {
        var record = await _context.GuestRecords.FirstOrDefaultAsync(x => x.RecordId == recordId, token)
            ?? throw new InvalidOperationException($"Guest record '{recordId}' was not found.");

        record.Visited = visited;
        await _context.SaveChangesAsync(token);

        return record;
    }
}
