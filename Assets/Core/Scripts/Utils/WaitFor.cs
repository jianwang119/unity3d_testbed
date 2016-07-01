﻿using System;

namespace Core
{
    public abstract class WaitFor
    {
        public abstract bool Finish();
    }

    public class WaitForNextUpdate : WaitFor
    {
        private bool _isDone = false;

        public override bool Finish()
        {
            bool ret = _isDone;
            _isDone = true;
            return ret;
        }

        public void Reset()
        {
            _isDone = false;
        }
    }

    public class WaitForSeconds : WaitFor
    {
        private float seconds = 0;
        private float startTime = 0;
        
        public WaitForSeconds(float s)
        {
            seconds = s;
            startTime = UnityEngine.Time.time;
        }

        public override bool Finish()
        {
            if (UnityEngine.Time.time - startTime > seconds)
            {
                return true;
            }
            return false;
        }
    }
}