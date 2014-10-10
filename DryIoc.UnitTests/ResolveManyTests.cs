﻿using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc.UnitTests.CUT;
using NUnit.Framework;

namespace DryIoc.UnitTests
{
    [TestFixture]
    public class ResolveManyTests
    {
        [Test]
        public void Return_empty_collection_for_registered_service()
        {
            var container = new Container();

            var items = container.ResolveMany<IService>();

            Assert.That(items.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Return_one_item_collection()
        {
            var container = new Container();
            container.Register<IService, Service>();

            var items = container.ResolveMany<IService>().ToArray();

            Assert.That(items.Length, Is.EqualTo(1));
            Assert.That(items[0], Is.InstanceOf<Service>());
        }

        [Test]
        public void Return_two_items_in_order_of_registration()
        {
            var container = new Container();
            container.Register<IService, Service>();
            container.Register<IService, AnotherService>();

            var items = container.ResolveMany<IService>().ToArray();

            Assert.That(items.Length, Is.EqualTo(2));
            Assert.That(items[0], Is.InstanceOf<Service>());
            Assert.That(items[1], Is.InstanceOf<AnotherService>());
        }

        [Test]
        public void Aware_of_newly_registered_service()
        {
            var container = new Container();

            var items = container.ResolveMany<IService>().ToArray();
            Assert.That(items.Count(), Is.EqualTo(0));

            container.Register<IService, Service>();
            items = container.ResolveMany<IService>().ToArray();

            Assert.That(items.Length, Is.EqualTo(1));
            Assert.That(items[0], Is.InstanceOf<Service>());
        }

        [Test]
        public void Newly_registered_services_could_be_turned_off_for_speed()
        {
            var container = new Container();

            var items = container.ResolveMany<IService>(result: Resolver.ManyResult.FixedIsBitFaster).ToArray();
            Assert.That(items.Count(), Is.EqualTo(0));

            container.Register<IService, Service>();
            items = container.ResolveMany<IService>(result: Resolver.ManyResult.FixedIsBitFaster).ToArray();
            Assert.That(items.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Newly_registered_services_still_could_be_returned_by_resolving_as_dynamic_after_fixed()
        {
            var container = new Container();

            var items = container.ResolveMany<IService>(result: Resolver.ManyResult.FixedIsBitFaster).ToArray();
            Assert.That(items.Count(), Is.EqualTo(0));

            container.Register<IService, Service>();
            items = container.ResolveMany<IService>().ToArray();
            Assert.That(items.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Return_enumerable_of_object_is_possible_with_required_service_type()
        {
            var container = new Container();
            container.Register<IService, Service>(named: 1);
            container.Register<IService, AnotherService>(named: '1');

            IEnumerable<object> result = container.ResolveMany<object>(typeof(IService));
            var items = result.ToArray();

            Assert.That(items.Length, Is.EqualTo(2));
            Assert.That(items[0], Is.InstanceOf<Service>());
            Assert.That(items[1], Is.InstanceOf<AnotherService>());
        }

        [Test]
        public void Return_enumerable_of_object_is_possible_with_required_service_type_is_fine_fixed_view_too()
        {
            var container = new Container();
            container.Register<IService, Service>(named: 1);
            container.Register<IService, AnotherService>(named: '1');

            IEnumerable<object> result = container.ResolveMany<object>(typeof(IService), Resolver.ManyResult.FixedIsBitFaster);
            var items = result.ToArray();

            Assert.That(items.Length, Is.EqualTo(2));
            Assert.That(items[0], Is.InstanceOf<Service>());
            Assert.That(items[1], Is.InstanceOf<AnotherService>());
        }

        [Test]
        public void Should_throw_if_required_service_is_not_assignable_collection_item_type()
        {
            var container = new Container();
            container.Register<Service>();

            var ex = Assert.Throws<ContainerException>(() =>
                container.ResolveMany<AnotherService>(typeof(Service)));

            Assert.That(ex.Message, Is.StringContaining("Required service type DryIoc.UnitTests.CUT.Service is not assignable"));
        }

        [Test]
        public void Could_resolve_collection_of_generic_wrappers_if_needed()
        {
            var container = new Container();
            container.Register<Service>(named: "first");

            var items = container.ResolveMany<Func<KeyValuePair<string, object>>>(typeof(Service)).ToArray();

            Assert.That(items.Length, Is.EqualTo(1));
            Assert.That(items[0]().Key, Is.EqualTo("first"));
            Assert.That(items[0]().Value, Is.InstanceOf<Service>());
        }
    }
}