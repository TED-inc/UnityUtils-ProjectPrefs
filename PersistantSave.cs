using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using TEDinc.Utils.Encrition;
using Object = UnityEngine.Object;

namespace TEDinc.Utils.PersistantSave
{
    public static class PersistantSave
    {
        private static List<int> savedObjectHashes;

        public static void Init(int hashSalt, string key, string inPersistantPath, IEnumerable<Object> objectsToPersist) =>
            DataCashe.Init(hashSalt, key, inPersistantPath, objectsToPersist);
        public static void LoadFromPersistant()
        {
            if (!DataCashe.initialized)
                return;

            if (savedObjectHashes == null)
                savedObjectHashes = new List<int>();

            for (int i = 0; i < DataCashe.objectsToPersist.Count; i++)
            {
                string dataPath = GetPersistantPath(DataCashe.objectsToPersist[i]);
                string hashPath = GetPersistantPathHash(DataCashe.objectsToPersist[i]);

                CheckDirectories();
                if (!LoadObject())
                    SaveObjToPersistant(i);



                void CheckDirectories()
                {
                    string dirDataPath = Path.GetDirectoryName(dataPath);
                    string dirHashPath = Path.GetDirectoryName(hashPath);

                    if (!Directory.Exists(dirDataPath))
                        Directory.CreateDirectory(dirDataPath);
                    if (!Directory.Exists(dirHashPath))
                        Directory.CreateDirectory(dirHashPath);
                }

                bool LoadObject()
                {
                    string hash = LoadFile(hashPath);
                    if (hash == null)
                        return false;

                    int hashSaved = Convert.ToInt32(hash) ^ DataCashe.hashSalt;
                    bool firstLoad = savedObjectHashes.Count <= i;

                    if (firstLoad || hashSaved != savedObjectHashes[i])
                    {
                        string data = LoadFile(dataPath);
                        if (data == null)
                            return false;

                        int hashOfFile = data.GetHashCode();

                        if (firstLoad)
                            savedObjectHashes.Add(hashSaved);
                        else
                            savedObjectHashes[i] = hashSaved;

                        if (hashOfFile == hashSaved)
                        {
                            JsonUtility.FromJsonOverwrite(data, DataCashe.objectsToPersist[i]);
                            Debug.Log("[PS] Load:\n " + DataCashe.objectsToPersist[i].name);
                            InvokePersistantSaveOnLoadMethods();
                        }
                        else
                        {
                            Debug.LogError("[PS] Load failed. Invalid file:\n " + DataCashe.objectsToPersist[i].name);
                            return false;
                        }
                    }
                    else
                        Debug.Log("[PS] Load not necessary for:\n " + DataCashe.objectsToPersist[i].name);
                    return true;



                    string LoadFile(string path)
                    {
                        string data = null;

                        if (File.Exists(path))
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            FileStream file = File.Open(path, FileMode.Open);
                            data = (string)bf.Deserialize(file);
                            data = EncryptUtils.DecryptString(data, DataCashe.key);
                            file.Close();
                        }

                        return data;
                    }
                }

                void InvokePersistantSaveOnLoadMethods()
                {
                    foreach (MethodInfo method in DataCashe.objectsToPersist[i]
                        .GetType()
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                        .Where(method =>
                            method.GetCustomAttributes(typeof(PersistantSaveOnLoadAttribute), false).Length > 0))
                    {
                        method.Invoke(DataCashe.objectsToPersist[i], null);
                    }
                }
            }
        }

        public static void SaveToPersistant()
        {
            if (!DataCashe.initialized)
                return;

            for (int i = 0; i < DataCashe.objectsToPersist.Count; i++)
                SaveObjToPersistant(i);
        }

        private static void SaveObjToPersistant(int index)
        {
            if (DataCashe.objectsToPersist[index] == null)
            {
                Debug.LogWarning($"[PS] Saving failed. Object[{index}] is null");
                return;
            }

            string json = JsonUtility.ToJson(DataCashe.objectsToPersist[index]);
            SaveFile(GetPersistantPath(DataCashe.objectsToPersist[index]), json);
            SaveFile(GetPersistantPathHash(DataCashe.objectsToPersist[index]), (json.GetHashCode() ^ DataCashe.hashSalt).ToString());

            Debug.Log("[PS] Save:\n " + DataCashe.objectsToPersist[index].name);



            void SaveFile(string path, string data)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(path);
                data = EncryptUtils.EncryptString(data, DataCashe.key);
                bf.Serialize(file, data);
                file.Close();
            }
        }

        private static string GetPersistantPath(Object obj) =>
            $"{Application.persistentDataPath}/{DataCashe.inPersistantPath}/{obj.name}.pso";
        private static string GetPersistantPathHash(Object obj) =>
            GetPersistantPath(obj) + ".hash";


        private static class DataCashe
        {
            internal static bool initialized { get; private set; }
            internal static int hashSalt { get; private set; }
            internal static string key { get; private set; }
            internal static string inPersistantPath { get; private set; }
            internal static List<Object> objectsToPersist { get; private set; }

            internal static void Init(int hashSalt, string key, string inPersistantPath, IEnumerable<Object> objectsToPersist)
            {
                if (initialized)
                    return;
                initialized = true;

                DataCashe.hashSalt = hashSalt;
                DataCashe.key = key;
                DataCashe.inPersistantPath = inPersistantPath;
                DataCashe.objectsToPersist = objectsToPersist.ToList();
            }
        }
    }
}
