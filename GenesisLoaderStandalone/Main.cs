using System.Reflection;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Genesis;

public enum InfoType
{
    Info,
    Warning,
    Error
}
public static class Constant
{
    public static readonly string LogPath = Config.GetConfig("Path", "LogPath");
    public static readonly string ModPath = Config.GetConfig("Path", "ModPath");
    public static readonly string DataPath = Config.GetConfig("Path", "DataPath");
    public static readonly string AssemblyPath = Config.GetConfig("Path", "AssemblyPath");
}
public class Util
{
    static readonly StreamWriter filePtr;
    static Util()
    {
        filePtr = new StreamWriter(new FileStream(Config.GetConfig("Path", "LogPath"), FileMode.Create));
    }

    //for core component only
    /// <summary>
    /// Logs string into Logs.txt, for core component only.
    /// </summary>
    /// <param name="text">The text string you want to log.</param>
    /// <param name="info">Info level of the log, default is InfoType.info.</param>
    internal static void LogString(string text, InfoType info = InfoType.Info)
    {
        filePtr.WriteLine($"[{info}][GenesisLoader] {DateTime.Now:yyyy-MM-dd HH-mm-ss}: {text}");
        filePtr.Flush();
    }
    /// <summary>
    /// Logs string into Logs.txt, logs as info level.
    /// </summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="text">The text string you want to log.</param>
    public static void LogString(string modName, string text, InfoType info = InfoType.Info)
    {
        filePtr.WriteLine($"[{info}][{modName}] {DateTime.Now:yyyy-MM-dd HH-mm-ss}: {text}");
        filePtr.Flush();
    }

    internal static void LogUnity(string logString, string stackTrace, LogType type)
    {
        filePtr.WriteLine($"[{type}][Unity] {DateTime.Now:yyyy-MM-dd HH-mm-ss}: {logString}");
        if ((type == LogType.Error || type == LogType.Exception) && !string.IsNullOrWhiteSpace(stackTrace))
            filePtr.WriteLine($"Error stacktrace: {stackTrace}");
        filePtr.Flush();
    }
}