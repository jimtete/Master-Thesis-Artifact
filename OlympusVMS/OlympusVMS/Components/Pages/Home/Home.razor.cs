using Microsoft.AspNetCore.Components;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] public IMeetingService MeetingService { get; set; } = default;
    
    private readonly CancellationTokenSource _cts = new();
    

    private static string TodayText => DateTime.Today.ToShortDateString();
    private static bool IsLoading { get; set; } = true;
    private static string ErrorMessage { get; set; }
    private static IReadOnlyList<LoadTodayMeetingDto> Meetings { get; set; } = Array.Empty<LoadTodayMeetingDto>();


    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        
        Meetings = await MeetingService.LoadTodayMeetings(_cts.Token);

        if (Meetings is null || Meetings.Count == 0)
        {
            ErrorMessage = $"{nameof(Meetings)} is empty";
        }
        
        IsLoading = false;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}