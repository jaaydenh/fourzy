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
        /// Start routine
        /// </summary>
        /// <param name="name"></param>
        /// <param name="routine"></param>
        public void StartRoutine(string name, IEnumerator routine)
        {
            if (!routines.ContainsKey(name))
                routines.Add(name, new RoutineClass(this, name, routine));
        }

        /// <summary>
        /// Is specified routine active?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsRoutineActive(string name)
        {
            if (routines.ContainsKey(name))
                return routines[name].isActive;

            return false;
        }

        /// <summary>
        /// Starts routine
        /// </summary>
        /// <param name="name"></param>
        /// <param name="routine"></param>
        /// <param name="onEnd"></param>
        /// <param name="onCanceled"></param>
        public void StartRoutine(string name, IEnumerator routine, Action onEnd, Action onCanceled)
        {
            if (!routines.ContainsKey(name))
                routines.Add(name, new RoutineClass(this, name, routine, onEnd, onCanceled));
        }

        public void StartRoutine(string name, IEnumerator routine, Action both)
        {
            if (!routines.ContainsKey(name))
                routines.Add(name, new RoutineClass(this, name, routine, both, both));
        }

        /// <summary>
        /// Start a routine with time value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="onEnd"></param>
        /// <param name="onCanceled"></param>
        public void StartRoutine(string name, float time, Action onEnd, Action onCanceled)
        {
            if (!routines.ContainsKey(name))
                routines.Add(name, new RoutineClass(this, name, time, onEnd, onCanceled));
        }

        public void StartRoutine(string name, float time, Action both)
        {
            if (!routines.ContainsKey(name))
                routines.Add(name, new RoutineClass(this, name, time, both, both));
        }

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

            foreach (string key in keys)
                StopRoutine(key, callEnded);
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
        public Coroutine coroutine;
        public Action onEnded;
        public Action onCanceled;

        private Coroutine coroutineContainer;

        public bool isActive
        {
            get
            {
                return coroutineContainer != null;
            }
        }

        public RoutineClass(RoutinesBase owner, string name, IEnumerator routine)
        {
            this.name = name;
            this.owner = owner;

            coroutineContainer = owner.StartCoroutine(RoutineContainer(routine));
        }

        public RoutineClass(RoutinesBase owner, string name, IEnumerator routine, Action onEnded, Action onCanceled)
        {
            this.name = name;
            this.owner = owner;

            coroutineContainer = owner.StartCoroutine(RoutineContainer(routine));

            this.onEnded = onEnded;
            this.onCanceled = onCanceled;
        }

        public RoutineClass(RoutinesBase owner, string name, float time, Action onEnded, Action onCanceled)
        {
            this.name = name;
            this.owner = owner;

            coroutineContainer = owner.StartCoroutine(RoutineContainer(time));

            this.onEnded = onEnded;
            this.onCanceled = onCanceled;
        }

        public void Stop(bool callOnEnded)
        {
            if (coroutine != null)
                owner.StopCoroutine(coroutine);

            if (coroutineContainer != null)
                owner.StopCoroutine(coroutineContainer);

            if (callOnEnded && onEnded != null)
                onEnded.Invoke();

            owner.RemoveRoutine(name);
        }

        public void Cancel()
        {
            if (onCanceled != null)
                onCanceled.Invoke();

            Stop(false);
        }

        private IEnumerator RoutineContainer(IEnumerator _routine)
        {
            coroutine = owner.StartCoroutine(_routine);
            yield return coroutine;

            Stop(true);
        }

        private IEnumerator RoutineContainer(float time)
        {
            yield return new WaitForSeconds(time);

            Stop(true);
        }
    }
}
