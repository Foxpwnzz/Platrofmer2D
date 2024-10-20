using UnityEngine;

namespace Scripts.Components
{
    public class HealingPotionComponent : MonoBehaviour
    {
        [SerializeField] private int healingAmount = 5;  // Количество восстанавливаемого здоровья

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Проверяем, что столкновение произошло с игроком (героем)
            if (collision.CompareTag("Player"))
            {
                // Получаем компонент здоровья у героя
                var healthComponent = collision.GetComponent<HealthComponent>();
                if (healthComponent != null)
                {
                    // Восстанавливаем здоровье героя
                    healthComponent.Heal(healingAmount);  // Используем значение из HealingPotionComponent
                }
            }
        }
    }
}