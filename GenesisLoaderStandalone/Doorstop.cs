using UnityEngine;
using Genesis;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
namespace Doorstop
{
    class Entrypoint
    {
        public static bool UnityStarted = false;
        private static async Task OnGameInit()
        {
            if (!UnityStarted)
            {
                Application.logMessageReceivedThreaded += Util.LogUnity;
                UnityStarted = true;
                Util.LogString("Game initialized");
                Task init = Main.OnGameInit();
                Util.LogString("Started initializing mod and plugins");
                await init;
            }
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!UnityStarted)
            {
                _ = OnGameInit();
                UnityStarted = true;
            }
        }
        public static void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Config.ReadFile("./GenesisLoader.cfg");
            Util.LogString("Preinitalization started.");
            _ = Main.LoadMod();
            Task init = Main.PreInit();
            _ = Main.Init();
            Util.LogString("Preinitalization finished.");
        }
    }
}
