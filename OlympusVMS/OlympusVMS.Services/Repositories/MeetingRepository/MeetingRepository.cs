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
    
    public async Task RegisterGuestAsync(GuestRecord record)
    {
        _context.GuestRecords.Add(record);
        
        await _context.SaveChangesAsync();
    }
}