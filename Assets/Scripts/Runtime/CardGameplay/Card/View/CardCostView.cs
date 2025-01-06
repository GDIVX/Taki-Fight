using System.Collections.Generic;
using Runtime.CardGameplay.ManaSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Card.View
{
    public class CardCostView : MonoBehaviour
    {
        [SerializeField] private Image _prefab;
        private readonly Queue<Image> _imagePool = new Queue<Image>();
        private readonly List<Image> _activeImages = new List<Image>();

        public void Draw(List<Mana> cost)
        {
            // Deactivate all active images and return them to the pool
            foreach (var image in _activeImages)
            {
                image.gameObject.SetActive(false);
                _imagePool.Enqueue(image);
            }

            _activeImages.Clear();

            // Ensure we have enough images in the pool
            while (_imagePool.Count < cost.Count)
            {
                var newImage = Instantiate(_prefab, transform);
                newImage.gameObject.SetActive(false);
                _imagePool.Enqueue(newImage);
            }

            // Assign icons and activate the required images
            foreach (var mana in cost)
            {
                var image = _imagePool.Dequeue();

                image.sprite = mana.Icon;
                image.gameObject.SetActive(true);
                _activeImages.Add(image);
            }
        }
    }
}