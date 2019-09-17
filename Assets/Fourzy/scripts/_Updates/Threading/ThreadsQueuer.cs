//@vadym udod

using Fourzy._Updates.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Fourzy._Updates.Threading
{
    public class ThreadsQueuer : RoutinesBase
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
        private List<Action> instantiatingFunctions;

        private float lastTime;
        private Stopwatch stopwatch;

        public static void Initialize()
        {
            if (instance != null) return;

            GameObject go = new GameObject("ThreadsQueuer");
            go.transform.SetParent(null);
            instance = go.AddComponent<ThreadsQueuer>();

            DontDestroyOnLoad(go);
        }

        protected override void Awake()
        {
            base.Awake();

            queuedFunctions = new List<Action>();
            instantiatingFunctions = new List<Action>();
            stopwatch = new Stopwatch();
        }

        protected void Update()
        {
            while (queuedFunctions.Count > 0)
            {
                queuedFunctions[0]?.Invoke();

                queuedFunctions.RemoveAt(0);
            }

            lastTime = Time.time;
            stopwatch.Restart();
        }

        protected void LateUpdate()
        {
            //print(stopwatch.ElapsedMilliseconds);
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
