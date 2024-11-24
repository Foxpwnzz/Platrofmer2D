using Scripts.Components.ColliderBased;
using System.Collections;
using UnityEngine;

namespace Scripts.Creatures.Mobs.Patrolling
{
    public class PlatformPatrol : Patrol
    {
        [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] private LayerCheck __obstacleCheck;
        [SerializeField] private int _direction;
        [SerializeField] private Creature _creature;

        public override IEnumerator DoPatrol()
        {
            while (enabled)
            {
                if (_groundCheck.IsTouchingLayer && !__obstacleCheck.IsTouchingLayer)
                {
                    _creature.SetDirection(new Vector2(_direction, 0));
                }
                else
                {
                    _direction = -_direction;
                    _creature.SetDirection(new Vector2(_direction, 0));
                }

                yield return null;
            }
        }
    }
}