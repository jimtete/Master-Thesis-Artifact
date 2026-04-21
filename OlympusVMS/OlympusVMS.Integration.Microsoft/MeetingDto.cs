namespace OlympusVMS.Integration.Microsoft
{
    public class MeetingDto
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string Organizer { get; set; }
        public DateTimeOffset? Start { get; set; }
        public DateTimeOffset? End { get; set; }
        public List<AttendeeDto> Attendees { get; set; } = new();
    }

    public class AttendeeDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class RoomDto
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }
}
