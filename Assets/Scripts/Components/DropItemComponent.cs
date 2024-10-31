using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DropItem
{
    public GameObject item;                   // Объект, который будет выпадать
    public int minQuantity = 1;               // Минимальное количество предметов, которые могут выпасть
    public int maxQuantity = 1;               // Максимальное количество предметов, которые могут выпасть
    [Range(0f, 1f)] public float dropChance;  // Шанс выпадения предмета (от 0 до 1)
    public bool alwaysDrop;                   // Галочка для обязательного выпадения
    public bool enabled = true;               // Галочка для включения/выключения выпадения
}

namespace Scripts.Components
{
    public class DropItemComponent : MonoBehaviour
    {
        [SerializeField] private List<DropItem> dropItems; // Список всех дропов с настройками

        private void Start()
        {
            // Подписка на событие _onDie из HealthComponent
            var healthComponent = GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.AddOnDieListener(Drop);
            }
        }

        public void Drop()
        {
            foreach (var dropItem in dropItems)
            {
                // Проверка на включенность выпадения дропа и шанс выпадения, если предмет не обязательно выпадает
                if (!dropItem.enabled) continue;

                if (!dropItem.alwaysDrop && UnityEngine.Random.value > dropItem.dropChance)
                    continue;

                // Генерация случайного количества предметов
                int quantityToDrop = UnityEngine.Random.Range(dropItem.minQuantity, dropItem.maxQuantity + 1);

                // Создание экземпляров предметов и добавление отладочного сообщения
                for (int i = 0; i < quantityToDrop; i++)
                {
                    Instantiate(dropItem.item, transform.position, Quaternion.identity);
                }

                // Отладка: вывод информации о выпавших предметах
                Debug.Log($"Выпал предмет: {dropItem.item.name}, количество: {quantityToDrop}");
            }
        }
    }
}
