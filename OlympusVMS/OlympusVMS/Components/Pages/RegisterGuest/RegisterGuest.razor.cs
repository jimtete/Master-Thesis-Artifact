using Microsoft.AspNetCore.Components;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs;

namespace OlympusVMS.Components.Pages.RegisterGuest;

public partial class RegisterGuest : ComponentBase
{
    [Inject]
    public IMeetingService MeetingService { get; set; }

    protected RegisterGuestForm Form { get; set; } = new();
    protected string SuccessMessage {  get; set; }

    protected override void OnInitialized()
    {
        var now = DateTime.Now;
        Form.MeetingTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, now.Kind);
        Form.RegisteredBy = "admin";
    }

    protected async Task HandleValidSubmit()
    {
        await MeetingService.RegisterGuest(Form);
        SuccessMessage = "Your guest has been successfully registered.";
    }
}
