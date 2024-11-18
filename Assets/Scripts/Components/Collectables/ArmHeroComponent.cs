using UnityEngine;
using Scripts.Creatures.Hero;

namespace Scripts.Components.Collectables
{
    public class ArmHeroComponent : MonoBehaviour
    {
        public void ArmHero(GameObject go)
        {
            var hero = go.GetComponent<Hero>();
            if (hero != null)
            {
                hero.ArmHero();
            }
        }
    }
}
