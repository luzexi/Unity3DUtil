

using System;
using System.IO;

//  CFile.cs
//  Author: Lu Zexi
//  2015-01-17



//file tool
public class CFile
{
    //get files by dir
    public string[] GetFiles(string dir , string param = "*")
    {
        if(string.IsNullOrEmpty(dir)) return null;

        DirectoryInfo info = new DirectoryInfo(dir);
        FileInfo[] fileInfos = info.GetFiles(param , SearchOption.AllDirectories);
        string[] filenames = new string[fileInfos.Length];
        for(int i = 0 ; i<filenames.Length ; i++)
        {
            filenames[i] = fileInfos[i].FullName;
        }
        return filenames;
    }
}