namespace Runtime.Combat.Pawn.Targeting
{
    public interface ITarget<out T>
    {
        public void SetAsTarget();
    }
}