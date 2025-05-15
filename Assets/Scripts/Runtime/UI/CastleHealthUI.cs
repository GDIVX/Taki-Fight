using Runtime.Combat;
using Runtime.Events;
using Utilities;

namespace Runtime.UI
{
    public class CastleHealthUI : HealthBarUI
    {
        private void Awake()
        {
            ServiceLocator.Get<EventBus>().Subscribe<CastleHealthManager.CastlePawnInitializedEvent>(OnPawnInitialized);
        }

        private void OnPawnInitialized(CastleHealthManager.CastlePawnInitializedEvent initializedEvent)
        {
            SetUp(initializedEvent.HealthSystem);
        }
    }
}