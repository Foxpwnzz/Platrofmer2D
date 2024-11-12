using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.Model
{
    [Serializable]
    public class PlayerData
    {
        public int Coins;
        public int Hp;
        public int MaxHp; // Максимум hp 
        public bool IsArmed;
    }
}
