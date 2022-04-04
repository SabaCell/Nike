namespace Nike.Framework.Domain.EventSourcing;

public static class PrivateReflectionDynamicObjectExtensions
{
    public static dynamic AsDynamic(this object o)
    {
        return PrivateReflectionDynamicObject.WrapObjectIfNeeded(o);
    }
}