using UnityEngine;

namespace Scripts.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;
        private PlayerData _initialData; // Сохранение состояния на начало уровня
        public PlayerData Data => _data;

        private void Awake()
        {
            if (IsSessionExit())
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);
                SaveInitialState(); // Сохраняем начальное состояние при загрузке
            }
        }

        private bool IsSessionExit()
        {
            var sessions = FindObjectsOfType<GameSession>();
            foreach (var gameSession in sessions)
            {
                if (gameSession != this)
                    return true;
            }

            return false;
        }

        // Сохраняем начальное состояние уровня
        public void SaveInitialState()
        {
            _initialData = new PlayerData
            {
                Coins = _data.Coins,
                Hp = _data.Hp,
                IsArmed = _data.IsArmed
            };
        }

        // Восстанавливаем начальное состояние
        public void RestoreInitialState()
        {
            if (_initialData != null)
            {
                _data.Coins = _initialData.Coins;
                _data.Hp = _initialData.Hp;
                _data.IsArmed = _initialData.IsArmed;
            }
        }
    }
}