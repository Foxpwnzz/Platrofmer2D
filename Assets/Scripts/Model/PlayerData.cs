using System;
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
        public int SwordCount; // Количество мечей

        public PlayerData Clone()
        {
            var json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<PlayerData>(json);
        }
    }
}
