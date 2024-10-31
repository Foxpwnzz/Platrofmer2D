using Scripts.Components;
using Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;

namespace Scripts
{

    public class Hero : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private float _damageJumpSpeed;
        [SerializeField] private int _damage;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _groundCheckRadius;
        [SerializeField] private Vector3 _groundCheckPositionDelta;
        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _interactionLayer;
        [SerializeField] private float _longFallThreshold = -15f; // Порог скорости для долгого прыжка
        [SerializeField] private AnimatorController _armed;
        [SerializeField] private AnimatorController _disarmed;
        [SerializeField] private CheckCircleOverlap _attackRange;

        [Space] [Header("Particles")]
        [SerializeField] private SpawnComponent _footStepParticles;
        [SerializeField] private SpawnComponent _jumpParticles;
        [SerializeField] private SpawnComponent _fallParticles; // партикл приземления
        [SerializeField] private ParticleSystem _hitParticles;

        private Collider2D[] _interactionResult = new Collider2D[1];
        private Rigidbody2D _rigidbody;
        private Vector2 _direction;
        private Animator _animator;
        private Transform _platformTransform; // Ссылка на движущуюся платформу
        private bool _isOnMovingPlatform; // Флаг, находится ли герой на движущейся платформе
        private Vector3 _platformPreviousPosition; // Предыдущее положение платформы для расчета смещения


        private bool _isGrounded;
        private bool _allowDoubleJump;
        private bool _hasDoubleJumped; // Флаг для отслеживания двойного прыжка
        private float _maxFallSpeed; // Для отслеживания максимальной скорости падения

        private static readonly int IsGroundKey = Animator.StringToHash("is-ground");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int IsRunningKey = Animator.StringToHash("is-running");
        private static readonly int Hit = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");

        private int _coins;
        private bool _isArmed;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        private void Update()
        {
            _animator.SetBool(IsRunningKey, _direction.x != 0);
            _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);
        }

        private void FixedUpdate()
        {
            var xVelocity = _direction.x * _speed;
            var yVelocity = CalculateYVelocity();

            // Если герой стоит на движущейся платформе, добавляем её скорость
            if (_isOnMovingPlatform && _platformTransform != null)
            {
                Vector3 platformMovement = _platformTransform.position - _platformPreviousPosition;

                // Если герой не двигается в сторону, то платформа влияет на его скорость
                if (_direction.x == 0)
                {
                    _rigidbody.velocity = new Vector2(platformMovement.x / Time.fixedDeltaTime, yVelocity);
                }
                else
                {
                    // Если герой двигается, учитываем только его собственное направление
                    _rigidbody.velocity = new Vector2(xVelocity, yVelocity);
                }

                // Обновляем предыдущее положение платформы
                _platformPreviousPosition = _platformTransform.position;
            }
            else
            {
                _rigidbody.velocity = new Vector2(xVelocity, yVelocity);
            }


            bool wasGrounded = _isGrounded;
            _isGrounded = IsGrounded();

            // Отслеживаем максимальную скорость падения
            if (_rigidbody.velocity.y < _maxFallSpeed)
            {
                _maxFallSpeed = _rigidbody.velocity.y;
            }

            // Проверка на приземление после двойного прыжка или долгого прыжка
            if (!wasGrounded && _isGrounded)
            {
                if (_hasDoubleJumped)
                {
                    SpawnFallDust();
                }
                else if (_maxFallSpeed <= _longFallThreshold) // Если был долгий прыжок
                {
                    SpawnFallDust();
                }

                // Сбрасываем флаг двойного прыжка и максимальной скорости при приземлении
                _hasDoubleJumped = false;
                _allowDoubleJump = true;
                _maxFallSpeed = 0; // Сбрасываем максимальную скорость
            }

            _animator.SetBool(IsGroundKey, _isGrounded);

            UpdateSpriteDirection();
        }

        private float CalculateYVelocity()
        {
            var yVelocity = _rigidbody.velocity.y;
            var isJumpPressing = _direction.y > 0;

            if (_isGrounded) _allowDoubleJump = true;
            if (isJumpPressing)
            {
                yVelocity = CalculateJumpVelocity(yVelocity);
            }
            else if (_rigidbody.velocity.y > 0)
            {
                yVelocity *= 0.5f;
            }

            return yVelocity;
        }

        private float CalculateJumpVelocity(float yVelocity)
        {
            var isFalling = _rigidbody.velocity.y <= 0.001f;
            if (!isFalling) return yVelocity;

            if (_isGrounded)
            {
                yVelocity += _jumpSpeed;
            }
            else if (_allowDoubleJump)
            {
                yVelocity = _jumpSpeed;
                _allowDoubleJump = false;
                _hasDoubleJumped = true; // Устанавливаем флаг двойного прыжка
            }

            return yVelocity;
        }

        private void UpdateSpriteDirection()
        {
            if (_direction.x > 0)
            {
                transform.localScale = Vector3.one;  
            }
            else if (_direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private bool IsGrounded()
        {
            var hit = Physics2D.CircleCast(transform.position + _groundCheckPositionDelta, _groundCheckRadius, Vector2.down, 0, _groundLayer);
            if (hit.collider != null && hit.collider.CompareTag("MovingPlatforms"))
            {
                if (!_isOnMovingPlatform)
                {
                    _platformPreviousPosition = hit.collider.transform.position; // Устанавливаем начальное положение платформы
                }
                _isOnMovingPlatform = true;
                _platformTransform = hit.collider.transform;
            }
            else
            {
                _isOnMovingPlatform = false;
                _platformTransform = null;
            }
            return hit.collider != null;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = IsGrounded() ? HandlesUtils.TransparentGreen : HandlesUtils.TransparentRed;
            Handles.DrawSolidDisc(transform.position + _groundCheckPositionDelta, Vector3.forward, _groundCheckRadius);
        }
#endif

        public void SaySomething()
        {
            Debug.Log("Something!!!");
        }

        public void AddCoins(int coins)
        {
            _coins += coins;
            Debug.Log("Монеты добавлены: " + coins + ". Всего монет: " + _coins);
        }
        public void TakeDamage()
        {
            _animator.SetTrigger(Hit);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _damageJumpSpeed);
            if (_coins > 0)
            {
                SpawnCoins();
            }
            else
            {
                Debug.Log("No coins to spawn!");
            }
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_coins, 5);
            _coins -= numCoinsToDispose;

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.gameObject.SetActive(true);
            _hitParticles.Play();
        }

        public void Ineract()
        {
            var size = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                _interactionRadius,
                _interactionResult,
                _interactionLayer);

            for (int i = 0; i < size; i++)
            {
                var interactable = _interactionResult[i].GetComponent<InteractableComponent>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }

        public void Attack()
        {
            if (!_isArmed)
            {
                return;
            }
            _animator.SetTrigger(AttackKey);
        }

        public void OnAttacking()
        {
            var gos = _attackRange.GetObjectsInRange();
            foreach (var go in gos)
            {
                var hp = go.GetComponent<HealthComponent>();
                if (hp != null && go.CompareTag("Enemy"))
                {
                    hp.ApplyDamage(_damage);
                }
            }
        }

        public void ArmHero()
        {
            _isArmed = true;
            _animator.runtimeAnimatorController = _armed;
        }

        public void SpawnFootDust()
        {
            _footStepParticles.Spawn();
        }


        public void SpawnJumpDust()
        {
            _jumpParticles.Spawn();
        }

        public void SpawnFallDust()
        {
            _fallParticles.Spawn();
        }
    }
}