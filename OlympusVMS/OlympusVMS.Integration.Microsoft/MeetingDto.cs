namespace OlympusVMS.Integration.Microsoft
{
    public class MeetingDto
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset? Start { get; set; }
        public DateTimeOffset? End { get; set; }
        public string Location { get; set; }
    }
}
