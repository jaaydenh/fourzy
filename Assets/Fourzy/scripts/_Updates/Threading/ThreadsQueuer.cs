//@vadym udod

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Fourzy._Updates.Threading
{
    public class ThreadsQueuer : MonoBehaviour
    {
        public static ThreadsQueuer Instance
        {
            get
            {
                if (instance == null)
                    Initialize();

                return instance;
            }
        }

        private static ThreadsQueuer instance;

        private List<Action> queuedFunctions;

        public static void Initialize()
        {
            if (instance != null) return;

            GameObject go = new GameObject("ThreadsQueuer");
            go.transform.SetParent(null);
            instance = go.AddComponent<ThreadsQueuer>();

            DontDestroyOnLoad(go);
        }

        public void Awake()
        {
            queuedFunctions = new List<Action>();
        }

        public void Update()
        {
            while (queuedFunctions.Count > 0)
            {
                queuedFunctions[0]?.Invoke();

                queuedFunctions.RemoveAt(0);
            }
        }

        public Thread StartThreadForFunc(Action action)
        {
            Thread thread = new Thread(new ThreadStart(action));
            thread.Start();

            return thread;
        }

        public void QueueFuncToExecuteFromMainThread(Action action)
        {
            queuedFunctions.Add(action);
        }
    }
}
