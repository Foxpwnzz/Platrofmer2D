using Scripts.Model;
using Scripts.Model.Data;
using UnityEngine;
using UnityEngine.Events;
using static Scripts.Model.Data.InventoryData;

namespace Scripts.Components.Interactions
{
    public class RequireItemComponent : MonoBehaviour
    {
        [SerializeField] private InventoryItemData[] _required;
        [SerializeField] private bool _removeAfterUse;

        [SerializeField] private UnityEvent _onSuccess;
        [SerializeField] private UnityEvent _onFail;

        public void Check()
        {
            var session = FindObjectOfType<GameSession>();
            var areAllRequirementsMet = true;
            foreach (var item in _required)
            {
                var numItems = session.Data.Inventory.Count(item.Id);
                if(numItems < item.Value)
                    areAllRequirementsMet = false;
            }
            if (areAllRequirementsMet)
            {
                if (_removeAfterUse)
                {
                    foreach(var item in _required)
                        session.Data.Inventory.Remove(item.Id, item.Value);

                }

                _onSuccess?.Invoke();
            }
            else
            {
                _onFail?.Invoke();
            }
        }
    }
}