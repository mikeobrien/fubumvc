using AssemblyBottle;

namespace TestHarness
{
    // A handler that takes in a dependency from the bottle

    public class TimeResponse
    {
        public string Time { get; set; }
    }

    public class TimeGetHandler
    {
        private readonly ITimeService _timeService;

        public TimeGetHandler(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public TimeResponse Execute()
        {
            return new TimeResponse { Time = _timeService.GetCurrentTime() };
        }
    }
}