using System.Configuration;

namespace BuildingBlocks.Configuration
{
    public abstract class ConfiguratorStarter 
    {
        private Configurator _configurator;

        public void EnsureConfiguratorStarted()
        {
            if (_configurator == null)
            {
                _configurator = CreateConfigurator();
                _configurator.Configure();
            }
        }

        protected abstract Configurator CreateConfigurator();
    }

    public abstract class ConfigurationBootstrapper : IBootstrapper
    {
        private Configurator _configurator;

        public Configurator Configurator 
        {
            get { return _configurator; }
        }

        public void InitializeConfiguration()
        {
            _configurator = InitConfigurator();
        }

        protected abstract Configurator InitConfigurator();

        protected string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        protected string GetConnectionString(string key)
        {
            var hospitalConnectionString = ConfigurationManager.ConnectionStrings[key];
            if (hospitalConnectionString == null || string.IsNullOrEmpty(hospitalConnectionString.ConnectionString))
            {
                throw new ConfigurationErrorsException("Connection string with name " + key + " not found");
            }
            return hospitalConnectionString.ConnectionString;
        }
    }
}