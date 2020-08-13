using TEDinc.Utils.SO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace TEDinc.Utils.ProjPrefs
{
    public static class ProjectPrefs
    {
        public static ProjectPrefsSO InBuild => inBuild;
        private static ProjectPrefsSO inBuild;

#if UNITY_EDITOR
        public static ProjectPrefsSO InEditor
        {
            get
            {
                if (inEditor == null)
                    inEditor = SOUtils.FindOrCraete<ProjectPrefsSO>
                        (path: $"Assets\\{ nameof(Utils)}\\{nameof(ProjectPrefs)}\\{nameof(Editor)}\\{nameof(ProjectPrefsSO)}{nameof(InEditor)}");

                return inEditor;
            }
        }
        private static ProjectPrefsSO inEditor;
#endif

        public static void TryAdd(ProjectPrefsSO projectPrefsSO)
        {
            if (inBuild == null && projectPrefsSO.name.ToLower().Contains(nameof(inBuild).ToLower()))
                inBuild = projectPrefsSO;
#if UNITY_EDITOR
            else if (inEditor == null && projectPrefsSO.name.ToLower().Contains(nameof(inEditor).ToLower()))
                inEditor = projectPrefsSO;
#endif
        }
    }
}