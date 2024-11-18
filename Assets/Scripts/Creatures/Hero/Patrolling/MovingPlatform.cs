using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Creatures.Hero.Patrolling
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private List<Transform> points;
        [SerializeField] private float moveSpeed = 2;
        [SerializeField] private Transform platform;

        int goalPoint;

        private void Update()
        {
            MoveToNextPoint();
        }

        private void MoveToNextPoint()
        {
            platform.position = Vector2.MoveTowards(platform.position, points[goalPoint].position, Time.deltaTime * moveSpeed);

            if (Vector2.Distance(platform.position, points[goalPoint].position) < 0.1f)
            {
                if (goalPoint == points.Count - 1)
                {
                    goalPoint = 0;
                }
                else
                    goalPoint++;
            }
        }
    }
}
