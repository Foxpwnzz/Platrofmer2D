using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class SpriteAnimation : MonoBehaviour
    {
        [SerializeField] private string _idleAnimationName = "Idle";        // Имя для Idle анимации
        [SerializeField] private Sprite[] _idleSprites;                     // Спрайты для Idle анимации
        [SerializeField] private bool _idleLoop = true;                     // Зацикливание Idle анимации

        [SerializeField] private string _eventsAnimationName;               // Имя для Events анимации
        [SerializeField] private Sprite[] _eventsSprites;                   // Спрайты для Events анимации
        [SerializeField] private bool _eventsLoop = false;                  // Зацикливание Events анимации

        [SerializeField] private int _frameRate = 10;                       // Количество кадров в секунду
        [SerializeField] private UnityEvent _onComplete;                    // Событие по завершению анимации

        private SpriteRenderer _renderer;
        private float _secondPerFrame;
        private int _currentSpriteIndex;
        private float _nextFrameTime;

        private string _currentAnimationName;
        private Sprite[] _currentSprites;
        private bool _currentLoop;
        private bool _isPlaying = true;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _secondPerFrame = 1f / _frameRate;

            // Начинаем с анимации Idle
            SetClip(_idleAnimationName);
        }

        private void Update()
        {
            if (!_isPlaying || _nextFrameTime > Time.time) return;

            if (_currentSpriteIndex >= _currentSprites.Length)
            {
                if (_currentLoop)
                {
                    _currentSpriteIndex = 0;  // Зацикливаем
                }
                else
                {
                    _isPlaying = false;
                    _onComplete?.Invoke();  // Вызываем событие по завершению
                    return;
                }
            }

            _renderer.sprite = _currentSprites[_currentSpriteIndex];
            _nextFrameTime += _secondPerFrame;
            _currentSpriteIndex++;
        }

        public void SetClip(string name)
        {
            if (name == _idleAnimationName)
            {
                _currentAnimationName = _idleAnimationName;
                _currentSprites = _idleSprites;
                _currentLoop = _idleLoop;
            }
            else if (name == _eventsAnimationName)
            {
                _currentAnimationName = _eventsAnimationName;
                _currentSprites = _eventsSprites;
                _currentLoop = _eventsLoop;
            }
            else
            {
                Debug.LogWarning($"Анимация с именем {name} не найдена!");
                return;
            }

            _currentSpriteIndex = 0;
            _isPlaying = true;
            _nextFrameTime = Time.time + _secondPerFrame;
        }

        // Обработка столкновения с игроком
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) // Проверяем, что столкновение произошло с игроком
            {
                SetClip(_eventsAnimationName); // Переключаем анимацию из события
            }
        }
    }
}