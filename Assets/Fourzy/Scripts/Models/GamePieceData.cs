using UnityEngine;
using UnityEngine.Serialization;

namespace Fourzy
{
    [System.Serializable]
    public class GamePieceData : ISerializationCallbackReceiver
    {
        public int ID;
        public string Name;
        public bool Enabled;
        [SerializeField] string OutlineColor = string.Empty;
        [SerializeField] string SecondaryColor = string.Empty;

        public Color OutlineColorWrapper { get; set; }
        public Vector4 SecondaryColorWrapper { get; set; }

        public void OnBeforeSerialize()
        {
            OutlineColor = OutlineColorWrapper.ToJson();
            SecondaryColor = SecondaryColorWrapper.ToJson();
        }

        public void OnAfterDeserialize()
        {
            OutlineColorWrapper = OutlineColor.ColorFromJson();
            SecondaryColorWrapper = SecondaryColor.Vector4FromJson();
        }
    }


    public static class JsonConverterExternsion
    {
        public static string ToJson(this Color c)
        {
            return string.Format("{0},{1},{2},{3}", c.r, c.g, c.b, c.a);
        }

        public static Color ColorFromJson(this string value)
        {
            Color color = new Color();
            var rgba = value.Split(',');
            if (rgba.Length == 4)
            {
                color.r = float.Parse(rgba[0]);
                color.g = float.Parse(rgba[1]);
                color.b = float.Parse(rgba[2]);
                color.a = float.Parse(rgba[3]);
            }
            return color;
        }

        public static string ToJson(this Vector4 vector)
        {
            return string.Format("{0},{1},{2},{3}", vector.x, vector.y, vector.z, vector.w);
        }

        public static Vector4 Vector4FromJson(this string value)
        {
            Vector4 v = new Vector4();
            var xyzw = value.Split(',');
            if (xyzw.Length == 4)
            {
                v.x = float.Parse(xyzw[0]);
                v.y = float.Parse(xyzw[1]);
                v.z = float.Parse(xyzw[2]);
                v.w = float.Parse(xyzw[3]);
            }
            return v;
        }
    }
}