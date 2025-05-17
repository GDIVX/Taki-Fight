using Runtime.Combat;
using Runtime.Events;
using Utilities;

namespace Runtime.UI
{
    public class MageHealthUI : HealthBarUI
    {
        private void Awake()
        {
            ServiceLocator.Get<EventBus>().Subscribe<CastleHealthManager.CastlePawnInitializedEvent>(OnPawnInitialized);
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<EventBus>()
                .Unsubscribe<CastleHealthManager.CastlePawnInitializedEvent>(OnPawnInitialized);
        }

        private void OnPawnInitialized(CastleHealthManager.CastlePawnInitializedEvent initializedEvent)
        {
            SetUp(initializedEvent.HealthSystem);
        }
    }
}