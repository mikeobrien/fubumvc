using System;
using System.IO;
using System.Web;
using FubuMVC.Core;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark;

[assembly: FubuModule]

namespace AssemblyBottle
{
    public class Configuration
    {
        public string Url { get; set; }
        public string DateFormat { get; set; }
    }

    public interface ITimeService { string GetCurrentTime(); }

    public class TimeService : ITimeService
    {
        private readonly Configuration _configuration;
        public TimeService(Configuration configuration) { _configuration = configuration; }
        public string GetCurrentTime() { return DateTime.Now.ToString(_configuration.DateFormat); }
    }

    public class TimeResponse { public string Time { get; set; }}
    public class TimeGetHandler
    {
        private readonly ITimeService _timeService;
        public TimeGetHandler(ITimeService timeService) { _timeService = timeService; }
        public TimeResponse Execute() { return new TimeResponse { Time = _timeService.GetCurrentTime() }; } 
    }

    public class TestBottle : IFubuRegistryExtension
    {
        private readonly Configuration _configuration = new Configuration();

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            File.AppendAllText(@"c:\windows\temp\bottlestartup.log", string.Format("{0}: IFubuRegistryExtension.Configure()\r\n", DateTime.Now));
            registry.Import(new BottleRegistry(_configuration), _configuration.Url);
        }

        public TestBottle AtUrl(string url)
        {
            _configuration.Url = url;
            return this;
        }

        public TestBottle WithDateFormat(string format)
        {
            _configuration.DateFormat = format;
            return this;
        }
    }

    public class BottleRegistry : FubuRegistry
    {
        public BottleRegistry(Configuration configuration)
        {
            Actions.IncludeTypesNamed(x => x.EndsWith("Handler"));

            Routes
                .IgnoreNamespaceForUrlFrom<BottleRegistry>()
                .IgnoreMethodSuffix("Execute")
                .IgnoreClassSuffix("GetHandler")
                .ConstrainToHttpMethod(action => action.HandlerType.Name.EndsWith("GetHandler"), "GET");

            Import<SparkEngine>();
            Views.TryToAttachWithDefaultConventions();

            Media.ApplyContentNegotiationToActions(x => x.HandlerType.Assembly == GetType().Assembly);

            Services(x => {
                x.AddService(configuration);
                x.AddService(typeof(ITimeService), new ObjectDef(typeof(TimeService)));
            });
        }
    }
}