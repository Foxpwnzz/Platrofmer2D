using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Components.CutScenes
{
    public class TimerComponent : MonoBehaviour
    {
        [SerializeField] private TimerData[] _timers;

        public void SetTimer(int index)
        {
            var timer = _timers[index];

            StartCoroutine(StartTimer(timer));
            
        }

        private IEnumerator StartTimer(TimerData timer)
        {
            yield return new WaitForSeconds(timer.Delay);
            timer.OnTimerUp?.Invoke();
        }


        [Serializable]
        public class TimerData
        {
            public float Delay;
            public UnityEvent OnTimerUp;
        }
    }
}
