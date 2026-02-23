using UnityEngine;
using Genesis;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Doorstop
{
    class Entrypoint
    {
        public static bool UnityStarted = false;
        private static void OnGameInit()
        {
            if (!UnityStarted)
            {
                Application.logMessageReceivedThreaded += Util.LogUnity;
                UnityStarted = true;
                Util.LogString("Game initialized");
                Main.OnGameInit().Wait();
                Util.LogString("Started initializing mod and plugins");
            }
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!UnityStarted)
            {
                OnGameInit();
                UnityStarted = true;
            }
            _ = Main.OnSceneLoaded();
        }
        public static void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Config.ReadFile("./GenesisLoader.cfg");
            Util.LogString("Preinitalization started.");
            List<Task> tasks = [Main.LoadMod(), Main.PreInit()];
            Task.WhenAll(tasks).Wait();
            Main.Init().Wait();
            Util.LogString("Preinitalization finished.");
        }
    }
}
