namespace Utilities
{
    public abstract class Service<T> where T : class
    {
        protected Service()
        {
            ServiceLocator.Register(this as T);
        }
    }
}