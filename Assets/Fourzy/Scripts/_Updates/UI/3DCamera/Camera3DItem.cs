//@vadym udod

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Fourzy._Updates.UI.Camera3D
{
    /// <summary>
    /// Adds a camera to this object and uses its renderTexture to whatever the purpose is.
    /// </summary>
    [ExecuteInEditMode]
    public class Camera3DItem : MonoBehaviour
    {
        [Tooltip("Pixel width")]
        public int originalTextureWidth = 256;
        [Tooltip("Pixel height")]
        public int originalTextureHeight = 256;
        public StringEventJoin[] events;

        public string key;
        [HideInInspector]
        public RenderTexture renderTexture;

        private Camera cam;
        private Dictionary<string, StringEventJoin> eventsFastAccess = new Dictionary<string, StringEventJoin>();

        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(key))
                GetKey();

            cam = GetComponent<Camera>();

            if (!cam)
                cam = gameObject.AddComponent<Camera>();

            //get events
            if (events != null)
                foreach (StringEventJoin @event in events)
                    eventsFastAccess.Add(@event.command, @event);

            ConfigureCamera();
        }

        [ContextMenu("Reconfigure camera")]
        public void ConfigureCamera()
        {
            if (!cam)
                return;

            renderTexture = new RenderTexture(originalTextureWidth, originalTextureHeight, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();

            cam.fieldOfView = 60f;
            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = Color.clear;
            cam.farClipPlane = 10f;

            cam.targetTexture = renderTexture;
        }

        public virtual void Init() { }

        public Vector2 GetCameraSize()
        {
            float size = cam.farClipPlane * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad);
            return new Vector2(size * 2f, size * 2f);
        }

        public void PlaceObject(GameObject go, Vector3 offset)
        {
            go.transform.SetParent(transform);
            go.transform.localPosition = offset;
        }

        public void SizeTexture(Vector2 newSize)
        {
            renderTexture.Release();

            renderTexture.width = (int)newSize.x;
            renderTexture.height = (int)newSize.y;

            renderTexture.Create();
        }

        public void TryExecute(string command)
        {
            if (eventsFastAccess.ContainsKey(command))
                eventsFastAccess[command].@event.Invoke();
        }

        [ContextMenu("Random key")]
        public void GetKey()
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(UnityEngine.Random.value.ToString());
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            key = sb.ToString();
        }

        public class StringEventJoin
        {
            public string command;
            public Action @event;
        }
    }

}