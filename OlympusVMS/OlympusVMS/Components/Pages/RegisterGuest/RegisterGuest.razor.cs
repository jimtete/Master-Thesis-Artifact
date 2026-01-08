using Microsoft.AspNetCore.Components;
using OlympusVMS.Utils.DTOs;

namespace OlympusVMS.Components.Pages.RegisterGuest;

public partial class RegisterGuest : ComponentBase
{
    [SupplyParameterFromForm]
    protected RegisterGuestForm Form { get; set; } = new();
    protected string SuccessMessage {  get; set; }

    protected override void OnInitialized()
    {
        Form.MeetingTime = DateTime.Now;
    }

    protected void HandleValidSubmit()
    {
        Console.WriteLine($"Saved: {Form.FirstName}");
    }
}
