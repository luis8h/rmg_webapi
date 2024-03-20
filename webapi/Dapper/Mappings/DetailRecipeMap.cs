using Dapper.FluentMap.Mapping;
using webapi.Models.Extended;

namespace webapi.Dapper.Mappings
{
    public class DetailRecipeMap : EntityMap<DetailRecipe> {

        public DetailRecipeMap()
        {
            Map(r => r.Id).ToColumn("recipe_id");
            Map(r => r.Name).ToColumn("recipe_name");
        }

    }
}

