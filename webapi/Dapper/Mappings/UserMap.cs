using Dapper.FluentMap.Mapping;
using webapi.Models.Basic;

namespace webapi.Dapper.Mappings
{
    public class UserMap : EntityMap<User>
    {
        public UserMap()
        {
        }
    }
}
