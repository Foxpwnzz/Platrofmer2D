﻿using Scripts.Model;
using Scripts.Utils;
using UnityEditor.Animations;
using UnityEngine;
using Scripts.Components.ColliderBased;
using Scripts.Components.Health;

namespace Scripts.Creatures.Hero
{
    public class Hero : Creature
    {
        [SerializeField] private CheckCircleOverlap _interactionCheck;
        [SerializeField] private LayerCheck _wallCheck;

        [SerializeField] private float _slamDownVelocity;

        [Space]
        [Header("Weapon")]
        [SerializeField] private int _swordCount;
        [SerializeField] private Cooldown _throwCoolDown;
        [SerializeField] private AnimatorController _armed;
        [SerializeField] private AnimatorController _disarmed;

        [Space]
        [Header("Particles")]
        [SerializeField] private ParticleSystem _hitParticles;

        private static readonly int IsThrowKey = Animator.StringToHash("throw");
        private static readonly int IsOnWall = Animator.StringToHash("is-on-wall");

        private bool _allowDoubleJump;
        private bool _isOnWall;
        private float _defaultGravityScale;

        private GameSession _session;
        private HealthComponent _health;

        protected override void Awake()
        {
            base.Awake();
            _defaultGravityScale = Rigidbody.gravityScale;
        }

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            var health = GetComponent<HealthComponent>();

            health.MaxHp = _session.Data.MaxHp;
            health.ModifyHealth(_session.Data.Hp);
            _swordCount = _session.Data.SwordCount;
        }

        public void OnHealthChanged(int currentHealth)
        {
            _session.Data.Hp = currentHealth;
        }

        public void IncreaseMaxHealth(int amount)
        {
            _health.MaxHp += amount;
            _session.Data.MaxHp = _health.MaxHp;
        }

        protected override void Update()
        {
            base.Update();

            var moveToSameDirection = Direction.x * transform.lossyScale.x > 0;
            if (_wallCheck.IsTouchingLayer && moveToSameDirection)
            {
                _isOnWall = true;
                Rigidbody.gravityScale = 0;
            }
            else
            {
                _isOnWall = false;
                Rigidbody.gravityScale = _defaultGravityScale;
            }

            Animator.SetBool(IsOnWall, _isOnWall);
        }

        protected override float CalculateYVelocity()
        {
            var isJumpPressing = Direction.y > 0;

            if (IsGrounded || _isOnWall)
            {
                _allowDoubleJump = true;
            }

            if (!isJumpPressing && _isOnWall)
            {
                return 0f;
            }

            return base.CalculateYVelocity();
        }

        protected override float CalculateJumpVelocity(float yVelocity)
        {
            if (!IsGrounded && _allowDoubleJump && !_isOnWall)
            {
                _particles.Spawn("Jump");
                _allowDoubleJump = false;
                return _jumpSpeed;

            }

            return base.CalculateJumpVelocity(yVelocity);
        }

        public void AddCoins(int coins)
        {
            _session.Data.Coins += coins;
            Debug.Log("Монеты добавлены: " + coins + ". Всего монет: " + _session.Data.Coins);
        }
        public override void TakeDamage()
        {
            base.TakeDamage();
            if (_session.Data.Coins > 0)
            {
                SpawnCoins();
            }
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_session.Data.Coins, 5);
            _session.Data.Coins -= numCoinsToDispose;

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.gameObject.SetActive(true);
            _hitParticles.Play();
        }

        public void Interact()
        {
            _interactionCheck.Check();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.IsInLayer(_groundLayer))
            {
                var contact = other.contacts[0];
                if (contact.relativeVelocity.y >= _slamDownVelocity)
                {
                    _particles.Spawn("SlamDown");
                }
            }
        }

        public override void Attack()
        {
            if (!_session.Data.IsArmed) return;
            base.Attack();
        }

        public void ArmHero()
        {
            _session.Data.SwordCount++;
            _swordCount = _session.Data.SwordCount;
            _session.Data.IsArmed = true;
            UpdateHeroWeapon();
        }

        private void UpdateHeroWeapon()
        {
            Animator.runtimeAnimatorController = _session.Data.IsArmed ? _armed : _disarmed;
        }

        public void OnDoThrow()
        {
            if (_swordCount > 0)
            {
                _swordCount--;
                _session.Data.SwordCount = _swordCount;
                if (_swordCount == 0)
                {
                    _session.Data.IsArmed = false;
                    UpdateHeroWeapon();
                }
                _particles.Spawn("Throw");
            }
        }

        public void Throw()
        {
            if (!_session.Data.IsArmed || _session.Data.SwordCount <= 0)
            {
                Debug.LogWarning("Нельзя бросить меч, его нет!");
                return;
            }

            if (_throwCoolDown.IsReady)
            {
                Animator.SetTrigger(IsThrowKey);
                _throwCoolDown.Reset();
            }
        }
    }
}