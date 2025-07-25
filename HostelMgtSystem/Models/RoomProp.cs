namespace HostelMgtSystem.Models
{
    public class RoomProp
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string PropertyName { get; set; } = string.Empty; 
        public string PropertyValue { get; set; } = string.Empty; 

        public Room? Room { get; set; }
    }
}
