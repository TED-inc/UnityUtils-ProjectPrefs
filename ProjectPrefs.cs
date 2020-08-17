using UnityEditor;
using UnityEngine;

namespace TEDinc.Utils.ProjPrefs
{
    public static class ProjectPrefs
    {
        public static ProjectPrefsSO InBuild { get; private set; }

#if UNITY_EDITOR
        public static ProjectPrefsSO InEditor =>
            inEditor != null ?
                inEditor :
                FindInEditorSO();
        private static ProjectPrefsSO inEditor;
#endif

        public static void TryAdd(ProjectPrefsSO projectPrefsSO)
        {
            if (InBuild == null && projectPrefsSO.name.ToLower().Contains(nameof(InBuild).ToLower()))
                InBuild = projectPrefsSO;
#if UNITY_EDITOR
            else if (InEditor == null && projectPrefsSO.name.ToLower().Contains(nameof(InEditor).ToLower()))
                inEditor = projectPrefsSO;
#endif
        }

#if UNITY_EDITOR
        private static ProjectPrefsSO FindInEditorSO()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:scriptableobject"))
            {
                ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (so.GetType() == typeof(ProjectPrefsSO) && so.name.ToLower().Contains(nameof(InEditor).ToLower()))
                {
                    inEditor = so as ProjectPrefsSO;
                    return inEditor;
                }
            }

            return null;
        }
#endif
    }
}