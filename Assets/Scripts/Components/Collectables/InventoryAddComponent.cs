using Scripts.Model.Data;
using Scripts.Model.Definitions;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Components.Collectables
{
    public class InventoryAddComponent : MonoBehaviour
    {
        [InventoryId] [SerializeField] private string _id;
        [SerializeField] private int _count;

        public void Add(GameObject go)
        {
            var hero = go.GetInterface<ICanAddInInventory>();
            hero?.AddInInventory(_id, _count);  
        }

    }
}
