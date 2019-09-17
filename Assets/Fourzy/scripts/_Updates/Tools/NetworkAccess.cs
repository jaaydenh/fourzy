//@vadym udod

using Fourzy._Updates.Tools;
using GameSparks.Core;
using Hellmade.Net;
using System;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Mechanics
{
    public class NetworkAccess : RoutinesBase
    {
//        public static Action<bool> onNetworkAccess;
//        public static string IP = "8.8.8.8";

//        public static float STEP_TIME = .1f;
//        public static bool RUN_ACCESS_LOOP = true;
//        public static bool DEBUG = false;

//        public static NetworkAccessEnum FAKE_ACCESS = NetworkAccessEnum.NONE;
//        public static NetworkAccessEnum NETWORK_STATE = NetworkAccessEnum.NONE;
//        //public static bool ACCESS => NETWORK_STATE == NetworkAccessEnum.ACCESSIBLE;
//        //for now pass throught GS
//        public static bool HAVE_ACCESS => GS.Available;

//        public static NetworkAccess Instance
//        {
//            get
//            {
//                if (instance == null)
//                    Initialize();

//                return instance;
//            }
//        }

//        private static NetworkAccess instance;

//        private NetworkAccessEnum stateToggle = NetworkAccessEnum.NONE;

//        protected override void Awake()
//        {
//            base.Awake();

//            NETWORK_STATE = NetworkAccessEnum.NONE;

//            //if (RUN_ACCESS_LOOP)
//            //    StartRoutine("networkAccessLoop", NetworkAccessRoutine());

//            //as for now tight it to GS.GameSparksAvailable
//            //GS.GameSparksAvailable += (state) => onNetworkAccess?.Invoke(state);
//            EazyNetChecker.OnConnectionStatusChanged += OnNetStatusChanged;
//        }

////        protected void Update()
////        {
////#if UNITY_EDITOR
////            if (Input.GetKeyDown(KeyCode.I))
////                FAKE_ACCESS = FAKE_ACCESS == NetworkAccessEnum.NONE ? NetworkAccessEnum.NON_ACCESSIBLE : NetworkAccessEnum.NONE;
////#endif
////        }

//        public static void Initialize(string IP = "", float STEP_TIME = 0f, bool RUN_ACCESS_LOOP = true, bool DEBUG = false, NetworkAccessEnum FAKE_ACCESS = NetworkAccessEnum.NONE)
//        {
//            if (!string.IsNullOrEmpty(IP))
//                NetworkAccess.IP = IP;

//            if (STEP_TIME != 0f)
//                NetworkAccess.STEP_TIME = STEP_TIME;

//            NetworkAccess.DEBUG = DEBUG;

//            if (FAKE_ACCESS != NetworkAccessEnum.NONE)
//                NetworkAccess.FAKE_ACCESS = FAKE_ACCESS;

//            Initialize();
//        }

//        public static void Initialize()
//        {
//            if (instance != null) return;

//            GameObject go = new GameObject("NetworkAccess");
//            go.transform.SetParent(null);
//            instance = go.AddComponent<NetworkAccess>();

//            DontDestroyOnLoad(go);
//        }

//        public static void _TryPing(Action<bool> onPingDone, float maxWaitTime = 2f, bool useFake = true) => Instance.TryPing(onPingDone, maxWaitTime, useFake);

//        public void TryPing(Action<bool> onPingDone, float maxWaitTime = 2f, bool useFake = true) => StartCoroutine(PingRoutine(onPingDone, maxWaitTime, useFake));

//        private void OnNetStatusChanged()
//        {
//            print("connection " + EazyNetChecker.Status);
//            onNetworkAccess?.Invoke(EazyNetChecker.Status == NetStatus.Connected);
//            //// Change status text depending on the status
//            //switch (EazyNetChecker.Status)
//            //{
//            //    case NetStatus.Connected:

//            //        break;
//            //    case NetStatus.NoDNSConnection:

//            //        break;
//            //    case NetStatus.WalledGarden:

//            //        break;
//            //    case NetStatus.PendingCheck:

//            //        break;
//            //}
//        }

//        private IEnumerator PingRoutine(Action<bool> onPingDone, float maxWaitTime, bool useFake)
//        {
//            if (useFake)
//                switch (FAKE_ACCESS)
//                {
//                    case NetworkAccessEnum.NON_ACCESSIBLE:
//                        yield return new WaitForSeconds(UnityEngine.Random.Range(.2f, maxWaitTime));
//                        onPingDone.Invoke(false);
//                        yield break;

//                    case NetworkAccessEnum.ACCESSIBLE:
//                        yield return new WaitForSeconds(UnityEngine.Random.Range(.2f, maxWaitTime));
//                        onPingDone.Invoke(true);
//                        yield break;
//                }

//            float waitTime = 0f;
//            Ping ping = new Ping(IP);

//            while (!ping.isDone && waitTime < maxWaitTime)
//            {
//                waitTime += STEP_TIME;

//                yield return new WaitForSeconds(STEP_TIME);
//            }

//            if (waitTime <= maxWaitTime && ping.time != -1)
//                onPingDone.Invoke(true);
//            else
//                onPingDone.Invoke(false);

//            ping.DestroyPing();
//        }

//        private IEnumerator NetworkAccessRoutine()
//        {
//            while (true)
//            {
//                //check internet access
//                TryPing((access) =>
//                {
//                    NetworkAccessEnum _state = access ? NetworkAccessEnum.ACCESSIBLE : NetworkAccessEnum.NON_ACCESSIBLE;

//                    if (((_state == NetworkAccessEnum.NON_ACCESSIBLE && NETWORK_STATE == NetworkAccessEnum.NON_ACCESSIBLE) /*no connection condition*/
//                        || _state == NetworkAccessEnum.ACCESSIBLE) /*connection condition*/
//                        && stateToggle != _state)
//                    {
//                        if (DEBUG)
//                            Debug.Log("Network state: " + _state);

//                        stateToggle = _state;

//                        onNetworkAccess?.Invoke(access);
//                    }
                    
//                    NETWORK_STATE = _state;
//                }, .9f);

//                yield return new WaitForSeconds(1f);
//            }
//        }

//        public enum NetworkAccessEnum
//        {
//            NONE,
//            ACCESSIBLE,
//            NON_ACCESSIBLE,
//        }
    }
}