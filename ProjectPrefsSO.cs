using UnityEngine;
using TEDinc.Utils.PersistantSave;
using TEDinc.Utils.ProjPrefs.Implementation;

namespace TEDinc.Utils.ProjPrefs
{
    [PersistantSave]
    [CreateAssetMenu(fileName = nameof(ProjectPrefsSO), menuName = nameof(TEDinc) + "/" + nameof(Utils) + "/" + nameof(ProjPrefs))]
    public class ProjectPrefsSO : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        private bool selfAddableInEditMode = false;
#endif
        public SerizDictOfString Strings => strings;
        public SerizDictOfInt Ints => ints;
        public SerizDictOfFloat Floats => floats;
        public SerizDictOfBool Bools => bools;

        [SerializeField]
        private SerizDictOfString strings;
        [SerializeField]
        private SerizDictOfInt ints;
        [SerializeField]
        private SerizDictOfFloat floats;
        [SerializeField]
        private SerizDictOfBool bools;

        private void Awake()
        {
            if (strings == null)
                strings = new SerizDictOfString();
            if (ints == null)
                ints = new SerizDictOfInt();
            if (floats == null)
                floats = new SerizDictOfFloat();
            if (bools == null)
                bools = new SerizDictOfBool();

#if UNITY_EDITOR
            if (selfAddableInEditMode)
                TryAssign();
#endif
        }


        [ContextMenu(nameof(ResetData))]
        public void ResetData()
        {
            strings = new SerizDictOfString();
            ints = new SerizDictOfInt();
            floats = new SerizDictOfFloat();
            bools = new SerizDictOfBool();
        }

        [PersistantSaveOnLoad]
        private void TryAssign() =>
            ProjectPrefs.TryAdd(this);



#if UNITY_EDITOR
        private void OnEnable()
        {
            if (selfAddableInEditMode)
                TryAssign();
        }
#endif
    }
}