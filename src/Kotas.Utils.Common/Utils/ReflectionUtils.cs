using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kotas.Utils.Common.Utils
{
    public static class ReflectionUtils
    {
        public static Assembly[] GetAllAssembliesWithoutEntry()
        {
            var referencedAssemblies =Assembly
                .GetEntryAssembly()?
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .ToArray();

            return referencedAssemblies;
        }

        public static Assembly[] GetAllAssemblies()
        {
            var referencedAssemblies =Assembly
                .GetEntryAssembly()?
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .ToArray();

            return referencedAssemblies?.Union(new[] {Assembly.GetEntryAssembly()}).ToArray();
        }

        public static IEnumerable<object> CreateInstancesImplementingInterface(Type interfaceType, Assembly[] assemblies)
        {
            Validations.ValidateInput(interfaceType, nameof(interfaceType));
            Validations.ValidateInput(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies)
            {
                foreach (TypeInfo ti in assembly.DefinedTypes)
                {
                    if (ti.ImplementedInterfaces.Contains(interfaceType))
                    {
                        yield return assembly.CreateInstance(ti.FullName ?? string.Empty);
                    }  
                }
            }
        }

        public static IEnumerable<TInterface> CreateInstancesImplementingInterface<TInterface>(Assembly[] assemblies)
        {
            Validations.ValidateInput(assemblies, nameof(assemblies));

            var interfaceType = typeof(TInterface);

            foreach (var assembly in assemblies)
            {
                foreach (TypeInfo ti in assembly.DefinedTypes)
                {
                    if (ti.ImplementedInterfaces.Contains(interfaceType))
                    {
                        yield return (TInterface)assembly.CreateInstance(ti.FullName ?? string.Empty);
                    }  
                }
            }
        }
    }
}
