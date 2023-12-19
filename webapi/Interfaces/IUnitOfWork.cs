namespace webapi.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IRecipeRepository RecipeRepository { get; }
        ITagRepository TagRepository { get; }
    }
}
