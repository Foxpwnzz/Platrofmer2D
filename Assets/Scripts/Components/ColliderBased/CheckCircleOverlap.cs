using UnityEditor;
using UnityEngine;
using Scripts.Utils;
using System;
using UnityEngine.Events;
using System.Linq;

namespace Scripts.Components.ColliderBased
{
    public class CheckCircleOverlap : MonoBehaviour
    {
        [SerializeField] private float _radius = 1f;
        [SerializeField] private LayerMask _mask;
        [SerializeField] private OnOverlapEvent _onOverlab;
        [SerializeField] private string[] _tags;
        private readonly Collider2D[] _interactionResult = new Collider2D[10];

        private void OnDrawGizmosSelected()
        {
            Handles.color = HandlesUtils.TransparentRed;
            Handles.DrawSolidDisc(transform.position, Vector3.forward, _radius);
        }

        public void Check()
        {
            var size = Physics2D.OverlapCircleNonAlloc(
              transform.position,
              _radius,
              _interactionResult,
              _mask);

            for (var i = 0; i < size; i++)
            {
                var overlapResult = _interactionResult[i];
                var isInTags = _tags.Any(tag => _interactionResult[i].CompareTag(tag));
                if (isInTags)
                {
                    _onOverlab?.Invoke(_interactionResult[i].gameObject);
                }
            }
        }

        [Serializable]
        public class OnOverlapEvent : UnityEvent<GameObject>
        {
        }
    }
}