using System.Reflection;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Genesis
{
    public enum InfoLevel
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
        static StreamWriter filePtr;
        static Util()
        {
            filePtr = new StreamWriter(new FileStream(Config.GetConfig("Path", "LogPath"), FileMode.Create));
        }

        //for core component only
        /// <summary>
        /// Logs string into Logs.txt, for core component only.
        /// </summary>
        /// <param name="text">The text string you want to log.</param>
        /// <param name="info">Info level of the log, default is InfoLevel.info.</param>
        internal static void LogString(string text, InfoLevel info = InfoLevel.Info)
        {
            filePtr.WriteLine($"[{info}][GenesisLoader] {DateTime.Now:yyyy-MM-dd HH-mm-ss}: {text}");
            filePtr.Flush();
        }
        /// <summary>
        /// Logs string into Logs.txt, logs as info level.
        /// </summary>
        /// <param name="modName">The name of the mod.</param>
        /// <param name="text">The text string you want to log.</param>
        public static void LogString(string modName, string text, InfoLevel info = InfoLevel.Info)
        {
            filePtr.WriteLine($"[{info}][{modName}] {DateTime.Now:yyyy-MM-dd HH-mm-ss}: {text}");
            filePtr.Flush();
        }
    }
    public static class Main
    {
        private static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
        private static Dictionary<string, Assembly> modAssemblies = new Dictionary<string, Assembly>();
        private static Dictionary<string, Assembly> gameAssemblies => new Dictionary<string, Assembly>();

        public static Dictionary<string, Assembly> Assemblies => assemblies;
        public static Dictionary<string, Assembly> ModAssemblies => modAssemblies;
        public static Dictionary<string, Assembly> GameAssemblies => gameAssemblies;
        //start of the chainload
        public static void Preinitalize()
        {
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
                Util.LogString($"Preinitalization failed, error: {e}", InfoLevel.Error);
            }
        }
        //to init plugin, this is earlier than init mods
        public static void Init()
        {
            try
            {
                foreach (KeyValuePair<string, Assembly> item in assemblies)
                {
                    foreach (Type type in item.Value.GetTypes())
                    {
                        if (typeof(IPluginBase).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            IPluginBase plugin = (IPluginBase)Activator.CreateInstance(type);
                            plugin.Init();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Util.LogString($"Initalization failed, error: {e}", InfoLevel.Error);
            }
        }
        public static void LoadMod()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                foreach (string item in Directory.GetDirectories(Config.GetConfig("Path", "ModPath")))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        if (Directory.Exists($"{item}/Assemblies"))
                            foreach (string subitem in Directory.GetFiles($"{item}/Assemblies", "*.dll", SearchOption.AllDirectories))
                            {
                                modAssemblies.Add(Path.GetFileNameWithoutExtension(subitem), Assembly.LoadFrom(subitem));
                                Util.LogString($"{Path.GetFileNameWithoutExtension(subitem)} loaded.");
                            }
                    }));
                }
            }
            catch (Exception e)
            {
                Util.LogString($"Modloading failed, error: {e}", InfoLevel.Error);
            }
        }

        public static void InitMod()
        {
            try
            {
                foreach (KeyValuePair<string, Assembly> item in modAssemblies)
                {
                    foreach (Type type in item.Value.GetTypes())
                    {
                        if (typeof(IModBase).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            IModBase mod = (IModBase)Activator.CreateInstance(type);
                            mod.Init();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Util.LogString($"Mod initalization failed, error: {e}", InfoLevel.Error);
            }
        }
    }
}
namespace Doorstop
{
    class Entrypoint
    {
        public static void Start()
        {
            Genesis.Config.ReadFile("./GenesisLoader.cfg");
            Genesis.Util.LogString("Preinitalization started.");
            Genesis.Main.Preinitalize();
            Genesis.Main.Init();
            Genesis.Util.LogString("Preinitalization finished.");
            Genesis.Util.LogString("Modloading started.");
            Genesis.Main.LoadMod();
            Genesis.Main.InitMod();
            Genesis.Util.LogString("Modloading finished.");
        }
    }
}
