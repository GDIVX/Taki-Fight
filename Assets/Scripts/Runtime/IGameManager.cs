using Runtime.Combat.Pawn;

namespace Runtime
{
    public interface IGameManager
    {
        PawnController Hero { get; }
    }
}