using System.Linq;
using DG.Tweening;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Deck;
using Runtime.Combat.Tilemap;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.View
{
    public class CardViewMediator
    {
        private CardView _view;
        private CardController _controller;

        public void Bind(CardController controller, CardView view)
        {
            _controller = controller;
            _view = view;

            _view.ToggleBlockRaycast(true);

            _view.OnHoverEnter += HandleHoverEnter;
            _view.OnHoverExit += HandleHoverExit;
            _controller.IsPlayable.OnValueChanged += OnPlayableChanged;

            Refresh(); // single source of truth for view sync
        }

        public void Refresh()
        {
            if (!_controller || !_view) return;

            // Title, Image, Cost
            _view.SetTitle(_controller.Data.Title);
            _view.SetImage(_controller.Data.Image);
            _view.SetCost(_controller.Instance.Cost);

            // Description
            var builder = new DescriptionBuilder();
            _view.UpdateDescription(builder.Build(_controller));

            // Highlight
            _view.SetHighlight(_controller.IsPlayable.Value);

            // Summon stats
            var firstStrategy = _controller.Data.PlayStrategies.FirstOrDefault();
            if (firstStrategy.PlayStrategy is SummonUnitPlay summon)
            {
                _view.ShowPawnStats(summon.Pawn);
            }
            else
            {
                _view.HidePawnStats();
            }
        }


        public void Unbind()
        {
            if (_view)
            {
                _view.OnHoverEnter -= HandleHoverEnter;
                _view.OnHoverExit -= HandleHoverExit;
            }

            if (_controller)
            {
                _controller.IsPlayable.OnValueChanged -= OnPlayableChanged;
            }

            _view = null;
            _controller = null;
        }

        private void OnPlayableChanged(bool enabled)
        {
            _view.SetHighlight(enabled);
        }

        private void HandleHoverEnter()
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            tilemap.AllTiles()
                .Where(tile => _controller.IsAskingForTile(tile))
                .ToList()
                .ForEach(tile => tile.View.Highlight(_controller.Data.Highlight));

            var hand = ServiceLocator.Get<HandController>();
            hand.Cards.ForEach(card =>
            {
                if (card.View != _view)
                    card.View.SetHighlight(_controller.IsAskingForCard(card));
            });
        }

        private void HandleHoverExit()
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            tilemap.AllTiles()
                .Where(tile => _controller.IsAskingForTile(tile))
                .ToList()
                .ForEach(tile => tile.View.ClearHighlight());

            var hand = ServiceLocator.Get<HandController>();
            hand.Cards.ForEach(card => { card.View.SetHighlight(card.IsPlayable.Value); });
        }

        public void ShowMessage(string message)
        {
            if (_view != null)
            {
                _view.ShowMessage(message);
            }
        }

        public void Discard()
        {
            _view.OnDiscard();
        }

        public void Consume()
        {
            _view.OnConsume();
        }

        public void Draw()
        {
            _view.OnDraw();
        }

        public Tween SetPosition(Vector3 position, Vector3 rotation, float duration)
        {
            return _view.Move(position, rotation, duration);
        }


        public void ResetLayoutState()
        {
            if (!_view) return;
            _view.MoveToRoot(0.2f);
        }

        public void SetBlockRaycast(bool enabled)
        {
            if (!_view) return;
            _view.ToggleBlockRaycast(enabled);
        }

        public Tween SetRoot(Vector3 position, Vector3 rotation, float duration)
        {
            _view.SetRoot(position, rotation);
            return _view.MoveToRoot(duration);
        }
    }
}