using OlympusVMS.Utils.Models;

namespace OlympusVMS.Components.Pages.Home;

public partial class Home
{
    protected string TodayText => DateTime.Today.ToShortDateString();

    protected List<GuestRecord> GuestRecords { get; } = new()
    {
        new GuestRecord
        {
            FirstName = "Anna", LastName = "Berlin", MeetingTime = DateTimeOffset.Now.AddHours(1),
        },
        new GuestRecord
        {
            FirstName = "Dimitrios", LastName = "Papadopoulos", MeetingTime = DateTimeOffset.Now.AddHours(2),
        },
        new GuestRecord
        {
            FirstName = "Maria", LastName = "Khan", MeetingTime = DateTimeOffset.Now.AddHours(3),
        }
    };
}