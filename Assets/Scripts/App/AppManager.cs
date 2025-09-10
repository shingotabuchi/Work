using Fwk;
using UnityEngine;

public class AppManager : SingletonPersistent<AppManager>
{
    float _deltaTime;

    protected override void Awake()
    {
        base.Awake();
        CameraManager.CreateIfNotExists();

#if UNITY_EDITOR
        WorkerWasdInputs.Instance.Initialize();
#endif
    }

    private void Update()
    {
        _deltaTime = Time.deltaTime;
        WorkerManager.Instance.Update(_deltaTime);

#if UNITY_EDITOR
        WorkerWasdInputs.Instance.Update(_deltaTime);
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
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