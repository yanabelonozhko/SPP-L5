using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepInjContainer
{
    public class DepProvider
    {
        private readonly DepConfig _dependencies;
        private readonly ConcurrentDictionary<Type, object> _singletonDependency = new();

        public DepProvider(DepConfig dependencies)
        {
            _dependencies = dependencies;
        }

        public T Resolve<T>(Enum? index = null)
        {
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return (T)ResolveEnum(typeof(T));
            }

            if (index != null)
            {
                return (T)Resolve(typeof(T), index);
            }

            return (T)Resolve(typeof(T));
        }

        private object Resolve(Type type, Enum index)
        {
            foreach (var implementation in _dependencies.EnumerableServices[type])
            {
                if (implementation.Index.Equals(index))
                    return CreateInstance(type, implementation);
            }

            return null;
        }

        private object Resolve(Type type)
        {
            if (!_dependencies.EnumerableServices.TryGetValue(type, out List<ImplInfo> implementation))
            {
                if (type.IsGenericType)
                {
                    if (!_dependencies.EnumerableServices.TryGetValue(type.GetGenericTypeDefinition(),
                            out implementation))
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return CreateInstance(type, implementation[0]);
        }


        private IEnumerable<object> ResolveEnum(Type type)
        {
            var implementations = CreateList(type.GenericTypeArguments[0]);
            foreach (var implementation in _dependencies.EnumerableServices[type.GenericTypeArguments[0]])
            {
                implementations.Add(CreateInstance(type, implementation));
            }

            return (IEnumerable<object>)implementations;
        }

        private object CreateInstance(Type type, ImplInfo implementation)
        {
            if (_singletonDependency.ContainsKey(implementation.ImplType))
            {
                return _singletonDependency[implementation.ImplType];
            }

            var instanceType = implementation.ImplType;
            if (instanceType.IsGenericTypeDefinition)
            {
                instanceType = instanceType.MakeGenericType(type.GenericTypeArguments);
            }

            var constructors = instanceType.GetConstructors();
            var constructor = constructors[0];
            foreach (var constructorInfo in constructors)
            {
                constructor = constructor.GetParameters().Where(p => p.ParameterType.IsInterface).ToList().Count >
                              constructorInfo.GetParameters().Where(p => p.ParameterType.IsInterface).ToList().Count
                    ? constructor
                    : constructorInfo;
            }

            var parameters = constructor.GetParameters();
            object instance;
            if (parameters.Length == 0)
            {
                instance = Activator.CreateInstance(instanceType);
                if (implementation.TimeToLive == LivingTime.Singleton)
                {
                    _singletonDependency[instanceType] = instance;
                }

                return instance;
            }

            List<object> initializedParameters = new List<object>(parameters.Length);
            foreach (var param in parameters)
            {
                initializedParameters.Add(Resolve(param.ParameterType));
            }


            instance = constructor.Invoke(initializedParameters.ToArray());

            if (implementation.TimeToLive == LivingTime.Singleton)
            {
                _singletonDependency[instanceType] = instance;
            }

            return instance;
        }

        public IList CreateList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }
    }
}
