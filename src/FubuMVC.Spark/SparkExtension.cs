﻿using System;
using FubuMVC.Core;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    // This approach uses default conventions.
    public class SparkExtension : IFubuRegistryExtension, ISparkExtension
    {
        private readonly SparkViewFacility _facility;

        public SparkExtension()
        {
            // TODO: move onto conventions
            var tokenizer = new SparkItemBuilder()
                .Apply<MasterPageBinder>()
                .Apply<ViewModelBinder>()
                .Apply<NamespaceBinder>();
            
            _facility = new SparkViewFacility(tokenizer);
        }

        public void Configure(FubuRegistry registry)
        {
            registry.Views.Facility(_facility);            
        }

        // DSL 
        // ConfigureSparkExpression...
    }

    public interface ISparkExtension
    {
    }

    // TODO: Ask JDM & JA about this.
    // This approach allows for configuration.
    public static class FubuRegistryExtensions
    {
        public static void UseSpark(this FubuRegistry fubuRegistry)
        {
            fubuRegistry.UseSpark(s => {});    
        }

        public static void UseSpark(this FubuRegistry fubuRegistry, Action<ISparkExtension> configure)
        {
            var spark = new SparkExtension();
            configure(spark);
            spark.Configure(fubuRegistry);
        }
    }
}