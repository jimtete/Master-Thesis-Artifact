using System.Collections.Generic;

namespace OlympusVMS.Integration.Microsoft.Configuration
{
    public class MeetingRoomOptions
    {
        public const string SectionName = "MeetingRooms";

        public List<string> Emails { get; set; } = new();
    }
}