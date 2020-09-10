using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nike.EventBus.Kafka
{
    public class TypeHelper
    {
        private readonly WeakReference<IList<Type>> _types = new WeakReference<IList<Type>>(null);
        private readonly string exceptTypesRegex = "(System.*|Microsoft.*)";

        private IEnumerable<Type> Types
        {
            get
            {
                IList<Type> types;
                if (!_types.TryGetTarget(out types))
                {
                    types = new List<Type>();
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var count = assemblies.Length;
                    foreach (var assembly in assemblies)
                    {
                        var assemblyName = assembly.GetName().FullName;

                        if (!assembly.IsDynamic)
                        {
                            Type[] exportedTypes = null;
                            try
                            {
                                exportedTypes = assembly.GetExportedTypes();
                            }
                            catch (ReflectionTypeLoadException e)
                            {
                                exportedTypes = e.Types;
                            }

                            if (exportedTypes != null)
                                foreach (var exportedType in exportedTypes)
                                    if (!Regex.IsMatch(exportedType.FullName, exceptTypesRegex))
                                        types.Add(exportedType);
                        }
                    }

                    _types.SetTarget(types);
                }

                return types;
            }
        }

        public Type GetType(string typeFullName)
        {
            return Types.FirstOrDefault(type => type.FullName == typeFullName);
        }
    }
}