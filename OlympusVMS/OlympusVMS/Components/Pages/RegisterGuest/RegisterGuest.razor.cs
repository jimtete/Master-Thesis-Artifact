using Microsoft.AspNetCore.Components;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Utils.DTOs;

namespace OlympusVMS.Components.Pages.RegisterGuest;

public partial class RegisterGuest : ComponentBase
{
    [Inject]
    public IMeetingService MeetingService { get; set; }
    
    [SupplyParameterFromForm]
    protected RegisterGuestForm Form { get; set; } = new();
    protected string SuccessMessage {  get; set; }

    protected override void OnInitialized()
    {
        Form.MeetingTime = DateTime.Now;
    }

    protected async Task HandleValidSubmit()
    {
        await MeetingService.RegisterGuest(Form);
        SuccessMessage = "Your guest has been successfully registered.";
    }
}
