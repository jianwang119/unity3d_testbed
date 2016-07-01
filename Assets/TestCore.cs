using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCore: MonoBehaviour 
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "test"))
        {
            TestTask();
        }
    }
    IEnumerator<Core.WaitFor> Test()
    {
        Core.Logger.Log("wait for next update 0");
        yield return new Core.WaitForNextUpdate();
        Core.Logger.Log("wait for next update 1");

        Core.Logger.Log("wait for seconds 0");
        yield return new Core.WaitForSeconds(3);
        Core.Logger.Log("wait for seconds 1");

        Core.Logger.Log("add TestInTest task");
        yield return Core.TaskManager.Inst.StartTask(TestInTest());

        int id = 0;
        while(true)
        {
            Core.Logger.LogFormat("id: {0}", id++);
            yield return null;

            if (id > 10)
                break;
        }
    }

    IEnumerator<Core.WaitFor> TestInTest()
    {
        Core.Logger.Log("wait for seconds 0 in TestInTest");
        yield return new Core.WaitForSeconds(3);
        Core.Logger.Log("wait for seconds 1 in TestInTest");
    }
    Core.Task TestTask()
    {
        Core.Logger.Log("add test task");
        return Core.TaskManager.Inst.StartTask(Test());
    }
}
