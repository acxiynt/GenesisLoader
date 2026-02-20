using System.Reflection;
using System.IO;
using System;
using System.Collections.Generic;

namespace GenesisLoader
{
    public class Util
    {
        static StreamWriter filePtr;
        static Util()
        {
            filePtr = new StreamWriter(new FileStream(Config.GetConfig("Path", "LogPath"), FileMode.Append));
        }

        //for main component only
        internal static void LogString(string text)
        {
            filePtr.WriteLine($"[GenesisLoader] {DateTime.Now:yyyy-MM-dd HH-mm-ss}: {text}");
            filePtr.Flush();
        }

        public static void LogString(string modName, string text)
        {
            filePtr.WriteLine($"[{modName}] {DateTime.Now:yyyy-MM-dd HH-mm-ss}: {text}");
            filePtr.Flush();
        }
    }
    public static class Main
    {
        //start of the chainload
        public static void Preinitalize()
        {
            Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
            try
            {
                foreach (string item in Directory.GetFiles(Config.GetConfig("Path", "AssemblyPath")))
                {
                    assemblies.Add(Path.GetFileNameWithoutExtension(item), Assembly.LoadFrom(item));
                    Util.LogString($"{Path.GetFileNameWithoutExtension(item)} loaded.");
                }
            }
            catch (Exception e)
            {
                Util.LogString($"Preinitalization failed, error: {e}");
            }
            ResolveAssembly(assemblies);
        }
        public static void ResolveAssembly(Dictionary<string, Assembly> assemblies)
        {
            AppDomain.CurrentDomain.AssemblyLoad
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string name = new AssemblyName(args.Name).Name;

                if (assemblies.TryGetValue(name, out Assembly assembly))
                    return assembly;

                return null;
            };
        }
    }
}
namespace Doorstop
{
    class Entrypoint
    {
        public static void Start()
        {
            GenesisLoader.Config.ReadFile("./GenesisLoader.cfg");
            GenesisLoader.Util.LogString("Preinitalization started.");
            GenesisLoader.Main.Preinitalize();
            GenesisLoader.Util.LogString("Preinitalization finished.");
        }
    }
}