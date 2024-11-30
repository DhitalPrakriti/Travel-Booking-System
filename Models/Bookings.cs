namespace TravelBookingSystem.Models
{
    public class Bookings
    {
        public int BookingId { get; set; }
        public string ? UserName { get; set; }
        public int DestinationId { get; set; }
        public DateTime BookingDate { get; set; }
        public Destinations ? Destination { get; set; }


    }
}
