using Microsoft.AspNetCore.Components;
using OlympusVMS.Utils.DTOs;

namespace OlympusVMS.Components.Pages.RegisterGuest;

public partial class RegisterGuest : ComponentBase
{
    protected RegisterGuestForm Form { get; } = new();
    protected string SuccessMessage {  get; set; }

    protected void HandleValidSubmit()
    {
        Console.WriteLine($"Saved: {Form.FirstName}");
    }
}
