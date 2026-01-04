using OlympusVMS.Utils.Models;

namespace OlympusVMS.Components.Pages.Home;

public partial class Home
{
    protected string TodayText => DateTime.Today.ToShortDateString();

    protected List<GuestRecord> GuestRecords { get; } = new()
    {
        
    };
}