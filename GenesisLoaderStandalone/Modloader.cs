namespace Genesis;

public interface IModBase
{
    void Init();
    void OnSceneLoaded() { }
}

public interface IPluginBase
{
    void Init();
    void OnGameInit() { }
    void OnSceneLoaded() { }
}