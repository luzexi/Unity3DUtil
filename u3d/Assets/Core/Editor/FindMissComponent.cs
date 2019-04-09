
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class FindMissComponent
{
    [MenuItem("Assets/FindMissComponent",false,100)]
    static private void Find()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if(string.IsNullOrEmpty(path))
        {
            Debug.LogError("请选择目标prefab");
            return;
        }

        string dataPath = Application.dataPath;
        Debug.Log("开始查找:");
        if(!string.IsNullOrEmpty(path))
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(obj!=null)
            {
                Component[] cps = obj.GetComponentsInChildren<Component>(true);
                for(int i=0;i<cps.Length;++i)
                {
                    if(cps[i]!=null)
                    {
                        SerializedObject sobj = new SerializedObject(cps[i]);
                        var iter = sobj.GetIterator();
                        while(iter.NextVisible(true))
                        {
                            if(iter.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                if(iter.objectReferenceValue == null && iter.objectReferenceInstanceIDValue != 0)
                                {
                                    //引用是空但是有实例ID，说明miss了
                                    Debug.Log(cps[i].gameObject.name);
                                }
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("查找完成");
    }  
    
    [MenuItem("Assets/FindMissComponent", true)]
    static private bool VFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }

}