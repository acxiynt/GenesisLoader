using System;
using System.Collections.Generic;
using System.IO;
using DolocTown;
using HarmonyLib;
using UnityEngine.InputSystem.EnhancedTouch;
namespace GenesisLoader;
public class Config 
{
    private static readonly Dictionary<string,Dictionary<string, string>> Dict = new Dictionary<string, Dictionary<string, string>>();
    private static readonly Config __self;
    public static Config Instance => __self;
    private static void Initalize()
    {
        foreach(UnserializedConfig config in ConfigConst.Config)
            if(!Dict.ContainsKey(config.header) || !Dict[config.header].ContainsKey(config.entry))
                Serialize(config);
    }

    static Config()
    {
        __self = new Config();
        Initalize();
    }
    private Config(){}
    //i like cereal
    /// <summary>
    /// Serializes config file passed as string path, creates a new config with config value stored in runtime if path is invalid
    /// </summary>
    /// <param name="path">Path of the file, passed as a string</param>
    private static void Serialize(string path)
    {
        string line = "0";
        string header = "default";
        int count = 0;
        if(!File.Exists(path) || new FileInfo(path).Length == 0)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            StreamWriter filePtr = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.ReadWrite));
            foreach(KeyValuePair<string, Dictionary<string, string>> group in Dict)
            {
                filePtr.WriteLine($"[{group.Key}]");
                foreach(KeyValuePair<string, string>item in group.Value)
                    filePtr.WriteLine($"{item.Key} = {item.Value}");
            }
            filePtr.Close();
            return;
        }
        StreamReader file = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
        while(!header.IsNullOrEmpty())
        {
            count++;
            line = file.ReadLine()?.TrimStart();
            if(line == null)
                break;
            else if (line == "")
                continue;
            if(line.TrimStart()[0] == ';')
                continue;
            if(line.TrimStart()[0] == '[')
            {
                header = line.Substring(1);
                header = header.Split(";",2)[0];
                header = header.Remove(header.Length-1);
                if(header.IsNullOrEmpty())
                    header = "default";
                continue;
            }
            if(line.Contains("="))
            {
                string[] delimited = line.Split("=",2);
                AddConfig(header, delimited[0].Trim(), delimited[1].TrimStart().Split(";",2)[0]);
            }
            else throw new FileFormatException($"error parsing value in line {count}");
        }
        file.Close();
    }
    private static void Serialize(UnserializedConfig config) => AddConfig(config.header, config.entry, config.data);

    private static void AddConfig(string header, string entry, string data)
    {
        if(!Dict.ContainsKey(header))
            Dict.Add(header, new Dictionary<string, string>());
        Dict[header][entry] = data;
    }
    /// <summary>
    /// API of Serialize(). Reads path of file, and add all config assosiated with it. If dont exist or empty, creates a new one with runtime config value
    /// </summary>
    /// <param name="path">Path of config file, in string</param>
    public static void ReadFile(string path) => Serialize(path);
    /// <summary>
    /// Queries the dictionary to return the config of header and entry passed as input, returns string.Empty() if not exist.
    /// </summary>
    /// <param name="header">The header of config value needed</param>
    /// <param name="entry">The name of config value needed</param>
    /// <returns>The config value, in string, will never be null, if fails, returns string.Empty()</returns>
    public static string GetConfig(string header, string entry) => Dict[header][entry];
}
public struct UnserializedConfig
{
    public readonly string header;
    public readonly string entry;
    public readonly string data;
    public UnserializedConfig(string header, string entry, string data)
    {
        this.header = header;
        this.entry = entry;
        this.data = data;
    }
}
public static class ConfigConst
{
    public static readonly UnserializedConfig[] Config =
    {
        new UnserializedConfig("Path", "DataPath", "./GenesisLoader/DolocData"),
        new UnserializedConfig("Path", "ModPath", "./GenesisLoader/DolocMod"),
        new UnserializedConfig("Path", "LogPath", "./GenesisLoader/Log.txt")
    };
}
