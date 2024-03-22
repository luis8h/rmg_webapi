using Dapper.FluentMap;
using webapi.Dapper.Mappings;

namespace webapi.Dapper
{
    public class InitDapper
    {
        public static void Init()
        {
            FluentMapper.Initialize(config =>
                    {
                    config.AddMap(new RecipeMap());
                    config.AddMap(new DetailRecipeMap());
                    config.AddMap(new TagMap());
                    config.AddMap(new RatingMap());
                    config.AddMap(new UserMap());
                    });

            // FluentMapper.Initialize(config => {
            //         config.AddMap(new RecipeMap());
            //         });
            //
            // FluentMapper.Initialize(config => {
            //         config.AddMap(new DetailRecipeMap());
            //         });
            //
            // FluentMapper.Initialize(config => {
            //         config.AddMap(new TagMap());
            //         });
            //
            // FluentMapper.Initialize(config => {
            //         config.AddMap(new RatingMap());
            //         });
        }
    }
}
