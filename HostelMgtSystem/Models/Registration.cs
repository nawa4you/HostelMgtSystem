namespace HostelMgtSystem.Models
{
    public class Registration
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;
        public int HostelId { get; set; }
        public int? RoomId { get; set; }

        public Hostel? Hostel { get; set; }
        public Room? Room { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;



    }
}