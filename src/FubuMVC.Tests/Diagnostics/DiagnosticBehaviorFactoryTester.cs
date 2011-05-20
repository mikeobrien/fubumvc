﻿using System;
using FubuCore.Binding;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    public class DiagnosticBehaviorFactoryTester : InteractionContext<DiagnosticBehaviorFactory>
    {
        private ServiceArguments _serviceArguments;

        protected override void beforeEach()
        {
            _serviceArguments = new ServiceArguments();

            var db = MockRepository.GenerateMock<DiagnosticBehavior>(
                MockFor<IDebugReport>(), 
                MockFor<IDebugDetector>(), 
                MockFor<IUrlRegistry>(), 
                MockFor<IRequestHistoryCache>());

            MockFor<IContainerFacility>()
                .Expect(x => x.Get<DiagnosticBehavior>())
                .Return(db);

            MockFor<IContainerFacility>()
                .Expect(x => x.Get<IDebugReport>())
                .Return(MockFor<IDebugReport>());
        }

        [Test]
        public void if_services_has_output_writer_it_is_used()
        {
            var writer = MockFor<IOutputWriter>();
            writer.Expect(x => x.WriteHtml("fubu")).Repeat.Once();

            ClassUnderTest.BuildBehavior(_serviceArguments.With(writer), Guid.NewGuid());
            _serviceArguments.Get<IOutputWriter>().WriteHtml("fubu");

            writer.VerifyAllExpectations();
        }

        [Test]
        public void if_services_has_no_output_writer_it_requests_one_from_container_facility()
        {
            var container = MockFor<IContainerFacility>();
            
            container.Expect(x => x.Get<IOutputWriter>())
                .Return(Arg<IOutputWriter>.Is.Anything);
            
            ClassUnderTest.BuildBehavior(_serviceArguments, Guid.NewGuid());
            
            container.VerifyAllExpectations();            
        }
    }
}
