//@vadym udod

using Fourzy._Updates.Tools;
using System;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace Fourzy._Updates.Managers
{
    public class NetworkManager : RoutinesBase
    {
        #region Initializing
        public static NetworkManager instance
        {
            get
            {
                if (!_instance) Initialize();

                return _instance;
            }
        }

        private static NetworkManager _instance;

        private static void Initialize()
        {
            GameObject go = new GameObject("NetworkManager");
            _instance = go.AddComponent<NetworkManager>();
            _instance.SetMethod();

            DontDestroyOnLoad(go);
        }

        #endregion

        public static Action<NetStatus> onStatusChanged;

        private NetCheckMethod selected;

        public static NetStatus Status { get; private set; }

        public void StartChecking() => StartRoutine("checking", Check(true));

        public void FastCheck()
        {
            if (!IsRoutineActive("check")) StartRoutine("check", _Check());
        }

        private void SetMethod()
        {
            switch (Application.platform)
            {
                //case RuntimePlatform.WindowsEditor:
                //case RuntimePlatform.WindowsPlayer:
                //case RuntimePlatform.XboxOne:
                //    selected = new NetCheckMethod("msftconnecttest", "http://www.msftconnecttest.com/connecttest.txt", "Microsoft Connect Test");
                //    return;

                //case RuntimePlatform.IPhonePlayer:
                //case RuntimePlatform.OSXEditor:
                //case RuntimePlatform.OSXPlayer:
                //    selected = new NetCheckMethod("applehotspot", "https://captive.apple.com/hotspot-detect.html", "<HTML><HEAD><TITLE>Success</TITLE></HEAD><BODY>Success</BODY></HTML>");
                //    return;

                default:
                    selected = new NetCheckMethod("google204", "https://clients3.google.com/generate_204", HttpStatusCode.NoContent);
                    return;
            }
        }

        private IEnumerator Check(bool continuous)
        {
            do
            {
                yield return StartRoutine("check", _Check());
                yield return new WaitForSeconds(30f);
            } while (continuous);
        }

        private IEnumerator _Check()
        {
            yield return selected.Check();

            NetStatus previousStatus = Status;
            Status = selected.GetCheckStatus();

            if (Status != previousStatus)
            {
                onStatusChanged?.Invoke(Status);
            }
        }
    }

    [System.Serializable]
    public class NetCheckMethod
    {
        public string id;
        public string link;
        public NetCheckResponseType responseType;
        public HttpStatusCode expectedHttpStatusCode;
        public string expectedContent;

        private NetStatus status;

        public NetCheckMethod(string id, string link, HttpStatusCode expectedHttpStatusCode)
        {
            this.id = id;
            this.link = link;
            this.responseType = NetCheckResponseType.HTTPStatusCode;
            this.expectedHttpStatusCode = expectedHttpStatusCode;
            this.expectedContent = "";
        }

        public NetCheckMethod(string id, string link, string expectedContent, bool contentContain = false)
        {
            this.id = id;
            this.link = link;
            this.responseType = contentContain ? NetCheckResponseType.ResponseContainContent : NetCheckResponseType.ResponseContent;
            this.expectedHttpStatusCode = HttpStatusCode.OK;
            this.expectedContent = expectedContent;
        }

        public NetCheckMethod(string id, string link, NetCheckResponseType responseType, HttpStatusCode expectedHttpStatusCode, string expectedContent)
        {
            this.id = id;
            this.link = link;
            this.responseType = responseType;
            this.expectedHttpStatusCode = expectedHttpStatusCode;
            this.expectedContent = expectedContent;
        }

        public IEnumerator Check()
        {
            UnityWebRequest www = UnityWebRequest.Get(link);
            yield return www.SendWebRequest();

            // Check if there is any internet connectivity
            if (www.isNetworkError || www.isHttpError || www.responseCode == 0)
            {
                status = NetStatus.NoDNSConnection;
                yield break;
            }

            if (responseType == NetCheckResponseType.HTTPStatusCode)
                status = www.responseCode.ToString() == ((int)expectedHttpStatusCode).ToString() ? NetStatus.Connected : NetStatus.WalledGarden;
            else if (responseType == NetCheckResponseType.ResponseContent)
                status = www.downloadHandler.text.Trim().Equals(expectedContent.Trim()) ? NetStatus.Connected : NetStatus.WalledGarden;
            else if (responseType == NetCheckResponseType.ResponseContainContent)
                status = www.downloadHandler.text.Trim().Contains(expectedContent.Trim()) ? NetStatus.Connected : NetStatus.WalledGarden;
        }

        public NetStatus GetCheckStatus() => status;

        public override string ToString() => id + ": " + link;

        public override int GetHashCode() => id.GetHashCode() ^ link.GetHashCode() ^ responseType.GetHashCode() ^ expectedContent.GetHashCode() ^ expectedHttpStatusCode.GetHashCode();

        public override bool Equals(object obj)
        {
            bool result;
            if (!(obj is NetCheckMethod))
            {
                result = false;
            }
            else
            {
                NetCheckMethod other = (NetCheckMethod)obj;
                result = id.Equals(other.id) && link.Equals(other.link) && responseType.Equals(other.responseType)
                    && expectedHttpStatusCode.Equals(other.expectedHttpStatusCode) && expectedContent.Equals(other.expectedContent);
            }
            return result;
        }
    }

    public enum NetStatus
    {
        PendingCheck,
        NoDNSConnection,
        WalledGarden,
        Connected
    }
    public enum NetCheckResponseType
    {
        HTTPStatusCode,
        ResponseContent,
        ResponseContainContent
    }
}