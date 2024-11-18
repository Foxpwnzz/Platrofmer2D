using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Components.Health
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private int _maxHp; // Максимального здоровья
        [SerializeField] private float _invulnerability = 1.0f; // Время неуязвимости
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] private UnityEvent _onDie;

        private bool _isInvulnerable;

        public int MaxHp
        {
            get => _maxHp;
            set
            {
                _maxHp = value;
                _health = Mathf.Clamp(_health, 0, _maxHp); // Обновляем текущее здоровье в пределах нового максимума
            }
        }

        public void ModifyHealth(int healthDelta)
        {
            if (_health <= 0) return;
            if (_isInvulnerable && healthDelta < 0) return;
            _health = Mathf.Clamp(_health + healthDelta, 0, _maxHp); // Ограничиваем текущее здоровье

            if (healthDelta < 0)
            {
                _onDamage?.Invoke();
                StartInvulnerability(); // Запускаем неуязвимость
            }

            if (healthDelta > 0)
            {
                _onHeal?.Invoke();
            }

            if (_health <= 0)
            {
                _onDie?.Invoke();
            }
        }

        private void StartInvulnerability()
        {
            if (_invulnerability > 0)
            {
                _isInvulnerable = true;
                StartCoroutine(InvulnerabilityCoroutine());
            }
        }

        private IEnumerator InvulnerabilityCoroutine()
        {
            yield return new WaitForSeconds(_invulnerability);
            _isInvulnerable = false;
        }

        public void AddOnDieListener(UnityAction action)
        {
            _onDie.AddListener(action);
        }
    }
} 