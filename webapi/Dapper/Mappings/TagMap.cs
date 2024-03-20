using Dapper.FluentMap.Mapping;
using webapi.Models.Basic;

namespace webapi.Dapper.Mappings
{
    public class TagMap : EntityMap<Tag> {

        public TagMap()
        {
            Map(t => t.Id).ToColumn("tag_id");
            Map(t => t.Name).ToColumn("tag_name");
        }

    }
}
