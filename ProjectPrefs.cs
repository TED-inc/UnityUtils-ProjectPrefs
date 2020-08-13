namespace TEDinc.Utils.ProjPrefs
{
    public static class ProjectPrefs
    {
        public static ProjectPrefsSO InBuild { get; private set; }

#if UNITY_EDITOR
        public static ProjectPrefsSO InEditor { get; private set; }
#endif

        public static void TryAdd(ProjectPrefsSO projectPrefsSO)
        {
            if (InBuild == null && projectPrefsSO.name.ToLower().Contains(nameof(InBuild).ToLower()))
                InBuild = projectPrefsSO;
#if UNITY_EDITOR
            else if (InEditor == null && projectPrefsSO.name.ToLower().Contains(nameof(InEditor).ToLower()))
                InEditor = projectPrefsSO;
#endif
        }
    }
}