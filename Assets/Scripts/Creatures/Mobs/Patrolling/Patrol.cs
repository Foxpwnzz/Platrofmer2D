using System.Collections;
using UnityEngine;

namespace Scripts.Creatures.Mobs.Patrolling
{
    public abstract class Patrol : MonoBehaviour
    {
        public abstract IEnumerator DoPatrol();
    }
}
