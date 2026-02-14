using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using DolocTown.Config;
using SimpleJSON;
using System;
using System.Text;
using System.IO;
namespace GenesisLoader;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Harmony patcher = new Harmony("genesisengine.genesisloader");
        patcher.PatchAll();
        Logger = base.Logger;
        Logger.LogInfo($"[GenesisEngine] genesisloader hook hit");
		Util.LogString("hook hit");
		GenesisLoader.Config.ReadFile("./Bepinex/Config/GenesisLoader.cfg");
    }
}

public class Util
{
	static StreamWriter filePtr = new StreamWriter(new FileStream(Config.GetConfig("Path", "LogPath"), FileMode.Append));
	public static void LogString(string text)
	{	
		filePtr.WriteLine($"[GenesisLoader] {DateTime.Now:yyyy-MM-dd HH-mm-ss}: {text}");
		filePtr.Flush();
	}
}

[HarmonyPatch(typeof(Tables), MethodType.Constructor)]
[HarmonyPatch(new Type[] { typeof(Func<string, JSONNode>) })]
public class Patch
{
	static void Prefix(ref Func<string, JSONNode> loader)
	{
		Func<string, JSONNode> _loader = loader;
		loader = name =>
		{
			string path = Path.Combine(Config.GetConfig("Path", "ModPath"), $"{name}.json");
			string dataPath = Path.Combine(Config.GetConfig("Path", "DataPath"), $"{name}.json");
			if(!File.Exists(dataPath))
				Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
			string errorPath = Config.GetConfig("Path", "LogPath");
			if (!File.Exists(dataPath))
			{
				JSONNode json = _loader(name);
				using StreamWriter filePtr = File.CreateText(dataPath);
				filePtr.WriteLine(json.ToString(2));
			}
			if (File.Exists(path))
			{
				JSONNode mod;
				try
				{
					mod = JSON.Parse(File.ReadAllText(path));
				}
				catch (Exception e)
				{
					Util.LogString($"json {name}.json failed to parse with exception {e}, reading game default.");
					return _loader(name);
				}
				return mod;
			}
			else
			{
				JSONNode json = _loader(name);
				return json;
			}
		};
	}
}