﻿//@vadym udod

using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Camera3D
{
    /// <summary>
    /// Holds all rendered 3D items
    /// </summary>
    public class Camera3DManager : MonoBehaviour
    {
        public static Camera3DManager instance;

        public Material defaultItemToIMageMaterial;

        [HideInInspector]
        public Vector3 currentPosition;
        //dictionary for all items
        [HideInInspector]
        public Dictionary<string, Camera3DItem> items = new Dictionary<string, Camera3DItem>();
        //dictionary for content-items connection
        [HideInInspector]
        public Dictionary<Camera3DItem, List<GameObject>> itemsToContent = new Dictionary<Camera3DItem, List<GameObject>>();

        protected void Awake()
        {
            if (instance != null)
                return;

            instance = this;
            currentPosition = transform.position;
        }

        public T InstantiateItem<T>() where T : Camera3DItem
        {
            GameObject itemObject = new GameObject("_item");
            T item = itemObject.AddComponent<T>();

            item.transform.SetParent(transform);
            item.transform.position = currentPosition;
            currentPosition.x += item.GetCameraSize().x;

            item.GetKey();

            items.Add(item.key, item);

            return item;
        }

        public Camera3DItem InstantiateItem()
        {
            return InstantiateItem<Camera3DItem>();
        }

        public Camera3DItem SpawnItem(Camera3DItem item, bool forceNewKey = false)
        {
            Camera3DItem copy = null;

            copy = Instantiate(item);

            if (forceNewKey) copy.GetKey();

            copy.transform.SetParent(transform);
            copy.transform.position = currentPosition;
            currentPosition.x += copy.GetCameraSize().x;

            items.Add(copy.key, copy);

            return copy;
        }

        public void JoinToItem(GameObject go, Camera3DItem item)
        {
            if (!itemsToContent.ContainsKey(item))
                itemsToContent.Add(item, new List<GameObject>());

            itemsToContent[item].Add(go);
        }

        public void RemoveFromItem(GameObject go)
        {
            Camera3DItem itemToRemove = null;

            foreach (var pair in itemsToContent)
            {
                GameObject goToRemove = null;
                foreach (GameObject _go in pair.Value)
                {
                    if (_go == go)
                    {
                        goToRemove = _go;
                        break;
                    }
                }

                if (goToRemove)
                    pair.Value.Remove(goToRemove);

                if (pair.Value.Count == 0)
                {
                    itemToRemove = pair.Key;
                    break;
                }
            }

            if (itemToRemove)
            {
                Destroy(itemToRemove.gameObject);

                itemsToContent.Remove(itemToRemove);
                items.Remove(itemToRemove.key);
            }
        }

        public Camera3DItem GetItem(Camera3DItem item, bool forceNew = false)
        {
            Camera3DItem result;

            if (items.ContainsKey(item.key) && !forceNew)
                result = items[item.key];
            else
                result = SpawnItem(item, forceNew);

            return result;
        }
    }
}