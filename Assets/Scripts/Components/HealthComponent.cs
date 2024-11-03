using System;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;        // Текущее количество здоровья
        [SerializeField] private int _maxHealth;     // Максимальное количество здоровья
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private HealthChangeEvent _onChange;

        // Геттеры для текущего и максимального здоровья
        public int Health => _health;
        public int MaxHealth => _maxHealth;

        // Метод для нанесения урона
        public void ApplyDamage(int damageValue)
        {
            _health -= damageValue;
            _onChange?.Invoke(_health);

            _onDamage?.Invoke();

            if (_health <= 0)
            {
                _onDie?.Invoke();
            }
        }

        // Метод для лечения
        public void Heal(int healingAmount)
        {
            _health += healingAmount;
            _onChange?.Invoke(_health);

            if (_health > _maxHealth)
            {
                _health = _maxHealth;  // Ограничиваем здоровье максимумом
            }
        }

        public void AddOnDieListener(UnityAction action)
        {
            _onDie.AddListener(action);
        }


        public void SetHealth(int health)
        {
            _health = health;
        }

        [Serializable]
        public class HealthChangeEvent : UnityEvent<int>
        {

        }
    }
} 