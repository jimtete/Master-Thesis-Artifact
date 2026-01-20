using System.ComponentModel.DataAnnotations;

namespace OlympusVMS.Utils.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = ConfigSections.Database;
    
    [Required]
    public string ConnectionString { get; init; } =  string.Empty;
}