namespace Utilities
{
    public abstract class Service
    {
        protected Service()
        {
            ServiceLocator.Register(this);
        }
    }
}