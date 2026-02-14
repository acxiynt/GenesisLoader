using BepInEx;
using BepInEx.Logging;
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
			string path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(Tables).Assembly.Location), "..", "..", "DolocData", $"{name}.json"));
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			string errorPath = Path.Combine(Path.GetDirectoryName(path), "..", "error.txt");
			using StreamWriter errorFilePtr = new StreamWriter(File.Open(errorPath, FileMode.Append, FileAccess.Write), Encoding.UTF8);
			if (File.Exists(path))
			{
				JSONNode mod;
				try
				{
					mod = JSON.Parse(File.ReadAllText(path));
				}
				catch (Exception e)
				{
					errorFilePtr.WriteLine($"[GenesisEngine] {DateTime.Now:yyyy-MM-dd HH-mm-ss} json {name}.json failed to parse with exception {e}, reading game default.");
					return _loader(name);
				}
				return mod;
			}
			else
			{
				errorFilePtr.WriteLine($"[GenesisEngine] {DateTime.Now:yyyy-MM-dd HH-mm-ss} json {name}.json does not exist.");
				JSONNode json = _loader(name);
				using StreamWriter filePtr = File.CreateText(path);
				filePtr.WriteLine(json.ToString(2));
				return json;
			}
		};
	}
}