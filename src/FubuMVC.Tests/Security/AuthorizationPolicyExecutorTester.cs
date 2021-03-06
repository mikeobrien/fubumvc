﻿using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class when_executing_authorization_policies : InteractionContext<AuthorizationPolicyExecutor>
    {
        private IAuthorizationPolicy[] policies;
        private AuthorizationRight _answer;

        protected override void beforeEach()
        {

            var request = MockFor<IFubuRequest>();

            policies = Services.CreateMockArrayFor<IAuthorizationPolicy>(3);
            policies[0].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.Allow);
            policies[1].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.None);
            policies[2].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.None);
            _answer = ClassUnderTest.IsAuthorized(request, policies);
        }

        [Test]
        public void should_query_all_authorization_policies()
        {
            policies[0].VerifyAllExpectations();
            policies[1].VerifyAllExpectations();
            policies[2].VerifyAllExpectations();
        }

        [Test]
        public void should_return_the_combined_result_of_all_policies()
        {
            _answer.ShouldEqual(AuthorizationRight.Allow);
        }
    }
}