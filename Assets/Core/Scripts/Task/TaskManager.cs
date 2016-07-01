using System;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class TaskManager
    {
        private static TaskManager _Inst = new TaskManager();

        public static TaskManager Inst { get { return _Inst; } }

        private class TaskInternal : Task
        {
            public IEnumerator<WaitFor> Enumerator { get; set; }

            public bool IsDone { get; set; }

            public TaskInternal(IEnumerator<WaitFor> e)
            {
                Enumerator = e;
            }

            public override bool IsFinish()
            {
                return IsDone;
            }
        }

        private List<TaskInternal> taskListFront = new List<TaskInternal>();
        private List<TaskInternal> taskListBack = new List<TaskInternal>();

        private bool DoTask(TaskInternal task)
        {
            IEnumerator<WaitFor> e = task.Enumerator;

            bool bNextDo = true;

            if (!FpsManager.Inst.IsBusy)
            {
                try
                {
                    if (e.Current == null || e.Current.IsFinish())
                    {
                        if (!e.MoveNext())
                        {
                            bNextDo = false;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception);
                }
            }

            return bNextDo;
        }

        public Task StartTask(IEnumerator<WaitFor> e)
        {
            TaskInternal task = new TaskInternal(e);
            taskListFront.Add(task);
            return task;
        }

        public void CancelTask(Task task)
        {
            TaskInternal t = task as TaskInternal;
            if (t != null && !t.IsDone)
            {
                t.IsCancelled = true;
                t.IsDone = true;
            }
        }

        public void OnUpdate(float deltaTime)
		{
            List<TaskInternal> taskListTmp = taskListFront;
            taskListFront = taskListBack;
            taskListBack = taskListTmp;

            for (int i = 0; i < taskListTmp.Count; i++)
			{
                TaskInternal task = taskListTmp[i];

				if (!task.IsCancelled)
				{
                    if (DoTask(task))
					{
                        taskListFront.Add(task);
					}
					else
					{
                        task.IsDone = true;
					}
				}
			}
            taskListTmp.Clear();
        }
    }
}

