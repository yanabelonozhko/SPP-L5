using System.Collections.Concurrent;

namespace DepInjContainer
{
    public class DepConfig
    {
        public readonly ConcurrentDictionary<Type, ImplInfo> Services = new();
        public readonly ConcurrentDictionary<Type, List<ImplInfo>> EnumerableServices = new();

        public void Register<TDependency, TImplementation>(LivingTime timeToLive = LivingTime.InstancePerDependency, Enum? index = null)
        {
            Register(typeof(TDependency), typeof(TImplementation), timeToLive, index);
        }

        public void Register(Type TDependency, Type TImplementation, LivingTime timeToLive = LivingTime.InstancePerDependency, Enum? index = null)
        {
            if (Services.ContainsKey(TDependency))
                EnumerableServices[TDependency].Add(new ImplInfo(index, timeToLive, TImplementation));
            else
            {
                EnumerableServices[TDependency] = new List<ImplInfo>()
                { new (index, timeToLive, TImplementation) };
                Services[TDependency] = new ImplInfo(index, timeToLive, TImplementation);
            }
        }
    }
}
