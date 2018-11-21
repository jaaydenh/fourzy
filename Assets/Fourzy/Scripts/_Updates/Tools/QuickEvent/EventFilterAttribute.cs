using UnityEngine;

namespace ByteSheep.Events
{
    [System.Flags]
    public enum TargetFilter
    {
        Static = StaticField | StaticProperty | StaticMethod,
        StaticField = 1,
        StaticProperty = 2,
        StaticMethod = 4,
        Dynamic = DynamicField | DynamicProperty | DynamicMethod,
        DynamicField = 8,
        DynamicProperty = 16,
        DynamicMethod = 32,
    };

    [System.Flags]
    public enum TargetTypeFilter
    {
        Void = 1,
        String = 2,
        Int = 4,
        Float = 8,
        Bool = 16,
        Color = 32,
        Vector2 = 64,
        Vector3 = 128,
        Object = 256,
        GameObject = 512,
        Transform = 1024,
        Enum = 2048,
        Any = 4095
    };

    public class EventFilterAttribute : PropertyAttribute
    {
        public TargetFilter targetFilters = TargetFilter.Static | TargetFilter.Dynamic;
        public TargetTypeFilter typeFilters = TargetTypeFilter.Any;

        public EventFilterAttribute(TargetFilter targetFilters)
        {
            this.targetFilters = targetFilters;
        }

        public EventFilterAttribute(TargetTypeFilter typeFilters)
        {
            this.typeFilters = typeFilters;
        }

        public EventFilterAttribute(TargetFilter targetFilters, TargetTypeFilter typeFilters)
        {
            this.targetFilters = targetFilters;
            this.typeFilters = typeFilters;
        }
    }
}