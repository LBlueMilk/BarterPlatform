namespace BarterPlatform.Models
{
    public class PersonalPageViewModel
    {
        public Member Member { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Item> Items { get; set; }
    }
}
