using webapi.Models.Basic;

namespace webapi.Models.Dtos
{
    public class PutRecipeDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Preptime { get; set; }
        public int? Cooktime { get; set; }
        public int? Worktime { get; set; }
        public int? Difficulty { get; set; }
        public DateTime? EditedAt { get; set; }
        public int? EditedBy { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Rating> Ratings { get; set; }

        public PutRecipeDto()
        {
            Tags = new List<Tag>();
            Ratings = new List<Rating>();
        }
    }
}
