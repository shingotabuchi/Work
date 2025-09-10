using Fwk;
using UnityEngine;

public class AppManager : SingletonPersistent<AppManager>
{
    float _deltaTime;

    private void Update()
    {
        _deltaTime = Time.deltaTime;
        WorkerManager.Instance.Update(_deltaTime);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstance()
    {
        if (Instance != null)
        {
            return;
        }

        var go = new GameObject("AppManager");
        Instance = go.AddComponent<AppManager>();
    }
}