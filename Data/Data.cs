using System.Collections.Generic;
using System;

namespace Ferrum.Data
{
    public class InvalidDataSettingException: Exception
    {
        public InvalidDataSettingException(string message)
        : base(message) { }
    }

    [Serializable]
    public class ADataType
    {
        // Add your functions or props or vars here
    }

    [Serializable]
    public class DTString: ADataType
    {
        public string str = "";
    }

    [Serializable]
    public class DTFloat : ADataType
    {
        private float val = 0f;
        public float Value { get { return val; } set {
            val = value;
        } }

        public float Modify (float by, float min = float.MinValue, float max = float.MaxValue)
        {
            return Math.SetClamped(ref val, by, min, max);
        }
    }


    [Serializable]
    public class DTBoolean : ADataType
    {
        private bool val = false;
        public bool Value
        {
            get { return val; }
            set
            {
                val = value;
            }
        }
    }


    /// <summary>
    /// An objects that hold assigned data
    /// 
    /// Use extension classes to add common setters / getters
    /// </summary>
    [Serializable]
    public class AssignedData
    {
        private Dictionary<string, ADataType> data = new();

        public T GetDT<T>(string key, bool createIfNDef = true) where T : ADataType, new()
        {
            if (data.TryGetValue(key, out var found))
            {
                if (found is T typedFound)
                {
                    return typedFound;
                }
                else
                {
                    throw new Exception("Key is found but value type doesn't match the given type! Ensure you are using the correct key and type.");
                }
            }
            else
            {
                if(createIfNDef)
                {
                    T newInstance = new ();
                    data[key] = newInstance;
                    return newInstance;
                }
                else
                {
                    return null;
                }
            }
        }

        //---

        public void SetString(string key, string value)
        {
            GetDT<DTString>(key).str = value;
        }

        public string GetString(string key, string def = null)
        {
            DTString dtstr = GetDT<DTString>(key, false);
            return dtstr != null ? dtstr.str : def;
        }

        public void SetFloat(string key, float value)
        {
            GetDT<DTFloat>(key).Value = value;
        }

        public float ModifyFloat(string key, float by, float min = float.MinValue, float max = float.MaxValue)
        {
            return GetDT<DTFloat>(key).Modify(by, min, max);
        }

        public float GetFloat(string key, float def = 0f)
        {
            DTFloat dtflt = GetDT<DTFloat>(key, false);
            return dtflt != null ? dtflt.Value : def;
        }
        public void SetBoolean(string key, bool value)
        {
            GetDT<DTBoolean>(key).Value = value;
        }

        public bool GetBoolean(string key, bool def = false)
        {
            DTBoolean dtflt = GetDT<DTBoolean>(key, false);
            return dtflt != null ? dtflt.Value : def;
        }

    }
}