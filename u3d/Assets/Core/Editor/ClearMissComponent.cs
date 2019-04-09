
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;



public class ClearMissComponent
{

    [MenuItem("Assets/ClearMissingScripts",false,100)]
    static void ClearMissingScripts()
    {
        string dataPath = Application.dataPath;
        List<string> withoutExtensions = new List<string>() { ".prefab"};
        string[] files = Directory.GetFiles(dataPath, "*.*", SearchOption.AllDirectories)
            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        int startIndex = 0;

        EditorApplication.update = delegate()
        {
            string file = files[startIndex];

            bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

            DeleteNullScript(file);

            startIndex++;
            if (isCancel || startIndex >= files.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;
                Debug.Log("匹配结束");
            }

        };


        // EditorSettings.serializationMode = SerializationMode.ForceText;
        // string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        // if(string.IsNullOrEmpty(path))
        // {
        //     Debug.LogError("请选择目标prefab");
        //     return;
        // }

        // DeleteNullScript(path);

        // AssetDatabase.SaveAssets();
        // Debug.Log("ClearMissingScripts Success");
    }


    private static void DeleteNullScript(string path)
	{
		bool isNull = false;
		string s = File.ReadAllText(path);
 
		Regex regBlock = new Regex("MonoBehaviour");
 
		// 以"---"划分组件
		string[] strArray = s.Split(new string[] { "---" }, StringSplitOptions.RemoveEmptyEntries);
 
		for (int i = 0; i < strArray.Length; i++)
		{
			string blockStr = strArray[i];
 
			if (regBlock.IsMatch(blockStr))
			{
				// 模块是 MonoBehavior
				// Match guidMatch = Regex.Match(blockStr, "m_Script: {fileID: (.*), guid: (?<GuidValue>.*?), type:");
                Match guidMatch = Regex.Match(blockStr, "m_Script: {fileID: (?<FileID>.*?)}");
				if (guidMatch.Success)
				{
					// 获取 MonoBehavior的guid
					string fileID = guidMatch.Groups["FileID"].Value;
					//Debug.Log("Guid:" + guid);
 
                    // string _path = AssetDatabase.GUIDToAssetPath(guid);
					// if (string.IsNullOrEmpty(_path))
                    if(fileID == "0")
					{
						// 工程中无此脚本 空脚本！！！
						Debug.Log(path+" 有空脚本");
						isNull = true;
 
						// 删除操作
 
						// 删除MonoScript
						s = s.Replace("---" + blockStr, "");
 
						Match idMatch = Regex.Match(blockStr, "!u!(.*) &(?<idValue>.*?)\r");
						if (idMatch.Success)
						{
							// 获取 MonoBehavior的guid
							string id = idMatch.Groups["idValue"].Value;
 
							// 删除MonoScript的引用
							Regex quote = new Regex("  - (.*): {fileID: " + id + "}");
							s = quote.Replace(s, "");
						}
 
					}
 
				}
 
			}
 
 
		}
 
		if (isNull)
		{
			// 有空脚本 写回prefab
			File.WriteAllText(path, s);
 
			// 打印Log
			Debug.Log(path);
		}
	}


    /// <summary>
    /// 删除预设上面丢失的脚本
    /// </summary>
    // static void ClearMissingScripts()
    // {
    //     //测试一个
    //     List<GameObject> prefabList = new List<GameObject>(Selection.gameObjects);//GetAllUIPrefab();
    //     //List<GameObject> prefabList = GetAllGameObjects(FindItemType.FindObj);

    //     int prefabCount = prefabList.Count;
    //     Debug.Log("prefabCount = " + prefabCount);

    //     for (int i = prefabCount - 1; i >= 0; i--)
    //     {
    //         DeleteMissingScripts(prefabList[i]);
    //     }

    //     AssetDatabase.SaveAssets();
    //     Debug.Log("ClearMissingScripts Success");
    // }

    // static void DeleteMissingScripts(GameObject obj)
    // {
    //     Debug.Log("name is :" + obj.name);
    //     Regex guidRegex = new Regex("m_Script: {fileID: (.*), guid: (?<GuidValue>.*?), type:");
    //     Regex fileRegex = new Regex("--- !u!(?<groupNum>.*?) &(?<fileID>.*?)\r\n");
    //     int fileStrLenght = 30;
    //     string filePath = AssetDatabase.GetAssetPath(obj);
    //     string s = File.ReadAllText(filePath);
    //     string groupSpilChar = "---";
    //     string fileStr = "";
    //     bool isChange = false;
    //     MatchCollection matchList = guidRegex.Matches(s);
    //     if (matchList != null)
    //     {
    //         for (int i = matchList.Count - 1; i >= 0; i--)
    //         {

    //             string guid = matchList[i].Groups["GuidValue"].Value;
    //             if (AssetDatabase.GUIDToAssetPath(guid) == "")
    //             {
    //                 isChange = true;
    //                 int startIndex = s.LastIndexOf(groupSpilChar, matchList[i].Index);
    //                 int endIndex = s.IndexOf(groupSpilChar, matchList[i].Index);

    //                 Match fileMatch = fileRegex.Match(s.Substring(startIndex, fileStrLenght));
    //                 fileStr = "- " + fileMatch.Groups["groupNum"].Value + ": {fileID: " + fileMatch.Groups["fileID"].Value + "}\r\n  ";

    //                 s = s.Replace(s.Substring(startIndex, endIndex - startIndex), "");
    //                 s = s.Replace(fileStr, "");
    //             }
    //         }
    //     }
    //     if (isChange)
    //     {
    //         File.WriteAllText(filePath, s);
    //         Debug.Log(obj.name + " missing scripts destory success!");
    //     }
    // }

    // [MenuItem("Assets/ClearMissingScripts", true)]
    // static private bool VFind()
    // {
    //     string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //     return (!string.IsNullOrEmpty(path));
    // }

}