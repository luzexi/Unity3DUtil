
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


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

    // create dir if not exist
    public static void CreateDirectoryIfNotExist(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    // copy folder to another place
    public static void CopyFolderAll(string fromPath_, string toPath_)
    {
        DirectoryInfo source = new DirectoryInfo(fromPath_);
        DirectoryInfo target = new DirectoryInfo(toPath_);
        CopyAll(source, target);
    }

    static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        if (source.FullName.ToLower() == target.FullName.ToLower())
        {
            return;
        }

        // Check if the target directory exists, if not, create it.
        if (Directory.Exists(target.FullName) == false)
        {
            Directory.CreateDirectory(target.FullName);
        }

        // Copy each file into it's new directory.
        foreach (FileInfo fi in source.GetFiles())
        {
            //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
            fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
    }

    // write binary file
    public static void WriteBinaryFile(string path, byte[] data)
    {
        string dir = Path.GetDirectoryName(path);
        CreateDirectoryIfNotExist(dir);

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            fs.Write(data, 0, data.Length);
            fs.Close();
        }
    }

    // read binary file
    public static byte[] ReadBinaryFile(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();
            return data;
        }
    }

    // write string to file
    public static void WriteStringToFile(string path, string content)
    {
        FileStream fs = new FileStream(path, FileMode.Create);

        // convert to binary & write file
        byte[] bytes = new System.Text.UTF8Encoding().GetBytes(content);
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
    }

    // read string to file
    public static string ReadStringFromFile(string path)
    {
        // read file
        FileStream fs = new FileStream(path, FileMode.Open);

        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        fs.Close();

        // convert binary to string
        string s = new System.Text.UTF8Encoding().GetString(bytes);

        return s;
    }
}