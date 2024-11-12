using System.Collections;
using UnityEngine;

namespace Scripts.Creatures
{
    public class EdgePatrol : Patrol
    {
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _groundCheckDistance = 0.5f; // Дистанция для проверки края платформы

        private Creature _creature;
        private bool _movingRight = true;

        private void Awake()
        {
            _creature = GetComponent<Creature>();
        }

        public override IEnumerator DoPatrol()
        {
            while (enabled)
            {
                Vector2 direction = _movingRight ? Vector2.right : Vector2.left;
                _creature.SetDirection(direction);

                // Позиция для проверки платформы чуть впереди и снизу
                Vector3 platformCheckPosition = _groundCheck.position + (Vector3)direction * 0.5f;

                // Проверка на наличие платформы и отсутствие препятствий впереди
                bool isPlatformAhead = Physics2D.CircleCast(platformCheckPosition, 0.2f, Vector2.down, _groundCheckDistance, _groundLayer);
                bool isObstacleAhead = Physics2D.Raycast(transform.position, direction, 0.5f, _groundLayer);

                // Разворот, если нет платформы впереди или есть препятствие
                if (!isPlatformAhead || isObstacleAhead)
                {
                    _movingRight = !_movingRight;
                    _creature.SetDirection(Vector2.zero);
                }

                yield return null;
            }
        }
    }
}