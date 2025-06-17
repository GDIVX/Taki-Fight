using System;
using Runtime.CardGameplay.Card;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackMod
{
    public interface IDamageHandler : IDescribable
    {
        DamageResult DamagePawn(int inDamage, int currentArmor);
    }


    public readonly struct DamageResult
    {
        public readonly int HealthOnlyDamage;
        public readonly int ArmorOnlyDamage;

        public DamageResult(int healthOnlyDamage, int armorOnlyDamage)
        {
            HealthOnlyDamage = healthOnlyDamage;
            ArmorOnlyDamage = armorOnlyDamage;
        }
    }


    /// <summary>
    ///     Normal damage that dosen't modify the inDamage
    /// </summary>
    [Serializable]
    public class NormalDamageHandler : IDamageHandler
    {
        public DamageResult DamagePawn(int inDamage, int currentArmor)
        {
            var armorDamage = Mathf.Min(inDamage, currentArmor);
            var healthDamage = inDamage - armorDamage;

            return new DamageResult(healthDamage, armorDamage);
        }

        public string GetDescription()
        {
            return "Damage";
        }
    }


    /// <summary>
    ///     Ignores armor and attack health directly
    /// </summary>
    [Serializable]
    public class PierceDamageHandler : IDamageHandler
    {
        public DamageResult DamagePawn(int inDamage, int currentArmor)
        {
            return new DamageResult(inDamage, 0);
        }

        public string GetDescription()
        {
            var builder = new DescriptionBuilder();
            builder.WithKeyword("Piercing");
            builder.AppendInLine(" Damage");
            return builder.GetFormattedText();
        }
    }


    /// <summary>
    ///     Damage armor only
    /// </summary>
    [Serializable]
    public class ShredDamageHandler : IDamageHandler
    {
        public DamageResult DamagePawn(int inDamage, int currentArmor)
        {
            var armorDamage = Mathf.Min(inDamage, currentArmor);
            return new DamageResult(0, armorDamage);
        }

        public string GetDescription()
        {
            var builder = new DescriptionBuilder();
            builder.WithKeyword("Sundering");
            builder.AppendInLine(" Damage");
            return builder.GetFormattedText();
        }
    }

    /// <summary>
    ///     Double damage against armor, spillover isn't doubled
    /// </summary>
    [Serializable]
    public class BluntDamageHandler : IDamageHandler
    {
        public DamageResult DamagePawn(int inDamage, int currentArmor)
        {
            var rawArmorDamage = Mathf.Min(inDamage, currentArmor);
            var armorDamage = Mathf.Min(currentArmor, rawArmorDamage * 2);
            var remainingDamage = inDamage - rawArmorDamage;

            return new DamageResult(remainingDamage, armorDamage);
        }

        public string GetDescription()
        {
            var builder = new DescriptionBuilder();
            builder.WithKeyword("Blunt");
            builder.AppendInLine(" Damage");
            return builder.GetFormattedText();
        }
    }

    /// <summary>
    ///     Double damage to health after spillover
    /// </summary>
    [Serializable]
    public class CutDamageHandler : IDamageHandler
    {
        public DamageResult DamagePawn(int inDamage, int currentArmor)
        {
            var armorDamage = Mathf.Min(inDamage, currentArmor);
            var remainingDamage = inDamage - armorDamage;
            var healthDamage = remainingDamage * 2;

            return new DamageResult(healthDamage, armorDamage);
        }

        public string GetDescription()
        {
            var builder = new DescriptionBuilder();
            builder.WithKeyword("Slash");
            builder.AppendInLine(" Damage");
            return builder.GetFormattedText();
        }
    }
}