using System;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.AttackMod;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [Serializable]
    public class GetPawnsParams : StrategyParams
    {
        public int TargetsCount;
        public PawnOwner PawnOwner;
    }

    [Serializable]
    public class AttackParams : GetPawnsParams
    {
        [SerializeReference] public IDamageHandler DamageHandler;
    }
}