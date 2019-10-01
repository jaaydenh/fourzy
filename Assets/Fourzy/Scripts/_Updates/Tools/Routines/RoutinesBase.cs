//@vadym udod

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Tools
{
    public abstract class RoutinesBase : MonoBehaviour
    {
        protected Dictionary<string, RoutineClass> routines;

        protected virtual void Awake()
        {
            routines = new Dictionary<string, RoutineClass>();
        }

        /// <summary>
        /// Starts routine
        /// </summary>
        /// <param name="name"></param>
        /// <param name="routine"></param>
        /// <param name="onEnd"></param>
        /// <param name="onCanceled"></param>
        public Coroutine StartRoutine(string name, IEnumerator routine, Action onEnd = null, Action onCanceled = null)
        {
            if (!routines.ContainsKey(name))
            {
                RoutineClass _routineContainer = new RoutineClass(this, name, routine, onEnd, onCanceled);
                routines.Add(name, _routineContainer);

                return _routineContainer.WaitFor();
            }

            return null;
        }

        public Coroutine StartRoutine(string name, IEnumerator routine, Action both) => StartRoutine(name, routine, both, both);

        /// <summary>
        /// Start a routine with time value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="onEnd"></param>
        /// <param name="onCanceled"></param>
        public Coroutine StartRoutine(string name, float time, Action onEnd = null, Action onCanceled = null)
        {
            if (!routines.ContainsKey(name))
            {
                RoutineClass _routineContainer = new RoutineClass(this, name, time, onEnd, onCanceled);
                routines.Add(name, _routineContainer);

                return _routineContainer.WaitFor();
            }

            return null;
        }

        public Coroutine StartRoutine(string name, float time, Action both) => StartRoutine(name, time, both, both);

        /// <summary>
        /// Is specified routine active?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsRoutineActive(string name) => routines.ContainsKey(name);

        /// <summary>
        /// Stops specified routine with optional call of onEnded
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callEnded"></param>
        /// <returns></returns>
        public bool StopRoutine(string name, bool callEnded)
        {
            if (routines.ContainsKey(name))
            {
                routines[name].Stop(callEnded);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Stops all routines
        /// </summary>
        /// <param name="callEnded"></param>
        public void StopAllRoutines(bool callEnded)
        {
            List<string> keys = new List<string>(routines.Keys);

            foreach (string key in keys) StopRoutine(key, callEnded);
        }

        /// <summary>
        /// Cancel ongoing routine and calls onCanceled if the one have it
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CancelRoutine(string name)
        {
            if (routines.ContainsKey(name))
            {
                routines[name].Cancel();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removing this routine entry wont stop coroutine, so make sure you did before calling this
        /// </summary>
        /// <param name="name"></param>
        public void RemoveRoutine(string name)
        {
            if (routines.ContainsKey(name))
                routines.Remove(name);
        }
    }

    public class RoutineClass
    {
        public RoutinesBase owner;

        public string name;
        public Action onEnded;
        public Action onCanceled;

        public bool terminated = false;
        public Coroutine nested;
        private IEnumerator payload;

        public RoutineClass(RoutinesBase owner, string name, IEnumerator routine, Action onEnded, Action onCanceled)
        {
            this.name = name;
            this.owner = owner;

            this.onEnded = onEnded;
            this.onCanceled = onCanceled;

            payload = routine;
            nested = owner.StartCoroutine(Wrapper());
        }

        public RoutineClass(RoutinesBase owner, string name, float time, Action onEnded, Action onCanceled)
        {
            this.name = name;
            this.owner = owner;

            this.onEnded = onEnded;
            this.onCanceled = onCanceled;

            nested = owner.StartCoroutine(Wrapper(time));
        }

        public Coroutine WaitFor()
        {
            return owner.StartCoroutine(Wait());
        }

        public void Stop(bool callOnEnded)
        {
            terminated = true;

            if (callOnEnded && onEnded != null) onEnded.Invoke();
            if (nested != null) owner.StopCoroutine(nested);

            owner.RemoveRoutine(name);
        }

        public void Cancel()
        {
            onCanceled?.Invoke();

            Stop(false);
        }

        private IEnumerator Wrapper(float time = 0f)
        {
            if (time == 0f)
            {
                if (payload != null) while (payload.MoveNext() && !terminated) yield return payload.Current;
            }
            else if (time < 0f) while (true) if (terminated) yield break; else yield return null;
            else
            {
                float timer = 0f;

                while (timer < time)
                {
                    if (terminated)
                        yield break;
                    else
                    {
                        yield return null;
                        timer += Time.deltaTime;
                    }
                }
            }

            Stop(true);
        }

        private IEnumerator Wait()
        {
            while (!terminated) yield return null;
        }
    }
}
