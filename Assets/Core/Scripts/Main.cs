using UnityEngine;
using System.Collections;

namespace Core
{
    public class Main : MonoBehaviour
    {
        public static Main Inst;

        void Awake()
        {
            Main.Inst = this;
            Object.DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 30;
        }

        void OnDestroy()
        {
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;

            FpsManager.Inst.OnUpdate(deltaTime);
            TaskManager.Inst.OnUpdate(deltaTime);

            FpsManager.Inst.TryLimitFps(deltaTime);
        }
    }
}

