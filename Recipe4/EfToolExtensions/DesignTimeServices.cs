// Code example lifted from Rowan Miller's blog post at https://romiller.com/2017/02/10/ef-core-1-1-pluralization-in-reverse-engineer/

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Configuration.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication
{
    /// <summary>
    /// If there is a class that implements IDesignTimeServices, 
    /// then the EF Tools will call it to allow custom services 
    /// to be registered.
    ///
    /// We implement this method so that we can replace some of 
    /// the services used during reverse engineer.
    /// </summary>
    public class MyDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            services.AddSingleton<IScaffoldingModelFactory, CustomRelationalScaffoldingModelFactory>();
            services.AddSingleton<DbContextWriter, CustomDbContextWriter>();
            services.AddSingleton<ConfigurationFactory, CustomConfigurationFactory>();
        }
    }

    /// <summary>
    /// ConfigurationFactory creates instances of configuration objects, 
    /// which hold any configuration needs to be performed using 
    /// Fluent API and/or data annotations.
    ///
    /// We override this so that we can always configure the table 
    /// name for each entity.
    /// </summary>
    public class CustomConfigurationFactory : ConfigurationFactory
    {
        public CustomConfigurationFactory(
            IRelationalAnnotationProvider extensionsProvider,
            CSharpUtilities cSharpUtilities,
            ScaffoldingUtilities scaffoldingUtilities)
            : base(
                  extensionsProvider, 
                  cSharpUtilities, 
                  scaffoldingUtilities)
        { }

        public override ModelConfiguration CreateModelConfiguration(
            IModel model,
            CustomConfiguration customConfiguration)
        {
            return new CustomModelConfiguration(
                this,
                model,
                customConfiguration,
                ExtensionsProvider,
                CSharpUtilities,
                ScaffoldingUtilities);
        }
    }

    /// <summary>
    /// ModelConfiguration reads the model and works out what 
    /// configuration needs to be performed using Fluent API 
    /// and/or data annotations.
    ///
    /// We override this because the base implementation will only 
    /// configure the table name if it is different than the entity 
    /// name. But, by default, EF will use the DbSet property name 
    /// as the table name. This works by default because the entity 
    /// and DbSet property have the same name, but we are changing that.
    /// </summary>
    public class CustomModelConfiguration : ModelConfiguration
    {
        private ConfigurationFactory _configurationFactory;

        public CustomModelConfiguration(
            ConfigurationFactory configurationFactory,
            IModel model,
            CustomConfiguration customConfiguration,
            IRelationalAnnotationProvider annotationProvider,
            CSharpUtilities cSharpUtilities,
            ScaffoldingUtilities scaffoldingUtilities)
            : base(
                  configurationFactory, 
                  model, 
                  customConfiguration, 
                  annotationProvider, 
                  cSharpUtilities, 
                  scaffoldingUtilities)
        {
            _configurationFactory = configurationFactory;
        }

        public override void AddTableNameConfiguration(EntityConfiguration entityConfiguration)
        {
            // Rather than being smart, we're just always configuring the 
            // table name for every entity.

            var entityType = entityConfiguration.EntityType;
            var delimitedTableName = CSharpUtilities.DelimitString(AnnotationProvider.For(entityType).TableName);
            var delimitedSchemaName = CSharpUtilities.DelimitString(AnnotationProvider.For(entityType).Schema);

            entityConfiguration.FluentApiConfigurations.Add(
                _configurationFactory.CreateFluentApiConfiguration(
                    true, /* <= hasAttributeEquivalent */
                    nameof(RelationalEntityTypeBuilderExtensions.ToTable),
                    delimitedTableName,
                    delimitedSchemaName));

            entityConfiguration.AttributeConfigurations.Add(
                _configurationFactory.CreateAttributeConfiguration(
                    nameof(TableAttribute),
                    delimitedTableName,
                    $"{nameof(TableAttribute.Schema)} = {delimitedSchemaName}"));
        }
    }

    /// <summary>
    /// SqlServerScaffoldingModelFactory reads the database schema 
    /// and turns it into an EF model.
    /// 
    /// We override this so that we can singularize entity type 
    /// names in the model.
    /// </summary>
    public class CustomRelationalScaffoldingModelFactory : SqlServerScaffoldingModelFactory
    {
        public CustomRelationalScaffoldingModelFactory(
            ILoggerFactory loggerFactory,
            IRelationalTypeMapper typeMapper,
            IDatabaseModelFactory databaseModelFactory,
            CandidateNamingService candidateNamingService)
            : base(
                  loggerFactory, 
                  typeMapper, 
                  databaseModelFactory, 
                  candidateNamingService)
        { }

        protected override string GetEntityTypeName(TableModel table)
        {
            // Use the base implementation to get a C# friendly name
            var name = base.GetEntityTypeName(table);

            // Singularize the name
            return Inflector.Inflector.Singularize(name) ?? name;
        }
    }

    /// <summary>
    /// DbContextWriter writes out the C# code for the context.
    /// 
    /// We override this so that we can pluralize the DbSet names.
    /// </summary>
    public class CustomDbContextWriter : DbContextWriter
    {
        public CustomDbContextWriter(
            ScaffoldingUtilities scaffoldingUtilities,
            CSharpUtilities cSharpUtilities)
            : base(scaffoldingUtilities, cSharpUtilities)
        { }

        public override string WriteCode(ModelConfiguration modelConfiguration)
        {
            // There is no good way to override the DbSet naming, as it uses 
            // an internal StringBuilder. This means we can't override 
            // AddDbSetProperties without re-implementing the entire class.
            // Therefore, we have to get the code and then do string manipulation 
            // to replace the DbSet property code

            var code = base.WriteCode(modelConfiguration);

            foreach (var entityConfig in modelConfiguration.EntityConfigurations)
            {
                var entityName = entityConfig.EntityType.Name;
                var setName = Inflector.Inflector.Pluralize(entityName) ?? entityName;

                code = code.Replace(
                    $"DbSet<{entityName}> {entityName}",
                    $"DbSet<{entityName}> {setName}");
            }

            return code;
        }
    }
}