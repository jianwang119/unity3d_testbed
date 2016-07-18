using UnityEngine;
using System.Collections;

namespace Core
{
    public class ObjectPool
    {
        private static ObjectPool _Inst = new ObjectPool();

        private GameObject goPool;

        public static ObjectPool Inst { get { return _Inst; } }


    }
}