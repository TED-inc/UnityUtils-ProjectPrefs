using System;

namespace TEDinc.Utils.PersistantSave
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class PersistantSaveAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PersistantSaveOnLoadAttribute : Attribute { }
}
