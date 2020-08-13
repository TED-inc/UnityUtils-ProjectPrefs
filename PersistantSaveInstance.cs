using System.Collections.Generic;
using UnityEngine;
using TEDinc.Utils.Components;
using Object = UnityEngine.Object;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using TEDinc.Utils.MathExt;
#endif

namespace TEDinc.Utils.PersistantSave
{
    public sealed class PersistantSaveInstance : MonoBehaviour, IInitable
    {
        [SerializeField]
        private string _inPersistantPath = nameof(PersistantSave);
        [SerializeField]
        private List<Object> _objectsToPersist;
        [Header("Encryption params")]
        [SerializeField]
        private int _hashSalt = 12345678;
        [SerializeField]
        private string _encryptKey = "abcd1234ABCD5678";

        private static bool initialized = false;
        private static int intTryCounter = 0;

        #region InvokeOfLoadAndSave
        private void OnDisable() =>
            PersistantSave.SaveToPersistant();

        private void OnDestroy() =>
            PersistantSave.SaveToPersistant();

        private void OnApplicationQuit() =>
            PersistantSave.SaveToPersistant();

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                PersistantSave.SaveToPersistant();
            else if (initialized)
                PersistantSave.LoadFromPersistant();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && initialized)
                PersistantSave.LoadFromPersistant();
            else
                PersistantSave.SaveToPersistant();
        }
        #endregion

        public void Init()
        {
            if (initialized)
                return;
            if (_objectsToPersist == null || _objectsToPersist.Count == 0)
            {
                if (intTryCounter > 100)
                {
                    Debug.LogError("[PS] Init failed");
                    return;
                }    

                Invoke(nameof(Init), 0f);
                intTryCounter++;
                return;
            }
            initialized = true;

            PersistantSave.Init(_hashSalt, _encryptKey, _inPersistantPath, _objectsToPersist);
            PersistantSave.LoadFromPersistant();
        }

        

        #region editorTools
#if UNITY_EDITOR
        [ContextMenu(nameof(AddPersistantSave))]
        private void AddPersistantSave()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:scriptableobject"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.ToLower().Contains("editor"))
                    continue;

                Object obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (!_objectsToPersist.Contains(obj) && obj.GetType().GetCustomAttributes(typeof(PersistantSaveAttribute), true).Length > 0)
                    _objectsToPersist.Add(obj);
            }
        }

        [ContextMenu(nameof(RandomizeEncryption))]
        private void RandomizeEncryption()
        {
            if (!Application.isPlaying && !EditorUtility.DisplayDialog(
                "Randomize encryption keys confirmation",
                "This action will break your persistant saves",
                "Confirm", "Cancel"))
                return;

            _hashSalt = RandomExt.Random.Next();
            _encryptKey = RandomExt.NextString(16);
            if (!Application.isPlaying)
                if (EditorUtility.DisplayDialog(
                    "Ecryption keys changed",
                    "We recomend clear persistan saves",
                    "Clear", "Cancel"))
                    Tools.ClearPersistantSave.Clear();


        }

        private static bool encryptionChecked;

        private void Start()
        {
            if (encryptionChecked)
                return;
            encryptionChecked = true;

            if (_hashSalt == 12345678)
                Debug.LogError($"[PS] You must change \"{ObjectNames.NicifyVariableName(nameof(_hashSalt))}\"");
            if (_encryptKey == "abcd1234ABCD5678")
                Debug.LogError($"[PS] You must change \"{ObjectNames.NicifyVariableName(nameof(_encryptKey))}\"");
        }
#endif
        #endregion
    }
}
