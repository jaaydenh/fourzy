using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using SA.Foundation.Patterns;

namespace SA.Foundation.Async
{
    public static class SA_Coroutine 
    {

        /// <summary>
        /// Start's a coroutine immediately.
        /// </summary>
        /// <param name="routine">
        /// The <see cref="IEnumerator"/> routine you would like to start.
        /// </param> 
        public static Coroutine Start(IEnumerator routine) {
            return SA_InternalCoroutine.Instance.MonoStartCoroutine(routine);
        }

        /// <summary>
        /// Stops a routine.
        /// </summary>
        /// <param name="routine">
        /// The <see cref="IEnumerator"/> routine you would like to stop.
        /// </param> 
        public static void Stop(IEnumerator routine) {
            SA_InternalCoroutine.Instance.MonoStopCoroutine(routine);
        }

        /// <summary>
        ///  Add's routine to a global queue. Global queue run's only one routine at time.
        ///  <para> Please note, that if any of added routine's will stuck in internal loop, whole queue
        /// will be compromised.</para>
        /// </summary>
        public static void AddCoroutineToQueue(IEnumerator coroutine) {
            CoroutineQueue.Instance.AddCoroutine(coroutine);
        }


        /// <summary>
        /// Waits until the end of the frame after all cameras and GUI is rendered, just
        //  before displaying the frame on screen.
        /// </summary>
        public static Coroutine WaitForEndOfFrame(Action action) {
            return SA_InternalCoroutine.Instance.StartInstruction(new WaitForEndOfFrame(), action);
        }

        /// <summary>
        /// Waits until next fixed frame rate update function. 
        /// See Also: MonoBehaviour.FixedUpdate.
        /// </summary>
        public static Coroutine WaitForFixedUpdate(Action action) {
            return  SA_InternalCoroutine.Instance.StartInstruction(new WaitForFixedUpdate(), action);
        }

        /// <summary>
        ///  Wait for a given number of seconds using scaled time
        /// </summary>
        public static Coroutine WaitForSeconds(float seconds, Action action) {
            return SA_InternalCoroutine.Instance.StartInstruction(new WaitForSeconds(seconds), action);
        }

        /// <summary>
        ///  Wait for a random number of seconds between min [inclusive] and max [inclusive] interval, using scaled time
        /// </summary>
        /// <param name="min">min interval</param>
        /// <param name="max">max interval</param>
        /// <param name="action">action to be called </param>
        /// <returns></returns>
        public static Coroutine WaitForSecondsRandom(float min, float max, Action action) {
            float delay = UnityEngine.Random.Range(min, max);
            return WaitForSeconds(delay, action);
        }


       


        //--------------------------------------
        // Private Classes
        //--------------------------------------



        private class SA_InternalCoroutine : SA_Singleton<SA_InternalCoroutine>
        {
            public Coroutine MonoStartCoroutine(IEnumerator routine) {
                return StartCoroutine(routine);
            }

            public void MonoStopCoroutine(IEnumerator routine) {
                StopCoroutine(routine);
            }

            public Coroutine StartInstruction(YieldInstruction instruction, Action action) {
                return StartCoroutine(RunActionAfterInstruction(instruction, action));
            }

            private IEnumerator RunActionAfterInstruction(YieldInstruction instruction, Action action) {
                yield return instruction;
                action.Invoke();
            }
        }


        private class CoroutineQueue : SA_Singleton<CoroutineQueue>
        {
            private Queue<IEnumerator> m_coroutineQueue = new Queue<IEnumerator>();

            protected override void Awake() {
                base.Awake();
                StartCoroutine(CoroutineLoop());
            }

            public void AddCoroutine(IEnumerator coroutine) {
                m_coroutineQueue.Enqueue(coroutine);
            }

            private IEnumerator CoroutineLoop() {
                while (true) {
                    if (m_coroutineQueue.Count > 0) {
                        var enumerator = m_coroutineQueue.Dequeue();
                        yield return enumerator;
                    } else {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }

        }


    }
}