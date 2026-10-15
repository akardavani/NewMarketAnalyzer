using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MarketAnalyzer.Persistence.Extensions
{
    internal class SqlServerMethodTranslatorExtension : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;
        private IServiceProvider _serviceProvider;

        public DbContextOptionsExtensionInfo Info =>
            _info ??= new ExtensionInfo(this);

        public SqlServerMethodTranslatorExtension WithSqlServerMethodTranslator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddSingleton<IMethodCallTranslatorPlugin, SqlServerMethodCallTranslatorPlugin>();
        }

        public void Validate(IDbContextOptions options)
        {
            if (_serviceProvider is null)
            {
                throw new InvalidOperationException("The external IServiceProvider not configured.");
            }
        }

        private class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private string _logFragment;
            private int? _serviceProviderHash;

            public ExtensionInfo(SqlServerMethodTranslatorExtension extension) : base(extension)
            {

            }

            private new SqlServerMethodTranslatorExtension Extension
                => (SqlServerMethodTranslatorExtension)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override string LogFragment
            {
                get
                {
                    _logFragment ??= "Using SqlServerMethodTranslatorExtension";
                    return _logFragment;
                }
            }

            public override int GetServiceProviderHashCode()
            {
                _serviceProviderHash ??= Extension._serviceProvider.GetHashCode();
                return _serviceProviderHash.Value;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                if (debugInfo is null)
                {
                    throw new ArgumentNullException(nameof(debugInfo));
                }

                debugInfo["SqlServerMethodTranslatorExtension:" + nameof(SqlServerMethodCallTranslatorPlugin)] =
                   Extension._serviceProvider.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                return other is ExtensionInfo && other.GetServiceProviderHashCode() == GetServiceProviderHashCode();
            }
        }
    }
}
