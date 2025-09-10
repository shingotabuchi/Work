using UnityEngine;
using UnityEngine.EventSystems;

namespace Fwk
{
    public class EventSystemManager : SingletonPersistent<EventSystemManager>
    {
        public EventSystem EventSystem { get; private set; }
        public StandaloneInputModule InputModule { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            UpdateEventSystem();
        }

        public static void CreateIfNotExists()
        {
            if (Instance != null)
            {
                return;
            }
            var go = new GameObject("EventSystemManager");
            Instance = go.AddComponent<EventSystemManager>();
        }

        public void UpdateEventSystem()
        {
            FindAndSetEventSystem();
            DestroyOtherEventSystems();
        }

        private void FindAndSetEventSystem()
        {
            if (EventSystem != null)
            {
                return;
            }

            var eventSystemObject = FindFirstObjectByType<EventSystem>();
            if (eventSystemObject != null)
            {
                EventSystem = eventSystemObject;
                InputModule = eventSystemObject.GetComponent<StandaloneInputModule>();
                eventSystemObject.transform.SetParent(transform, false);
            }
            else
            {
                CreateEventSystem();
            }
        }

        private void CreateEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem");
            EventSystem = eventSystemObject.AddComponent<EventSystem>();
            InputModule = eventSystemObject.AddComponent<StandaloneInputModule>();
            eventSystemObject.transform.SetParent(transform, false);
        }

        private void DestroyOtherEventSystems()
        {
            var foundEventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            foreach (var eventSystem in foundEventSystems)
            {
                if (eventSystem != EventSystem)
                {
                    Destroy(eventSystem.gameObject);
                }
            }
        }
    }
}