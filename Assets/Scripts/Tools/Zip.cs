using ICSharpCode.SharpZipLib.Zip;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Zip
{
    public static void UnZipFile(string zipFilePath, string unZipDir)
    {
        ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath));
        ZipEntry zipEntry;
        while ((zipEntry = zipStream.GetNextEntry()) != null)
        {
            string fileName = unZipDir + "/" + zipEntry.Name;
            Debug.Log(fileName);
            string dirName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
            if (fileName.EndsWith("/"))
            {
                if (!Directory.Exists(fileName)) Directory.CreateDirectory(fileName);
            }
            else
            {
                FileStream streamWriter = File.Create(fileName);
                int size = 2048;
                byte[] data = new byte[size];
                while (true)
                {
                    size = zipStream.Read(data, 0, data.Length);
                    if (size > 0) streamWriter.Write(data, 0, size);
                    else break;
                }
                streamWriter.Close();
            }
        }
        zipStream.Close();
    }
}
