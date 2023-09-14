using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;

namespace Editor
{
    public class ReplaceTheFont : EditorWindow
    {
        private static ReplaceTheFont _window;
    private static List<string> _prefabPathList = new List<string>();
    private static TMP_FontAsset _targetFont;

    [MenuItem("Tools/字体工具")]
    public static void ShowWindow()
    {
        if (_window == null)
        {
            _window = GetWindow(typeof(ReplaceTheFont)) as ReplaceTheFont;
        }
        _window.titleContent = new GUIContent("字体工具");
        _window.Show();
    }
    
    void OnGUI()
    {
        _targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField("替换字体:", _targetFont, typeof(TMP_FontAsset), true);
        if (GUILayout.Button("替换场景中Text的字体"))
        {
            //寻找Hierarchy面板下所有的Text
            var tArray = Resources.FindObjectsOfTypeAll(typeof(TMP_Text));
            foreach (var t in tArray)
            {
                var text = t as TMP_Text;
                Undo.RecordObject(text, text.gameObject.name);
                text.font = _targetFont;
                //设置已改变
                EditorUtility.SetDirty(t);
            }
            Debug.Log("完成");
        }
        
        if (GUILayout.Button("替换预制体中Text的字体"))
        {
            GetFiles(new DirectoryInfo(Application.dataPath), "*.prefab", ref _prefabPathList);
            foreach (var pref in _prefabPathList)
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(pref);
                var t = obj.GetComponentsInChildren<TMP_Text>();
                if (t != null)
                {
                    foreach (var text in t)
                    {
                        Undo.RecordObject(text, text.gameObject.name);
                        text.font = _targetFont;
                        //设置已改变
                        EditorUtility.SetDirty(text);
                    }
                }
            }
            AssetDatabase.SaveAssets();
            Debug.Log("完成");
        }
    }

    /// <summary>
    /// 获得Asset目录下所有预制体对象
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="pattern"></param>
    /// <param name="fileList"></param>
    private static void GetFiles(DirectoryInfo directory, string pattern, ref List<string> fileList)
    {
        if (directory != null && directory.Exists && !string.IsNullOrEmpty(pattern))
        {
            try
            {
                foreach (var info in directory.GetFiles(pattern))
                {
                    var path = info.FullName.ToString();
                    fileList.Add(path.Substring(path.IndexOf("Assets")));
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            foreach (DirectoryInfo info in directory.GetDirectories())
            {
                GetFiles(info, pattern, ref fileList);
            }
        }
    } 
    }
}

