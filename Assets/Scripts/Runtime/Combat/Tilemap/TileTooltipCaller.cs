using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.UI.Tooltip;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Tilemap
{
    [RequireComponent(typeof(TileView))]
    public class TileTooltipCaller : TooltipCallerBase<CompoundTooltipSource>
    {
        [SerializeField, Required] private TileView _tileView;
        [SerializeField] private Color _tileTooltipColor, _pawnTooltipColor;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            var compoundSource = new CompoundTooltipSource()
            {
                TooltipSources = new List<IContentTooltipSource>()
            };
            var tileTooltip = new ContentTooltipSource()
            {
                //TODO: return when we have terrain types
                Header = "Plains",
                Subtitle = _tileView.Tile.Owner switch
                {
                    TileOwner.All => "Neutral Tile",
                    TileOwner.None => "Neutral Tile",
                    TileOwner.Player => "Friendly Tile",
                    TileOwner.Enemy => "Corrupted Tile",
                    _ => throw new ArgumentOutOfRangeException()
                },
                Content = "This is a plains tile.",
                BackgroundColor = _tileTooltipColor
            };

            compoundSource.TooltipSources.Add(tileTooltip);

            if (_tileView.Tile.IsOccupied)
            {
                var pawn = _tileView.Tile.Pawn;
                var descriptionBuilder = new DescriptionBuilder();

                var pawnTooltip = new ContentTooltipSource()
                {
                    Header = pawn.Data.Title,
                    Subtitle = pawn.Data.Tribe,
                    Content = descriptionBuilder.AsSummon(pawn.Data),
                    BackgroundColor = _pawnTooltipColor
                };

                var statusEffects = pawn.GetStatusEffects();
                var keywords = statusEffects.Select(e => e.Keyword).Distinct();

                compoundSource.TooltipSources.Add(pawnTooltip);
                compoundSource.TooltipSources.AddRange(keywords);
            }


            ShowTooltip(compoundSource);
        }
    }
}