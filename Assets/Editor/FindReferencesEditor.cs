using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class FindReferencesEditor : EditorWindow
{
	[MenuItem("Assets/Find References", false, 10)]
	public static void ShowFindReferencesWindows()
	{
		FindReferencesEditor window = (FindReferencesEditor)FindReferencesEditor.GetWindow(typeof(FindReferencesEditor), false, "查找资源引用");
		window.selectionObj = Selection.activeObject;
		window.targetDir = AssetDatabase.LoadAssetAtPath<Object> ("Assets");
		window.Show();
	}

	public Object selectionObj = null;
	public Object targetDir = null;

	private string targetPath = null;
	private Vector2 mScroll;

	public void OnGUI()
	{
		GUILayout.BeginVertical();
		selectionObj = EditorGUILayout.ObjectField("待查找资源:", selectionObj, typeof(Object), false);
		targetDir = EditorGUILayout.ObjectField("待查找目录:", targetDir, typeof(Object), false);
		if (targetDir != null) 
		{
			targetPath = Application.dataPath.Replace("/Assets", "/") + AssetDatabase.GetAssetPath (targetDir);
			if (!Directory.Exists (targetPath)) 
			{
				Debug.LogFormat ("Cann't find reference in target {0}, target is not directory!", targetPath);
				targetDir = null;
				targetPath = null;
			}
		}
		if (GUILayout.Button ("查找")) 
		{
			if (targetDir == null) 
			{
				Debug.Log ("Cann't find reference , target is null!");
			} 
			else 
			{
				Find ();
			}
		}
		if (matches.Count != 0) 
		{
			if (GUILayout.Button ("清除结果")) 
			{
				matches.Clear ();
			}
			EditorGUILayout.LabelField (string.Format("查找到引用：{0}个",matches.Count));
			mScroll = GUILayout.BeginScrollView(mScroll);
			foreach (var f in matches) 
			{
				EditorGUILayout.ObjectField("", AssetDatabase.LoadAssetAtPath<Object>(f), typeof(Object), false);
			}
			GUILayout.EndScrollView ();
		}

		GUILayout.EndVertical ();
	}


	private List<string> matches = new List<string>();

 	private void Find()
	{
		EditorSettings.serializationMode = SerializationMode.ForceText;
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (!string.IsNullOrEmpty(path))
		{
			string guid = AssetDatabase.AssetPathToGUID(path);
			string withoutExtensions = GetWithoutExtensions (selectionObj);//"*.prefab*.unity*.mat*.asset";
			string[] files = Directory.GetFiles(targetPath, "*.*", SearchOption.AllDirectories)
				.Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower()) 
					&& !s.Contains("/Assets/Editor/")
					&& !s.Contains("/Assets/Res/Movies/")
				).ToArray() ;
			int startIndex = 0;

			matches.Clear ();

			EditorApplication.update = delegate()
			{
				bool isCancel = false;

				if(files.Length > 0)
				{
					string file = files[startIndex];

					isCancel = EditorUtility.DisplayCancelableProgressBar(string.Format("匹配资源中: {0} / {1}", startIndex, files.Length), file, (float)startIndex / (float)files.Length);

					if (Regex.IsMatch(File.ReadAllText(file), guid))
					{
						matches.Add(GetRelativeAssetsPath(file));
						Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
					}
				}

				startIndex++;
				if (isCancel || startIndex >= files.Length)
				{
					EditorUtility.ClearProgressBar();
					EditorApplication.update = null;
					startIndex = 0;
					Debug.Log("匹配结束，忽略了 \"/Assets/Editor\" 和 \"/Assets/Res/Movies\" 目录");
				}

			};
		}
	}

	[MenuItem("Assets/Find References", true)]
	static private bool VFind()
	{
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		return (!string.IsNullOrEmpty(path));
	}

	static private string GetRelativeAssetsPath(string path)
	{
		return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
	}

	static private string GetWithoutExtensions(Object o)
	{
		string s = "*.prefab*.unity*.mat*.asset*.controller";
		if (o is Shader) {
			s = "*.mat";
		} else if (o is Material) {
			s = "*.prefab*.asset";
		} else if (o is MonoBehaviour) {
			s = "*.prefab";
		}
		return s;
	}
}