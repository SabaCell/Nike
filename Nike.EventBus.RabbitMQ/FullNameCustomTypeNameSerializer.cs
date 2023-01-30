using System;
using EasyNetQ;

namespace Nike.EventBus.RabbitMQ
{
    /// This custom ITypeNameSerializer changes the default behaviour of EasyNetQ TypeNameSerializer 
    /// which appends the assembly name to the serializer type name.
    /// This behaviour is based on the assumption that each application/microservice used it's own implemtation of Events/Commands types.
    /// Hint: EasyNetQ assumes that Events/Commands Dll is shared between applications/microservices.
    /// </summary>
    public abstract class FullNameCustomTypeNameSerializer : ITypeNameSerializer
    {
        public string Serialize(Type type)
        {
            return type.FullName;
        }

        public Type DeSerialize(string typeName)
        {
            try
            {
                var typeHelper = new TypeHelper();
                var type = typeHelper.GetType(typeName);
                var fullName = type.FullName;
                return type;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}