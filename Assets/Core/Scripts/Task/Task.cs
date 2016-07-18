using System;
using System.Collections.Generic;

namespace Core
{
    public class Task : WaitFor
    {
        public bool IsCancelled { get; set; }

        public IEnumerator<WaitFor> Enumerator { get; set; }

        public bool IsDone { get; set; }

        public Task(IEnumerator<WaitFor> e)
        {
            Enumerator = e;
        }

        public override bool IsFinish()
        {
            return IsDone;
        }        
    }
}

