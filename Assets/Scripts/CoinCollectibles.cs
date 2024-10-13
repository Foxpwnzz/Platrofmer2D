using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class CoinCollectibles : MonoBehaviour
    {
        private int _totalPoints = 0; // Общая сумма очков
        private int _silverCoinsCount = 0; // Количество серебряных монет
        private int _goldCoinsCount = 0; // Количество золотых монет

        // Метод для добавления очков
        private void AddPoints(int points, bool isSilver)
        {
            _totalPoints += points;

            if (isSilver)
            {
                _silverCoinsCount++; // Увеличиваем количество серебряных монет
            }
            else
            {
                _goldCoinsCount++; // Увеличиваем количество золотых монет
            }

            // Выводим данные в консоль
            Debug.Log("Total points: " + _totalPoints);
            Debug.Log("Silver coins collected: " + _silverCoinsCount);
            Debug.Log("Gold coins collected: " + _goldCoinsCount);
        }

        // Публичные методы для добавления очков за монеты
        public void AddSilverCoin()
        {
            AddPoints(1, true); // Серебряная монета всегда даёт 1 очко
        }

        public void AddGoldCoin()
        {
            AddPoints(10, false); // Золотая монета всегда даёт 10 очков
        }
    }
}
