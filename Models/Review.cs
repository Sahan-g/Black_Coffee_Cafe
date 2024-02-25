namespace Black_Coffee_Cafe.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }  
        public string Time { get; set; }
        public string ReviewDescription { get; set; }

    }
}
