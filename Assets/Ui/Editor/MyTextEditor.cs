using UnityEngine;
using System.Collections;
using UnityEditor.UI;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(MyText), true)]
[CanEditMultipleObjects]
public class MyTextEditor : UnityEditor.UI.GraphicEditor
{
    SerializedProperty m_Text;
    SerializedProperty m_FontData;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_Text = serializedObject.FindProperty("m_Text");
        m_FontData = serializedObject.FindProperty("m_FontData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Text);
        EditorGUILayout.PropertyField(m_FontData);
        AppearanceControlsGUI();
        RaycastControlsGUI();
        serializedObject.ApplyModifiedProperties();
    }
}
