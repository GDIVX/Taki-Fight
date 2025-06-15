using System;
using Runtime.Combat.Pawn.AttackMod;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [Serializable]
    public class AOEDamageParams : AOEPlayParams
    {
        public int Damage;
        [SerializeReference] public IDamageHandler DamageHandler = new NormalDamageHandler();
    }
}
