////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using SA.Foundation.Time;
using System.Text;

namespace SA.iOS.Foundation
{

    [Serializable]
    public class ISN_NSKeyValueObject
    {

        [SerializeField] private string m_key = String.Empty;
        [SerializeField] private string m_value = String.Empty;


        public ISN_NSKeyValueObject(string key, string value) {
            m_key = key;
            m_value = value;

        }


        /// <summary>
        /// Gets or sets the key of the Pair.
        /// </summary>
        public string Key {
            get { return m_key; }
        }

        /// <summary>
        /// Returns string representation of the value.
        /// </summary>
        public string StringValue {
            get { return m_value; }
        }

        /// <summary>
        /// Returns int representation of the value.
        /// </summary>
        public int IntValue {
            get {
                return Convert.ToInt32(m_value);
            }
        }

        /// <summary>
        /// Returns bool representation of the value.
        /// </summary>
        public bool BoolValue {
            get {
                return Convert.ToBoolean(m_value);
            }
        }

        /// <summary>
        /// Returns float representation of the value.
        /// </summary>
        public float FloatValue {
            get {
                return Convert.ToSingle(m_value);
            }
        }

        /// <summary>
        /// Returns long representation of the value.
        /// </summary>
        public long LongValue {
            get {
                return Convert.ToInt64(m_value);
            }
        }

        /// <summary>
        /// Returns ulong representation of the value.
        /// </summary>
        public ulong ULongValue {
            get {
                return Convert.ToUInt64(m_value);
            }
        }

        /// <summary>
        /// Returns Bytes array representation of the value.
        /// </summary>
        public byte[] BytesArrayValue {
            get {
                return Convert.FromBase64String(m_value);
            }
        }

        /// <summary>
        /// Returns DateTime representation of the value.
        /// </summary>
        public DateTime DateTimeValue {
            get {
                return SA_Unix_Time.ToDateTime(LongValue);
            }
        }

        /// <summary>
        /// Create an object from its JSON representation. Internally, this method uses the Unity serializer; 
        /// therefore the type you are creating must be supported by the serializer.
        /// It must be a plain class/struct marked with the Serializable attribute.Fields of the object must have types supported by the serializer. 
        /// Fields that have unsupported types, as well as private fields or fields marked with the NonSerialized attribute, will be ignored.
        /// </summary>
        public T GetObject<T>() {
            return JsonUtility.FromJson<T>(m_value);
        }
    }
}
