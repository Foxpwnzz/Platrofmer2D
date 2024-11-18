using UnityEngine;

namespace Scripts.Components.GoBased
{
    public class SpawnComponent : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private GameObject _prefab;

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            var instance = Instantiate(_prefab, _target.position, Quaternion.identity);
            instance.transform.localScale = _target.lossyScale;
            instance.SetActive(true);
        }
    }
}