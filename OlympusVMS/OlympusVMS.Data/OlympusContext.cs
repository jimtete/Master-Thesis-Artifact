using Microsoft.EntityFrameworkCore;

namespace OlympusVMS.Data;

public class OlympusContext : DbContext
{
    public OlympusContext(DbContextOptions<OlympusContext> options) :  base(options)
    {
        
    }
}