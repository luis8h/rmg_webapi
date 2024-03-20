using Dapper.FluentMap;
using webapi.Dapper.Mappings;

namespace webapi.Dapper
{
    public class InitDapper
    {
        public static void Init()
        {
            FluentMapper.Initialize(config => {
                    config.AddMap(new RecipeMap());
                    });

            FluentMapper.Initialize(config => {
                    config.AddMap(new TagMap());
                    });

            FluentMapper.Initialize(config => {
                    config.AddMap(new RatingMap());
                    });
        }
    }
}
