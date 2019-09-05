//@vadym udod

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tools
{
    public class TileOffset : MonoBehaviour
    {
        public static Material material;
        public static Dictionary<string, ScrollableBG> materials = new Dictionary<string, ScrollableBG>();

        public string id = "mainbg";
        [Range(.01f, .2f)]
        public float xSpeed = .03f;
        [Range(.01f, .2f)]
        public float ySpeed = .03f;

        private Image image;
        private Vector4 offset = Vector4.zero;

        private ScrollableBG _material;

        protected void Awake()
        {
            image = GetComponent<Image>();

            if (!materials.ContainsKey(id))
                materials.Add(id, new ScrollableBG()
                {
                    material = new Material(Shader.Find("UI/Default Offset"))
                    {
                        hideFlags = HideFlags.HideAndDontSave,
                    },
                });

            _material = materials[id];

            image.material = _material.material;
        }

        protected void Update()
        {
            if (_material.changeApplied) return;

            _material.offset += new Vector2(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime);
        }

        protected void LateUpdate()
        {
            if (_material.changeApplied) _material.changeApplied = false;
        }

        public class ScrollableBG
        {
            public Material material;
            public bool changeApplied;

            private Vector2 _offset;

            public Vector2 offset
            {
                get => _offset;
                set
                {
                    _offset = value;
                    material.SetVector("_Offset", _offset);
                    changeApplied = true;
                }
            }
        }
    }
}
