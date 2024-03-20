using Dapper.FluentMap.Mapping;
using webapi.Models.Basic;

namespace webapi.Dapper.Mappings
{
    public class RatingMap : EntityMap<Rating>
    {
        public  RatingMap()
        {
            Map(r => r.Id).ToColumn("rating_id");
            Map(r => r.User).ToColumn("user_id");
        }
    }
}
