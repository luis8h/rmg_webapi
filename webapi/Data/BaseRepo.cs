namespace webapi.Data
{
    public abstract class BaseRepo
    {
        public TResult  AccessDB<TResult>(Func<TResult> dbFunction)
        {
            TResult result =  dbFunction();

            return result;
        }
    }
}
