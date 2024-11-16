using UnityEngine;
using UnityEngine.InputSystem;
using Scripts.Creatures;
using UnityEngine.Rendering;

namespace Scripts
{

    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;

        public void OnMovement(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<Vector2>();
            _hero.SetDirection(direction);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _hero.Interact();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _hero.Attack();
            }
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _hero.Throw();
            }
        }
    }
}