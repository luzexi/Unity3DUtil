using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class RemoveDebugLog
{
    private static string whiteFile = "DebugWrap.cs";
    private static string scriptsFile = "scripts";

    [MenuItem("Assets/Remove Debug Log", false, 100)]
    static public void StartRemove()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        // string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string path = Path.Combine(Application.dataPath,scriptsFile);
        if (!string.IsNullOrEmpty(path))
        {
            List<string> withoutExtensions = new List<string>() {".cs" };
            //从选择的文件夹中拿到所有文件
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            int startIndex = 0;

            if(files.Length == 0)
            {
                Debug.Log("没有需要处理的文件！");
                return;
            }

            EditorApplication.update = delegate()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("清除Debug.Log...代码中。。。", file, (float)startIndex / (float)files.Length);
                
                if(!Regex.IsMatch(file,scriptsFile))
                {
                    //scripts以外的文件不处理
                    Debug.Log("scripts以外的文件不处理");
                }
                else if(Regex.IsMatch(file,whiteFile))
                {
                    //白名单不处理
                    Debug.Log("DebugWrap.cs 白名单不处理");
                }
                else if(NoteDebugLog(file))
                {
                    Debug.Log(file, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(file), typeof(Object)));
                }

                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    Debug.Log("匹配结束");
                }
            };
        }
    }

    // [MenuItem("Assets/Remove Debug Log", true)]
    // static private bool VFind()
    // {
    //     string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //     return (!string.IsNullOrEmpty(path) && Directory.Exists(path));
    // }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    static private bool DeleteDebugLog(string file)
    {
        string fileContent = "";
        string[] fileLines = File.ReadAllLines(file);
        bool hasWrite = false;

        StringBuilder sBuilder = new StringBuilder();
        
        for(int i=0;i<fileLines.Length;++i)
        {

            if(fileLines[i].IndexOf("Debug.Log")!=-1)
            {
                hasWrite = true;


                continue;
            }
            else
            {
                sBuilder.Append(fileLines[i]);
                sBuilder.Append("\n");
            }
        }
        fileContent = sBuilder.ToString();

        StreamWriter writer = new StreamWriter(file,false);
        writer.Write(fileContent);
        writer.Flush();
        writer.Close();
        return hasWrite;
    }


    static private bool NoteDebugLog(string file)
    {
        string debugPattern = @"((Debug.Log)).*?(?=(;))";
        string fileText = File.ReadAllText(file);
        if (Regex.IsMatch(fileText, debugPattern))
        {
            Regex rgx = new Regex(debugPattern);
            string result = rgx.Replace(fileText,"");
            StreamWriter writer = new StreamWriter(file,false);
            writer.Write(result);
            writer.Flush();
            writer.Close();
            return true;
        }
        return false;
    }

}