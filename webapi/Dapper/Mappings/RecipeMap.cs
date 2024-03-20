using Dapper.FluentMap.Mapping;
using webapi.Models.Basic;

namespace webapi.Dapper.Mappings
{
    public class RecipeMap : EntityMap<Recipe>
    {
        public  RecipeMap()
        {
            Map(r => r.Id).ToColumn("recipe_id");
            Map(r => r.Name).ToColumn("recipe_name");
        }
    }
}
