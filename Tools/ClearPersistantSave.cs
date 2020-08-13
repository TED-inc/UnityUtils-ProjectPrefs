#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace TEDinc.Utils.PersistantSave.Tools
{
    public sealed class ClearPersistantSave
    {
        [MenuItem(nameof(Tools) + "/" + nameof(ClearPersistantSave))]
        public static void Clear()
        {
            foreach (string path in Directory.GetDirectories(Application.persistentDataPath))
            {
                if (!path.EndsWith("Unity"))
                    Directory.Delete(path, true);
            }
            foreach (string path in Directory.GetFiles(Application.persistentDataPath))
            {
                if (!path.EndsWith("Unity"))
                    File.Delete(path);
            }
        }
    }
}
#endif