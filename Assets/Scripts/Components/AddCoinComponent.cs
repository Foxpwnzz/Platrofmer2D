using UnityEngine;
using Scripts.Creatures;

namespace Scripts.Components
{
    public class AddCoinComponent : MonoBehaviour
    {
        [SerializeField] private int _numCoins;
        private Hero _hero; 

        private void Start () 
        { 
            _hero = FindObjectOfType<Hero>();
        }

        public void Add()
        {
            _hero.AddCoins(_numCoins);
        }
    }
}