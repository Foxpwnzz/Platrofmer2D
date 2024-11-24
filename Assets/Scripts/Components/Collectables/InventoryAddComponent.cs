using Scripts.Creatures.Hero;
using Scripts.Model.Definitions;
using UnityEngine;

namespace Scripts.Components.Collectables
{
    public class InventoryAddComponent : MonoBehaviour
    {
        [InventoryId] [SerializeField] private string _id;
        [SerializeField] private int _count;

        public void Add(GameObject go)
        {
            var hero = go.GetComponent<Hero>();
            if (hero != null)
                hero.AddInInvetory(_id, _count);
        }

    }
}
