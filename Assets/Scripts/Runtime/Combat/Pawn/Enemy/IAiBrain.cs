using System.Collections;

namespace Runtime.Combat.Pawn.Enemy
{
    public interface IAiBrain
    {
        IEnumerator ChoseAndPlayStrategy();
    }
}