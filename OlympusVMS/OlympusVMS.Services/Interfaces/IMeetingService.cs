using OlympusVMS.Utils.DTOs;
using OlympusVMS.Utils.DTOs.Meeting;

namespace OlympusVMS.Services.Interfaces;

public interface IMeetingService
{
    Task<CreateOneMeetingResponse> RegisterGuest(RegisterGuestForm form);
    Task SetVisitedStatusAsync(Guid recordId, bool visited, CancellationToken token = default);
}
