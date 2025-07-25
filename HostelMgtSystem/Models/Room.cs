namespace HostelMgtSystem.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int HostelId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;

        public Hostel? Hostel { get; set; }
    }
}
