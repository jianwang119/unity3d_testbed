using UnityEngine;
using System.Collections;
using UnityEditor.UI;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(MyImage), true)]
[CanEditMultipleObjects]
public class MyImageEditor : ImageEditor
{

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
