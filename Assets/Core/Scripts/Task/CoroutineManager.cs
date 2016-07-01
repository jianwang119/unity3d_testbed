using UnityEngine;
using System.Collections;

namespace Core
{
    public class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _Inst;

        public static CoroutineManager Inst
        {
            get
            {
                if (_Inst == null)
                {
                    GameObject go = new GameObject("CoroutineMgr");
                    _Inst = go.AddComponent<CoroutineManager>();
                }

                return _Inst;
            }
        }
    }
}

