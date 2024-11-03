using UnityEngine;

namespace Scripts.Components
{
    public class DamageComponent : MonoBehaviour
    {
        [SerializeField] private int _damage = 1;           // Количество урона
        [SerializeField] private int _healingAmount = 5;    // Количество восстанавливаемого здоровья
        [SerializeField] private bool _isHealingObject = false; // Флаг, обозначающий, что это объект для лечения
        [SerializeField] private float _damageCooldown = 1f; // Задержка между нанесением урона в секундах

        private float _lastDamageTime;  // Время последнего нанесенного урона

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!_isHealingObject && other.gameObject.CompareTag("Player"))
            {
                ApplyDamage(other.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isHealingObject && other.CompareTag("Player"))
            {
                ApplyHealing(other.gameObject);
            }
        }

        // Метод для нанесения урона с проверкой задержки
        private void ApplyDamage(GameObject target)
        {
            if (Time.time - _lastDamageTime >= _damageCooldown)
            {
                var healthComponent = target.GetComponent<HealthComponent>();
                if (healthComponent != null)
                {
                    healthComponent.ApplyDamage(_damage);
                    Debug.Log($"Урон от spikes: текущие жизни {healthComponent.Health}/{healthComponent.MaxHealth}");
                    _lastDamageTime = Time.time; // Обновляем время последнего нанесенного урона
                }
            }
        }

        // Метод для восстановления здоровья
        private void ApplyHealing(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.Heal(_healingAmount);
                Debug.Log($"Лечение от heal potion: текущие жизни {healthComponent.Health}/{healthComponent.MaxHealth}");
            }
        }
    }
}