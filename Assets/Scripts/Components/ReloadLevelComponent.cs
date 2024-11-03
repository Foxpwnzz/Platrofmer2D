using Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Components
{
    public class ReloadLevelComponent : MonoBehaviour
    {
        public void Reload()
        {
            var session = FindObjectOfType<GameSession>();
            if (session != null)
            {
                session.RestoreInitialState(); // Восстанавливаем состояние на начало уровня
                Destroy(session.gameObject);   // Удаляем сессию
            }

            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name); // Перезагружаем сцену
        }
    }
}