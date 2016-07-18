using System;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class TaskManager
    {
        private static TaskManager _Inst = new TaskManager();

        public static TaskManager Inst { get { return _Inst; } }

        private List<Task> taskListFront = new List<Task>();
        private List<Task> taskListBack = new List<Task>();

        private bool DoTask(Task task)
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
            Task task = new Task(e);
            taskListFront.Add(task);
            return task;
        }

        public void CancelTask(Task task)
        {
            Task t = task as Task;
            if (t != null && !t.IsDone)
            {
                t.IsCancelled = true;
                t.IsDone = true;
            }
        }

        public void OnUpdate(float deltaTime)
		{
            List<Task> taskListTmp = taskListFront;
            taskListFront = taskListBack;
            taskListBack = taskListTmp;

            for (int i = 0; i < taskListTmp.Count; i++)
			{
                Task task = taskListTmp[i];

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

