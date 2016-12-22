using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCore: MonoBehaviour 
{
    Core.Task task;
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "test"))
        {
            if (task != null)
            {
                task.IsCancelled = true;
            }
            task = TestTask();
        }
    }
    IEnumerator<Core.WaitFor> Test()
    {
        Core.Logger.Log("==============================");

        Core.Logger.Log("wait for next update 0");
        yield return new Core.WaitForNextUpdate();
        Core.Logger.Log("wait for next update 1");

        Core.Logger.Log("wait for seconds 0");
        yield return new Core.WaitForSeconds(3);
        Core.Logger.Log("wait for seconds 1");

        Core.Logger.Log("---- add TestInTest task");
        yield return Core.TaskManager.Inst.StartTask(TestInTest());
        Core.Logger.Log("---- end TestInTest task");

        int id = 0;
        while(true)
        {
            Core.Logger.LogFormat("id: {0}", id++);
            yield return null;

            if (id > 10)
                break;
        }

        Core.Logger.Log("==============================");
    }

    IEnumerator<Core.WaitFor> TestInTest()
    {
        Core.Logger.Log("wait for seconds 0 in TestInTest");
        yield return new Core.WaitForSeconds(3);
        Core.Logger.Log("wait for seconds 1 in TestInTest");
    }
    Core.Task TestTask()
    {
        return Core.TaskManager.Inst.StartTask(Test());
    }
}
