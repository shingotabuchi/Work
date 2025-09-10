using UnityEngine;
using System;

namespace Fwk
{
    public class SingletonGeneric<T> where T : class, new()
    {
        protected SingletonGeneric() { }

        private static readonly Lazy<T> instance = new Lazy<T>(() => new T());

        public static T Instance { get { return instance.Value; } }
    }

    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; protected set; }
        public static bool Exists { get => Instance != null; }
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"Singleton of type {typeof(T)} already exists. Destroying duplicate instance.");
                Destroy(gameObject);
            }
        }
    }

    public class SingletonPersistent<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(this);
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"SingletonPersistent of type {typeof(T)} already exists. Destroying duplicate instance.");
                Destroy(gameObject);
            }
        }
    }
}