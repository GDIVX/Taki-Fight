using Runtime.Combat.Pawn.Abilities;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Runtime.Combat.StatusEffects;
using Runtime.CardGameplay.Card.View;
using Utilities;
using UnityEngine;

namespace AbilityTests
{
    public class MoveAbilityTests
    {
        [Test]
        public void GetDescription_Forward()
        {
            var ability = new MoveAbility();
            var data = new PawnStrategyData
            {
                Parameters = new MoveDirectionParams { Direction = MovementDirection.Forward },
                Potency = 2
            };
            ability.Initialize(data);

            Assert.That(ability.GetDescription(), Is.EqualTo("Dash 2 tiles forward"));
        }

        [Test]
        public void Play_MovesPawnForward()
        {
            var tilemap = new TilemapController();
            var start = new Tile(Vector2Int.zero);
            var end = new Tile(new Vector2Int(2, 0));
            tilemap.SetTile(start);
            tilemap.SetTile(end);
            ServiceLocator.Register(tilemap);

            var pawn = new PawnController
            {
                Owner = PawnOwner.Player,
                TilemapHelper = { AnchorTile = start }
            };

            var ability = new MoveAbility();
            var data = new PawnStrategyData
            {
                Parameters = new MoveDirectionParams { Direction = MovementDirection.Forward },
                Potency = 2
            };
            ability.Initialize(data);

            bool result = false;
            ability.Play(pawn, s => result = s);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(pawn.MoveCalled, Is.True);
                Assert.That(pawn.LastMoveTile, Is.EqualTo(end));
                Assert.That(pawn.ExecutedStrategies, Is.SameAs(pawn.Data.OnMoveStrategies));
            });
        }
    }

    public class JumpAbilityTests
    {
        [Test]
        public void ModifyMove_JumpsOverOccupied()
        {
            var tilemap = new TilemapController();
            var start = new Tile(Vector2Int.zero);
            var block = new Tile(new Vector2Int(1, 0)) { IsOccupied = true };
            var end = new Tile(new Vector2Int(2, 0));
            tilemap.SetTile(start);
            tilemap.SetTile(block);
            tilemap.SetTile(end);
            ServiceLocator.Register(tilemap);

            var pawn = new PawnController
            {
                Owner = PawnOwner.Player,
                TilemapHelper = { AnchorTile = start }
            };

            var ability = new JumpAbility();
            var next = block;
            ability.ModifyMove(pawn, ref next);

            Assert.That(next, Is.EqualTo(end));
        }
    }

    public class FlyAbilityTests
    {
        [Test]
        public void ModifyMove_FliesOverTiles()
        {
            var tilemap = new TilemapController();
            var start = new Tile(Vector2Int.zero);
            var block1 = new Tile(new Vector2Int(1, 0)) { IsOccupied = true };
            var block2 = new Tile(new Vector2Int(2, 0)) { IsOccupied = true };
            var end = new Tile(new Vector2Int(3, 0));
            tilemap.SetTile(start);
            tilemap.SetTile(block1);
            tilemap.SetTile(block2);
            tilemap.SetTile(end);
            ServiceLocator.Register(tilemap);

            var pawn = new PawnController
            {
                Owner = PawnOwner.Player,
                TilemapHelper = { AnchorTile = start }
            };

            var ability = new FlyAbility();
            var data = new PawnStrategyData { Potency = 3 };
            ability.Initialize(data);
            var next = block1;
            ability.ModifyMove(pawn, ref next);

            Assert.That(next, Is.EqualTo(end));
        }
    }

    public class ChargeAbilityTests
    {
        [Test]
        public void Play_KnockbacksOnCollision()
        {
            var tilemap = new TilemapController();
            var start = new Tile(Vector2Int.zero);
            var enemyTile = new Tile(new Vector2Int(1, 0)) { IsOccupied = true, Pawn = new PawnController() };
            tilemap.SetTile(start);
            tilemap.SetTile(enemyTile);
            ServiceLocator.Register(tilemap);

            var pawn = new PawnController
            {
                Owner = PawnOwner.Player,
                TilemapHelper = { AnchorTile = start }
            };

            var ability = new ChargeAbility();
            var data = new PawnStrategyData
            {
                Parameters = new ChargeAbilityParams { MovementDistance = 2, KnockbackStrength = 1 },
                Potency = 0
            };
            ability.Initialize(data);

            ability.Play(pawn, _ => { });

            Assert.That(PawnHelper.LastTarget, Is.EqualTo(enemyTile.Pawn));
            Assert.That(PawnHelper.LastMagnitude, Is.EqualTo(1));
        }
    }

    public class CapturePawnAbilityTests
    {
        [Test]
        public void Play_CallsCapture()
        {
            var ability = new CapturePawnAbility();
            var attacker = new PawnController();
            var target = new PawnController { CaptureReturn = true };
            var data = new PawnStrategyData { Potency = 3 };
            ability.Initialize(data);

            bool result = false;
            ability.Play(attacker, target, s => result = s);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(target.CapturePotency, Is.EqualTo(3));
            });
        }
    }

    public class KnockbackAbilityTests
    {
        [Test]
        public void Play_UsesPawnHelper()
        {
            var ability = new KnockbackAbility();
            var attacker = new PawnController { Owner = PawnOwner.Player };
            var target = new PawnController();
            var data = new PawnStrategyData { Potency = 2 };
            ability.Initialize(data);

            ability.Play(attacker, target, ref Unsafe.AsRef(0), _ => { });

            Assert.That(PawnHelper.LastTarget, Is.EqualTo(target));
            Assert.That(PawnHelper.LastMagnitude, Is.EqualTo(2));
        }
    }

    public class GainStatusEffectTests
    {
        [Test]
        public void GetDescription_FormatsKeyword()
        {
            var keyword = new Keyword { FormattedText = "Poison" };
            var status = new StatusEffectData { Keyword = keyword };
            var ability = new GainStatusEffect();
            var data = new PawnStrategyData
            {
                Parameters = new StatusEffectParams { StatusEffect = status },
                Potency = 1
            };
            ability.Initialize(data);
            var description = ability.GetDescription();
            Assert.That(description, Is.EqualTo("Gain Poison 1"));
        }

        [Test]
        public void Play_AppliesEffect()
        {
            var keyword = new Keyword { FormattedText = "Burn" };
            var status = new StatusEffectData { Keyword = keyword };
            var ability = new GainStatusEffect();
            var data = new PawnStrategyData
            {
                Parameters = new StatusEffectParams { StatusEffect = status },
                Potency = 2
            };
            ability.Initialize(data);
            var pawn = new PawnController();

            ability.Play(pawn, _ => { });

            Assert.Multiple(() =>
            {
                Assert.That(pawn.AppliedStatus, Is.EqualTo(status));
                Assert.That(pawn.AppliedStacks, Is.EqualTo(2));
            });
        }
    }

    public class PawnGainDamagePowerTests
    {
        [Test]
        public void Play_AddsModifier()
        {
            var ability = new PawnGainDamagePower();
            var data = new PawnStrategyData { Potency = 3 };
            ability.Initialize(data);
            var pawn = new PawnController();

            ability.Play(pawn, _ => { });

            Assert.That(pawn.Combat.Damage.Modifiers[ability], Is.EqualTo(3));
        }
    }

    public class PawnGameOverTests
    {
        [Test]
        public void Play_EndsGame()
        {
            var ability = new PawnGameOver();
            ability.Play(new PawnController(), _ => { });
            Assert.That(GameManager.Instance.EndRunCalled, Is.True);
        }
    }

    public class PawnWinCombatTests
    {
        [Test]
        public void Play_WinsCombat()
        {
            var ability = new PawnWinCombat();
            ability.Play(new PawnController(), _ => { });
            Assert.That(GameManager.Instance.WinCombatCalled, Is.True);
        }
    }
}
