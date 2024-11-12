using System.Collections;
using UnityEngine;

namespace Scripts.Creatures
{
    public abstract class Patrol : MonoBehaviour
    {
        public abstract IEnumerator DoPatrol();
    }
}
