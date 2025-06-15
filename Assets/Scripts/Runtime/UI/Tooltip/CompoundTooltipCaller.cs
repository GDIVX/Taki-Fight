using System.Collections.Generic;
using Runtime.Combat.StatusEffects;
using Runtime.Combat.Tilemap;
using Runtime.CardGameplay.Card;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI.Tooltip
{
    [RequireComponent(typeof(TileView))]
    public class CompoundTooltipCaller : TooltipCallerBase
    {
        private TileView _tileView;

        private void Awake()
        {
            _tileView = GetComponent<TileView>();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            var data = BuildData();
            ShowTooltip(data);
        }

        protected void ShowTooltip(CompoundTooltipData data)
        {
            if (data == null) return;
            if (CurrentTooltip == null)
            {
                CurrentTooltip = TooltipPool.GetTooltip();
            }

            if (CurrentTooltip is CompoundTooltipController compound)
            {
                compound.SetTooltip(data);
            }

            this.Timer(_delayTime, () => { CurrentTooltip.ShowTooltip(); });
        }

        private CompoundTooltipData BuildData()
        {
            var compound = ScriptableObject.CreateInstance<CompoundTooltipData>();

            if (_tileView && _tileView.Tile != null)
            {
                // Tile info tooltip could be retrieved from resources or built dynamically
                // For now create a simple instance
                var tileInfo = ScriptableObject.CreateInstance<TooltipData>();
                typeof(TooltipData).GetField("_header", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(tileInfo, $"Owner: {_tileView.Tile.Owner}");
                compound.AddTooltip(tileInfo);

                var pawn = _tileView.Tile.Pawn;
                if (pawn)
                {
                    var pawnTooltip = ScriptableObject.CreateInstance<TooltipData>();
                    var builder = new DescriptionBuilder();
                    typeof(TooltipData).GetField("_header", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.SetValue(pawnTooltip, pawn.Data.Title);
                    typeof(TooltipData).GetField("_description", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.SetValue(pawnTooltip, builder.AsSummon(pawn.Data));
                    compound.AddTooltip(pawnTooltip);

                    var statusHandler = pawn.GetComponent<StatusEffectHandler>();
                    if (statusHandler)
                    {
                        var field = typeof(StatusEffectHandler).GetField("_statusEffects", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (field?.GetValue(statusHandler) is List<IStatusEffect> effects)
                        {
                            foreach (var effect in effects)
                            {
                                var viewField = typeof(StatusEffectListView).GetField("_statusEffectViews", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                if (viewField?.GetValue(statusHandler.GetComponentInChildren<StatusEffectListView>()) is Dictionary<IStatusEffect, StatusEffectView> views
                                    && views.TryGetValue(effect, out var view))
                                {
                                    var tooltipCallerField = typeof(StatusEffectView).GetField("_tooltipCaller", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                    if (tooltipCallerField?.GetValue(view) is TooltipCaller caller)
                                    {
                                        var tooltipDataField = typeof(TooltipCaller).GetField("_tooltipData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                        if (tooltipDataField?.GetValue(caller) is TooltipData td)
                                        {
                                            compound.AddTooltip(td);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return compound;
        }
    }
}
