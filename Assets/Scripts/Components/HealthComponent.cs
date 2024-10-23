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

        // Геттеры для текущего и максимального здоровья
        public int CurrentHealth => _health;
        public int MaxHealth => _maxHealth;

        // Метод для нанесения урона
        public void ApplyDamage(int damageValue)
        {
            _health -= damageValue;

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
            if (_health > _maxHealth)
            {
                _health = _maxHealth;  // Ограничиваем здоровье максимумом
            }
        }
    }
}