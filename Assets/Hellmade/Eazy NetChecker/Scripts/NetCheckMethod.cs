using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace Hellmade.Net
{
    /// <summary>
    /// The method of checking and determining the status of the internet connection
    /// </summary>
    [System.Serializable]
    public class NetCheckMethod
    {
        /// <summary>
        /// The ID of the method
        /// </summary>
        public string id;
        /// <summary>
        /// The link the method uses for internet check
        /// </summary>
        public string link;
        /// <summary>
        /// The response type that is used for checking for validity of the connection
        /// </summary>
        public NetCheckResponseType responseType;
        /// <summary>
        /// The expected response from the HTTP request when target destination can succesfully be reached.Only applicable if HTTP status code is the selected response type
        /// </summary>
        public HttpStatusCode expectedHttpStatusCode;
        /// <summary>
        /// The response content is used as a response check. Whether the whole content needs to match, or just a part of it, depends on the selected response type. Check is done on trimmed content, therefore whitespaces, newlines and tabs are ignored.
        /// </summary>
        public string expectedContent;

        private NetStatus status;
        private ErrorInfo errorInfo;
        private float responseTime;

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

        /// <summary>
        /// Determines the internet connection status using the method's settings. Use GetCheckStatus after the check is finished to get the determined internet connection status
        /// </summary>
        /// <returns></returns>
        public IEnumerator Check()
        {
            if (EazyNetChecker.ShowDebug)
            {
                Debug.Log("[Eazy NetChecker] Selected method: " + id);
            }

            float startMillisecondsTime = Time.realtimeSinceStartup * 1000;

            UnityWebRequest www = UnityWebRequest.Get(link);
            yield return www.SendWebRequest();

            responseTime = (Time.realtimeSinceStartup * 1000) * startMillisecondsTime;
            errorInfo = new ErrorInfo(www.isNetworkError, www.isHttpError, www.error);

            // Check if there is any internet connectivity
            if (www.isNetworkError || www.isHttpError || www.responseCode == 0)
            {
                status = NetStatus.NoDNSConnection;
                yield break;
            }

            if (responseType == NetCheckResponseType.HTTPStatusCode)
            {
                status = www.responseCode.ToString() == ((int)expectedHttpStatusCode).ToString() ? NetStatus.Connected : NetStatus.WalledGarden;
            }
            else if (responseType == NetCheckResponseType.ResponseContent)
            {
                status = www.downloadHandler.text.Trim().Equals(expectedContent.Trim()) ? NetStatus.Connected : NetStatus.WalledGarden;
            }
            else if (responseType == NetCheckResponseType.ResponseContainContent)
            {
                status = www.downloadHandler.text.Trim().Contains(expectedContent.Trim()) ? NetStatus.Connected : NetStatus.WalledGarden;
            }
        }

        /// <summary>
        /// Returns the determined internet connection status after the check.
        /// </summary>
        /// <returns></returns>
        public NetStatus GetCheckStatus()
        {
            return status;
        }

        /// <summary>
        /// Returns any network error information
        /// </summary>
        /// <returns></returns>
        public ErrorInfo GetErrorInfo()
        {
            return errorInfo;
        }

        /// <summary>
        /// Returns the resposne time of the last check
        /// </summary>
        /// <returns></returns>
        public float GetResponseTime()
        {
            return responseTime;
        }

        public override string ToString()
        {
            return id + ": " + link;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() ^ link.GetHashCode() ^ responseType.GetHashCode() ^ expectedContent.GetHashCode() ^ expectedHttpStatusCode.GetHashCode();
        }

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
}