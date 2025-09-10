using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Fwk
{
    public static class SceneLoader
    {
        private static bool _isInitialized = false;

        public static void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public static void UnloadScene(string sceneName)
        {
            Initialize();
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(sceneName);
            }
            else
            {
                Debug.LogWarning($"Scene '{sceneName}' is not currently loaded.");
            }
        }

        public static async UniTask LoadSceneAsync(
            string sceneName,
            bool additive = false,
            Action<float> onLoadingProgress = null,
            Action<string> onSceneLoaded = null,
            CancellationToken cancellationToken = default)
        {
            Initialize();
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name cannot be null or empty.");
                return;
            }

            LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, mode);
            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();

                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                onLoadingProgress?.Invoke(progress);
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            onSceneLoaded?.Invoke(sceneName);
        }

        public static void LoadSceneSync(
            string sceneName,
            bool additive = false,
            Action<string> onSceneLoaded = null)
        {
            Initialize();
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name cannot be null or empty.");
                return;
            }

            LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

            try
            {
                SceneManager.LoadScene(sceneName, mode);
                onSceneLoaded?.Invoke(sceneName);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load scene '{sceneName}' synchronously: {ex.Message}");
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CameraManager.CreateIfNotExists();
            EventSystemManager.CreateIfNotExists();

            CameraManager.Instance.UpdateCameras();
            EventSystemManager.Instance.UpdateEventSystem();
        }
    }
}