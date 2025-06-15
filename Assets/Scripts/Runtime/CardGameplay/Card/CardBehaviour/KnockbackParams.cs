using System;
using Runtime.Combat.Pawn;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [Serializable]
    public class KnockbackParams : GetPawnsParams
    {
        public MovementDirection Direction;
    }
}
