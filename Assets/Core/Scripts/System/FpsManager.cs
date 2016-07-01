using UnityEngine;
using System.Collections;
using System.Threading;

namespace Core
{
    public class FpsManager
    {
        private static FpsManager _Inst = new FpsManager();

        public static FpsManager Inst { get { return _Inst; } }

        public const int FPS_LIMITS = 30;

        // TODO
        public bool IsBusy { get; set; }

        public float Fps { get; set; }

        private int FpsCount = 0;
        private float FpsStartTime = 0;
        private float FPSSleepLimits = 0;

        FpsManager()
        {
            IsBusy = false;
        }

        public void OnUpdate(float deltaTime)
        {
            FpsCount++;

            float time = Time.time;
            if (time - FpsStartTime >= 0.5f)
            {
                Fps = (float)FpsCount / (time - FpsStartTime);
                FpsStartTime = time;
                FpsCount = 0;
            }
        }

        public void TryLimitFps(float deltaTime)
        {
            float diff = deltaTime - (float)1.0f / FPS_LIMITS;

            if (diff > 0)
            {
                FPSSleepLimits = Mathf.Max(0f, FPSSleepLimits - Mathf.Max(0.001f, diff * 0.3f));
            }
            else
            {
                FPSSleepLimits += Mathf.Max(0.001f, -diff * 0.3f);
            }

            if (FPSSleepLimits > 0)
            {
                Thread.Sleep((int)(FPSSleepLimits * 1000));
            }
         }
    }

}
