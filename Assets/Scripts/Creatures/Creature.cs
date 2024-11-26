using Scripts.Components.Audio;
using Scripts.Components.ColliderBased;
using Scripts.Components.GoBased;
using UnityEngine;


namespace Scripts.Creatures
{
    public class Creature : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private bool _invertScale;
        [SerializeField] private float _speed;
        [SerializeField] protected float _jumpSpeed;
        [SerializeField] private float _damageVelocity;

        [Header("Checkers")]
        [SerializeField] protected LayerMask _groundLayer;
        [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] private CheckCircleOverlap _attackRange;
        [SerializeField] protected SpawnListComponent _particles;

        protected Rigidbody2D Rigidbody;
        protected Vector2 Direction;
        protected Animator Animator;
        protected PlaySoundsComponent Sounds;
        protected bool IsGrounded;
        private bool _isJumping;

        private Transform _platformTransform;
        private Vector3 _platformPreviousPosition;
        private bool _isOnMovingPlatform;
        private bool _isJumpingOffPlatform;

        private static readonly int IsGroundKey = Animator.StringToHash("is-ground");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int IsRunningKey = Animator.StringToHash("is-running");
        private static readonly int HitKey = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            Sounds = GetComponent<PlaySoundsComponent>();
        }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
        }

        protected virtual void Update() 
        {
            IsGrounded = _groundCheck.IsTouchingLayer;
            _isOnMovingPlatform = IsOnPlatform();
        }


        private void FixedUpdate()
        {
            var xVelocity = Direction.x * _speed;
            var yVelocity = CalculateYVelocity();
            if (_isOnMovingPlatform && _platformTransform != null)
            {
                Vector3 platformMovement = _platformTransform.position - _platformPreviousPosition;

                if (Direction.x == 0)
                {
                    Rigidbody.velocity = new Vector2(platformMovement.x / Time.fixedDeltaTime, yVelocity);
                }
                else
                {
                    Rigidbody.velocity = new Vector2(xVelocity, yVelocity);
                }

                _platformPreviousPosition = _platformTransform.position;
            }
            else
            {
                Rigidbody.velocity = new Vector2(xVelocity, yVelocity);
            }

            Animator.SetBool(IsGroundKey, IsGrounded);
            Animator.SetBool(IsRunningKey, Direction.x != 0);
            Animator.SetFloat(VerticalVelocityKey, Rigidbody.velocity.y);

            UpdateSpriteDirection(Direction);
        }

        protected virtual float CalculateYVelocity()
        {
            var yVelocity = Rigidbody.velocity.y;
            var isJumpPressing = Direction.y > 0;

            if (IsGrounded)
            {
                _isJumping = false;
            }

            if (isJumpPressing)
            {
                _isJumping = true;

                var isFalling = Rigidbody.velocity.y <= 0.001f;
                yVelocity = isFalling ? CalculateJumpVelocity(yVelocity) : yVelocity;
            }
            else if (Rigidbody.velocity.y > 0 && _isJumping)
            {
                yVelocity *= 0.5f;
            }

            return yVelocity;
        }

        protected virtual float CalculateJumpVelocity(float yVelocity)
        {
            if (IsGrounded)
            {
                yVelocity += _jumpSpeed;
                DoJumpVfx();
            }

            return yVelocity;
        }

        protected void DoJumpVfx()
        {
            _particles.Spawn("Jump");
            Sounds.Play("Jump");
        }

        public void UpdateSpriteDirection(Vector2 direction)
        {
            var multiplier = _invertScale ? -1 : 1;
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(multiplier, 1, 1); 
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1 * multiplier, 1, 1);
            }
        }

        protected virtual bool IsOnPlatform()
        {
            var hit = Physics2D.CircleCast(transform.position, 1f, Vector2.down, 0.1f, _groundLayer);
            if (hit.collider != null && hit.collider.CompareTag("MovingPlatforms"))
            {
                if (!_isOnMovingPlatform)
                {
                    _platformPreviousPosition = hit.collider.transform.position;
                }
                _isOnMovingPlatform = true;
                _platformTransform = hit.collider.transform;
            }
            else
            {
                _isOnMovingPlatform = false;
                _platformTransform = null;
            }
            return _isOnMovingPlatform;
        }

        public virtual void TakeDamage()
        {
            Animator.SetTrigger(HitKey);
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, _damageVelocity);
        }

        public virtual void Attack()
        {
            Animator.SetTrigger(AttackKey);
            _attackRange.Check();
            _particles.Spawn("Slash");
            Sounds.Play("Melee");
        }
    }
}