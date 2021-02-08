using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

public static class DataStream
{
    static string path = Application.persistentDataPath+ "\\";
    public static void Save<T>(T data, string fileName)
    {
        string dataPath = path + fileName;
        BinaryFormatter test = new BinaryFormatter();

        using (FileStream fs = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            test.Serialize(fs, data);
        }
    }
    public static T Load<T>(string fileName)
    {
        string dataPath = path + fileName;
        T result;
        BinaryFormatter test = new BinaryFormatter();
        using (FileStream fs = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            result = (T)test.Deserialize(fs);
        }
        return result;
    }

    static void AddText(FileStream fs, string value)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info,0,info.Length);
    }
}
