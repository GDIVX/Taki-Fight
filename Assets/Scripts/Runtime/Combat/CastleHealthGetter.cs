using CodeMonkey.HealthSystemCM;
using UnityEngine;
using Utilities;

namespace Runtime.Combat
{
    public class CastleHealthGetter : MonoBehaviour, IGetHealthSystem
    {
        public HealthSystem GetHealthSystem()
        {
            return ServiceLocator.Get<CastleHealthManager>().GetHealthSystem();
        }
    }
}
