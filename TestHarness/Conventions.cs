using AssemblyBottle;
using FubuMVC.Core;

namespace TestHarness
{
    public class Conventions : FubuRegistry
    {
        public Conventions()
        {
            Actions.IncludeTypesNamed(x => x.EndsWith("Handler"));

            Routes
                .HomeIs<IndexHandler>(x => x.ExecuteGet())
                .IgnoreNamespaceForUrlFrom<Conventions>()
                .IgnoreMethodSuffix("Execute")
                .IgnoreClassSuffix("GetHandler")
                .ConstrainToHttpMethod(action => action.HandlerType.Name.EndsWith("GetHandler"), "GET");

            // Configure the bottle
            Import<TestBottle>(x => x
                .AtUrl("bottletest")
                .WithDateFormat("MM-dd-yyyy"));

            Views.TryToAttachWithDefaultConventions();

            Media.ApplyContentNegotiationToActions(x => 
                !x.HasAnyOutputBehavior() && 
                x.HandlerType.Assembly == GetType().Assembly);
        }
    }
}