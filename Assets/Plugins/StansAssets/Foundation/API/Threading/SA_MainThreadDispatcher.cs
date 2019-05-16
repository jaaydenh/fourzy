using System.Collections.Generic;
using System;


using SA.Foundation.Patterns;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SA.Foundation.Threading
{

    /// <summary>
    /// Unity API isn't thread safe, so in case you are using threads,
    /// and you need to call any Unity API from that thread, this class is exactly what you need.
    /// <see cref="SA_MainThreadDispatcher"> is available for Editor and Playmode usage
    /// </summary>
    public static class SA_MainThreadDispatcher {


        /// <summary>
        /// If you going to use  <see cref="SA_MainThreadDispatcher"> in a playmode,
        /// you need to call Init methods befoe you can use <see cref="Enqueue">.
        /// </summary>
        public static void Init() {
			#if !UNITY_EDITOR
			MainThreadDispatcherPlaymode.Instance.Init ();
			#endif
		}


        /// <summary>
        /// Add's <see cref="Action"> to a main thread queue. 
        /// Action will be dispatched under a main thread on a next frame.
        /// </summary>
        public static void Enqueue(Action action) {
			#if UNITY_EDITOR
			MainThreadDispatcherEdior.Enqueue (action);
			#else
			if(MainThreadDispatcherPlaymode.HasInstance)
				MainThreadDispatcherPlaymode.Instance.Enqueue (action);
			#endif
		
		}






        #if UNITY_EDITOR
        private static class MainThreadDispatcherEdior
        {

            private static readonly Queue<Action> s_executionQueue = new Queue<Action>();
            static MainThreadDispatcherEdior() {
                EditorApplication.update += Update;
            }

            private static void Update() {
                lock (s_executionQueue) {
                    while (s_executionQueue.Count > 0) {
                        var action = s_executionQueue.Dequeue();
                        if (action != null)
                            action.Invoke();
                    }
                }
            }

            public static void Enqueue(Action action) {
                s_executionQueue.Enqueue(action);
            }
        }

        #endif

        //For Play Mode
        private class MainThreadDispatcherPlaymode : SA_Singleton<MainThreadDispatcherPlaymode>
        {

             private static readonly Queue<Action> s_executionQueue = new Queue<Action>();

            public void Init() { }

            public void Update() {
                lock (s_executionQueue) {
                    while (s_executionQueue.Count > 0) {
                        var action = s_executionQueue.Dequeue();
                        if (action != null)
                            action.Invoke();
                    }
                }
            }

            public void Enqueue(Action action) {
                s_executionQueue.Enqueue(action);
            }

        }

    }


}