using System.Reflection;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Genesis;

public static class Main
{
    private static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
    private static Dictionary<string, Assembly> modAssemblies = new Dictionary<string, Assembly>();
    public static Dictionary<string, Assembly> Assemblies => assemblies;
    public static Dictionary<string, Assembly> ModAssemblies => modAssemblies;
    private static readonly object _lock = new object();

    private readonly static Func<Dictionary<string, Assembly>, Action<IPluginBase>, Task> CallPlugin = async (assemblies, method) =>
    {
        List<Task> tasks = new List<Task>();
        foreach (KeyValuePair<string, Assembly> item in assemblies)
        {
            foreach (Type type in item.Value.GetTypes())
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        if (typeof(IPluginBase).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            IPluginBase plugin = (IPluginBase)Activator.CreateInstance(type);
                            method(plugin);
                        }
                    }
                    catch (Exception e)
                    {
                        Util.LogString($"{item.Key} initialization failed, error: {e}", InfoType.Error);
                    }
                }));
            }
        }
        await Task.WhenAll(tasks);
    };
    //ctrl c ctrl v my beloved but this is cleaner
    private readonly static Func<Dictionary<string, Assembly>, Action<IModBase>, Task> CallMod = async (modAssemblies, method) =>
    {
        List<Task> tasks = new List<Task>();
        foreach (KeyValuePair<string, Assembly> item in modAssemblies)
        {
            foreach (Type type in item.Value.GetTypes())
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        if (typeof(IModBase).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            IModBase mod = (IModBase)Activator.CreateInstance(type);
                            method(mod);
                        }
                    }
                    catch (Exception e)
                    {
                        Util.LogString($"{item.Key} initialization failed, error: {e}", InfoType.Error);
                    }
                }));
            }
        }
        await Task.WhenAll(tasks);
    };
    //start of the chainload
    public static async Task PreInit()
    {
        List<Task> tasks = new List<Task>();
        foreach (string file in Directory.GetFiles(Config.GetConfig("Path", "AssemblyPath")))
        {
            string item = file;
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    lock (_lock)
                        assemblies.Add(Path.GetFileNameWithoutExtension(item), Assembly.LoadFrom(item));
                    Util.LogString($"{Path.GetFileNameWithoutExtension(item)} loaded.");
                }
                catch (Exception e)
                {
                    Util.LogString($"Preinitialization failed, error: {e}", InfoType.Error);
                }
            }));
        }
        await Task.WhenAll(tasks);
    }

    public static async Task LoadMod()
    {
        List<Task> tasks = new List<Task>();
        foreach (string item in Directory.GetDirectories(Config.GetConfig("Path", "ModPath")))
        {
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    if (Directory.Exists($"{item}/Assemblies"))
                        foreach (string subitem in Directory.GetFiles($"{item}/Assemblies", "*.dll", SearchOption.AllDirectories))
                        {
                            lock (_lock)
                                modAssemblies.Add(Path.GetFileNameWithoutExtension(subitem), Assembly.LoadFrom(subitem));
                            Util.LogString($"{Path.GetFileNameWithoutExtension(subitem)} loaded.");
                        }
                }
                catch (Exception e)
                {
                    Util.LogString($"Modloading failed, error: {e}", InfoType.Error);
                }
            }));
        }
        await Task.WhenAll(tasks);
    }
    //to init plugin, this is earlier than init mods
    public static async Task Init() => await CallPlugin(assemblies, plugin => plugin.Init());
    //way later than Init()
    public static async Task OnGameInit() => await CallPlugin(assemblies, plugin => plugin.OnGameInit());
    public static async Task InitMod() => await CallMod(modAssemblies, mod => mod.Init());
    public static async Task OnSceneLoaded()
    {
        List<Task> tasks =
        [
            CallPlugin(assemblies, plugin => plugin.OnSceneLoaded()),
            CallMod(modAssemblies, mod => mod.OnSceneLoaded()),
        ];
        await Task.WhenAll(tasks);
    }
}