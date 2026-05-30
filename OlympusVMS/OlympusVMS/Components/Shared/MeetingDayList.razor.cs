using Microsoft.AspNetCore.Components;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Components.Shared;

public partial class MeetingDayList : ComponentBase
{
    [Parameter]
    public IReadOnlyList<CalendarMeetingDto> Meetings { get; set; } = Array.Empty<CalendarMeetingDto>();

    [Parameter]
    public string EmptyMessage { get; set; } = "No meetings found.";

    protected CalendarMeetingDto SelectedMeeting { get; set; }
    protected bool SelectedVisited { get; set; }
    protected bool IsSaving { get; set; }
    protected string ErrorMessage { get; set; }

    protected void OpenMeeting(CalendarMeetingDto meeting)
    {
        SelectedMeeting = meeting;
        SelectedVisited = meeting.Visited;
        ErrorMessage = string.Empty;
    }

    protected void CloseModal()
    {
        if (IsSaving)
        {
            return;
        }

        SelectedMeeting = null;
        ErrorMessage = string.Empty;
    }

    protected async Task SaveVisitedStatusAsync()
    {
        if (SelectedMeeting is null)
        {
            return;
        }

        IsSaving = true;
        ErrorMessage = string.Empty;

        try
        {
            await MeetingService.SetVisitedStatusAsync(SelectedMeeting.RecordId, SelectedVisited);
            SelectedMeeting = null;
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsSaving = false;
        }
    }
}
