using System;
using System.Collections.Generic;
using UnityEngine;
using TEDinc.Utils.Json;
using TEDinc.Utils.Encrition;

namespace TEDinc.Utils.ProjPrefs.Implementation
{
    [Serializable]
    public sealed class SerizDictOfInt : SerializebleDictBase<EncryptedInt, int> { }

    [Serializable]
    public sealed class SerizDictOfBool : SerializebleDictBase<EncryptedBool, bool> { }

    [Serializable]
    public sealed class SerizDictOfFloat : SerializebleDictBase<EncryptedFloat, float> { }

    [Serializable]
    public sealed class SerizDictOfString : SerializebleDictBase<EncryptedString, string> { }
    

    [Serializable]
    public abstract class SerializebleDictBase<TEncrypt, TValue> where TEncrypt : IEncrypted<TValue>, new ()
    {
        [SerializeField, TextArea(3, 10)]
        private string json;
        private Dictionary<string, TEncrypt> dict;

        public bool HasKey(string key)
        {
            if (string.IsNullOrEmpty(json))
                json = JsonHelper.ToJson(new Dictionary<string, TEncrypt>());
            if (dict == null || dict.Count == 0)
                dict = JsonHelper.FromJson<Dictionary<string, TEncrypt>>(json);

            if (string.IsNullOrEmpty(key))
                return false;
            else
                return dict.ContainsKey(key);
        }

        public TValue Get(string key) =>
            HasKey(key) ? dict[key].Get() : ((TEncrypt)Activator.CreateInstance(typeof(TEncrypt))).Get();

        public void Set(string key, TValue value)
        {
            if (HasKey(key))
                dict[key].Set(value);
            else
            {
                TEncrypt encrypt = new TEncrypt();
                encrypt.Set(value);
                dict.Add(key, encrypt);
            }

            json = JsonHelper.ToJson(dict);
        }


        public SerializebleDictBase() =>
            HasKey("");
    }
}
