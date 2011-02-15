using FubuMVC.Core;
using FubuMVC.HelloSpark.Controllers;
using Spark.Web.FubuMVC;
using Spark.Web.FubuMVC.ViewCreation;

namespace FubuMVC.HelloSpark
{
    public class HelloSparkRegistry : FubuRegistry
    {
        public HelloSparkRegistry()
        {
            IncludeDiagnostics(true);

            Applies
                .ToThisAssembly();

            Actions
                .IncludeClassesSuffixedWithController();

            this.Spark(spark =>
            {
                spark
                    .Settings
                    .AddViewFolder("/Features/");

                spark
                    .Policies
                    .Add<HelloSparkJavaScriptViewPolicy>()
                    .Add<HelloSparkPolicy>();

                spark
                    .Output
                    .ToJavaScriptWhen(call => call.HasOutput && call.OutputType().Equals(typeof(JavaScriptResponse)));

            });

            Routes
                .UrlPolicy<HelloSparkUrlPolicy>()
                .HomeIs<AirController>(c => c.TakeABreath());

            Output
                .ToJson
                .WhenTheOutputModelIs<JsonResponse>();
        }
    }
}
