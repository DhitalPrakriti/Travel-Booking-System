namespace TravelBookingSystem.Models
{
    public class Destinations
    {
        public int DestinationId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public int Popularity { get; set; }
        public string? Location { get; set; }  
    }
}

