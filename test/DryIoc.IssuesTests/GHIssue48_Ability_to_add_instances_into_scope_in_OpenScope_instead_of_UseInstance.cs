using NUnit.Framework;

namespace DryIoc.IssuesTests
{
    [TestFixture]
    public class GHIssue48_Ability_to_add_instances_into_scope_in_OpenScope_instead_of_UseInstance
    {
        [Test]
        public void Add_one_instance_to_scope()
        {
            var container = new Container();
            container.Register<A>();

            var b = new B();
            using (var scope = container.OpenScopeWithPresetServices("ScopeName", instances: b))
            {
                var a = scope.Resolve<A>();
                Assert.IsNotNull(a);
            }

            Assert.Throws<ContainerException>(() => container.Resolve<A>());
        }

        public class A
        {
            public A(B b) { }
        }

        public class B { }
    }
}
