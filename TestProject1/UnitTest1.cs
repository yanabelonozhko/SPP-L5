using DepInjContainer;

namespace TestProject1
{
    public class Tests
    {
        [Test]
        public void TDefaultRegistration()
        {
            var dependencies = new DepConfig();
            dependencies.Register<IService1, Service1>();
            dependencies.Register<AbstractService2, Service2>();

            var provider = new DepProvider(dependencies);
            var service = provider.Resolve<IService1>();
            Assert.That(service.GetType().ToString(), Is.EqualTo("TestProject1.Service1"));
        }

        [Test]
        public void TAbstractRegistration()
        {
            var dependencies = new DepConfig();
            dependencies.Register<AbstractService2, Service2>();

            var provider = new DepProvider(dependencies);
            var service = provider.Resolve<AbstractService2>();
            Assert.That(service.GetType().ToString(), Is.EqualTo("TestProject1.Service2"));
        }


        [Test]
        public void TSelfRegistration()
        {
            var dependencies = new DepConfig();
            dependencies.Register<Service1, Service1>();

            var provider = new DepProvider(dependencies);
            var service = provider.Resolve<Service1>();
            Assert.That(service.GetType().ToString(), Is.EqualTo("TestProject1.Service1"));
        }

        [Test]
        public void TRecursiveRegistration()
        {
            var dependencies = new DepConfig();
            dependencies.Register<IService, ServiceImpl>();
            dependencies.Register<IRepository, RepositoryImpl>();

            var provider = new DepProvider(dependencies);
            var service = provider.Resolve<IService>();
            Assert.That(service.GetType().ToString(), Is.EqualTo("TestProject1.ServiceImpl"));
        }

        [Test]
        public void TSingletonRegistration()
        {
            var dependencies = new DepConfig();
            dependencies.Register<IService1, Service1>(LivingTime.Singleton);

            var provider = new DepProvider(dependencies);
            var service = provider.Resolve<IService1>();
            service.SomeData = "singleton";
            var newService = provider.Resolve<IService1>();
            Assert.That(newService.SomeData, Is.EqualTo("singleton"));
        }

        [Test]
        public void TManyImplementations()
        {
            var dependencies = new DepConfig();
            dependencies.Register<IService1, Service1>();
            dependencies.Register<IService1, Service3>();

            var provider = new DepProvider(dependencies);
            IEnumerable<IService1> services = provider.Resolve<IEnumerable<IService1>>();
            Assert.That(services.Count(), Is.EqualTo(2));
        }

        [Test]
        public void TGenericRegistration()
        {
            var dependencies = new DepConfig();
            dependencies.Register<IRepository, RepositoryImpl>();
            dependencies.Register<IService5<IRepository>, ServiceImpl5<IRepository>>();

            var provider = new DepProvider(dependencies);
            var service = provider.Resolve<IService5<IRepository>>();
            Assert.That(service.GetType().ToString(), Is.EqualTo("TestProject1.ServiceImpl5`1[TestProject1.IRepository]"));
        }

        [Test]
        public void TOpenGeneric()
        {
            var dependencies = new DepConfig();
            dependencies.Register(typeof(IService5<>), typeof(ServiceImpl5<>));
            dependencies.Register<IRepository, RepositoryImpl>();

            var provider = new DepProvider(dependencies);
            var service = provider.Resolve<IService5<IRepository>>();
            Assert.That(service.GetType().ToString(), Is.EqualTo("TestProject1.ServiceImpl5`1[TestProject1.IRepository]"));
        }

        [Test]
        public void TEnum()
        {
            var dependencies = new DepConfig();
            dependencies.Register<IService1, Service1>(LivingTime.InstancePerDependency, ServiceImplementations.First);
            dependencies.Register<IService1, Service3>(LivingTime.InstancePerDependency, ServiceImplementations.Second);

            var provider = new DepProvider(dependencies);
            var first = provider.Resolve<IService1>(ServiceImplementations.First);
            var second = provider.Resolve<IService1>(ServiceImplementations.Second);
            Assert.That(first.GetType().ToString(), Is.EqualTo("TestProject1.Service1"));
            Assert.That(second.GetType().ToString(), Is.EqualTo("TestProject1.Service3"));
        }
    }

    enum ServiceImplementations
    {
        First,
        Second
    }

    interface IService
    {
    }

    class ServiceImpl : IService
    {
        public ServiceImpl(IRepository repository)
        {
        }

        public ServiceImpl(int f, int d)
        {
        }
    }

    interface IRepository
    {
    }

    class RepositoryImpl : IRepository
    {
        public RepositoryImpl()
        {
        }
    }

    public interface IService1
    {
        public string SomeData { get; set; }
    }

    public class Service1 : IService1
    {
        public string SomeData { get; set; }

        public void DoSomething()
        {
            Console.WriteLine("Do something");
        }
    }

    public class Service3 : IService1
    {
        public string SomeData { get; set; }
    }

    public abstract class AbstractService2
    {
        public void DoSomething()
        {
            Console.WriteLine("Do something");
        }
    }

    public class Service2 : AbstractService2
    {
    }

    interface IService5<TRepository> where TRepository : IRepository
    {
    }

    class ServiceImpl5<TRepository> : IService5<TRepository>
        where TRepository : IRepository
    {
        public ServiceImpl5(TRepository repository)
        {
        }
    }
}