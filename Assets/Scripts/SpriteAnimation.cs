using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]

    public class SpriteAnimation : MonoBehaviour
    {
        [Serializable]
        public class AnimationEvent
        {
            public string Name;                   // Название анимации
            public Sprite[] Sprites;              // Спрайты для анимации
            public bool Loop = false;             // Зацикливание анимации
            public UnityEvent onComplete;         // Событие по завершению анимации
        }

        [SerializeField] private List<AnimationEvent> _animationEvents;    // Список всех анимаций
        [SerializeField] private string _triggerAnimationName;
        [SerializeField] private int _frameRate = 10;                      // Кадры в секунду

        private SpriteRenderer _renderer;
        private float _secondPerFrame;
        private int _currentSpriteIndex;
        private float _nextFrameTime;

        private AnimationEvent _currentEvent;
        private bool _isPlaying = true;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _secondPerFrame = 1f / _frameRate;

            // Начинаем с анимации Idle
            if (_animationEvents.Count > 0)
            {
                SetClip(_animationEvents[0].Name);
            }
        }

        private void Update()
        {
            if (!_isPlaying || _nextFrameTime > Time.time) return;

            if (_currentSpriteIndex >= _currentEvent.Sprites.Length)
            {
                if (_currentEvent.Loop)
                {
                    _currentSpriteIndex = 0;  // Зацикливаем
                }
                else
                {
                    enabled = _isPlaying = false;
                    _currentEvent.onComplete?.Invoke();  // Вызываем событие завершения для текущей анимации
                    return;
                }
            }

            _renderer.sprite = _currentEvent.Sprites[_currentSpriteIndex];
            _nextFrameTime += _secondPerFrame;
            _currentSpriteIndex++;
        }

        public void SetClip(string name)
        {
            _currentEvent = _animationEvents.Find(eventData => eventData.Name == name);

            if (_currentEvent != null)
            {
                _currentSpriteIndex = 0;
                enabled = _isPlaying = true;
                _nextFrameTime = Time.time + _secondPerFrame;
            }
            else
            {
                Debug.LogWarning($"Анимация с именем {name} не найдена!");
            }
        }

        // Обработка столкновения с игроком
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) // Проверяем, что столкновение произошло с игроком
            {
                SetClip(_triggerAnimationName); // Переключаем на анимацию, указанную в инспекторе
            }
        }
    }
}