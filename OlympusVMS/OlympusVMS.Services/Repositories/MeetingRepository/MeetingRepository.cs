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
}