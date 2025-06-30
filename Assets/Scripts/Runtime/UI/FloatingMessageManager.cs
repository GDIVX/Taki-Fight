using DamageNumbersPro;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.View
{
    public class FloatingMessageManager : MonoService<FloatingMessageManager>
    {
        [SerializeField] DamageNumber _genericMessageTextPrefab, _errorMessageTextPrefab, _warningMessageTextPrefab;

        public void ShowMessage(string message, Transform parent)
        {
            ShowMessage(message, parent, _genericMessageTextPrefab);
        }

        public void ShowMessage(string message)
        {
            ShowMessage(message, transform, _genericMessageTextPrefab);
        }

        public void ShowError(string message, Transform parent)
        {
            ShowMessage(message, parent, _errorMessageTextPrefab);
        }

        public void ShowWarning(string message, Transform parent)
        {
            ShowMessage(message, parent, _warningMessageTextPrefab);
        }

        private void ShowMessage(string message, Transform parent, DamageNumber prefab)
        {
            prefab.Spawn(parent.position, message, parent);
        }

        private void ShowMessage(string message, Vector3 position, DamageNumber prefab)
        {
            prefab.Spawn(position, message);
        }

        public void ShowError(string message, Vector3 position)
        {
            ShowMessage(message, position, _errorMessageTextPrefab);
        }

        public void ShowError(string message)
        {
            ShowMessage(message);
        }
    }
}