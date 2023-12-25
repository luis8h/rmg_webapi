namespace webapi.Models.Basic
{
    public class Recipe
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Preptime { get; set; }
        public int? Cooktime { get; set; }
        public int? Worktime { get; set; }
        public int? Difficulty { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? EditedAt { get; set; }
        public int? EditedBy { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Rating> Ratings { get; set; }

        public Recipe()
        {
            Tags = new List<Tag>();
            Ratings = new List<Rating>();
        }
    }
}

