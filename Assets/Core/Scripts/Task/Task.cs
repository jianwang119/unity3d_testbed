using System;

namespace Core
{
    public abstract class Task : WaitFor
    {
        public bool IsCancelled { get; set; }
    }
}

