using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Scripts.Creatures.Weapons
{
    public class SinusoidalProjectile : BaseProjectile
    {
        [SerializeField] private float _frequency = 1f;
        [SerializeField] private float _amplitute = 1f;

        private float _originalY;
        private float _time;

        protected override void Start()
        {
            base.Start();
            _originalY = Rigidbody.position.y;
        }

        private void FixedUpdate()
        {
            var position = Rigidbody.position;
            position.x += Direction * _speed;
            position.y = _originalY + Mathf.Sin(_time * _frequency) * _amplitute;
            Rigidbody.MovePosition(position);
            _time += Time.fixedDeltaTime;
        }
    }
}