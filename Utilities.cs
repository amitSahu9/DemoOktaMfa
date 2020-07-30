namespace OktaDemo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using FlexibleConfiguration;
    using Microsoft.Win32;
    using Okta.Auth.Sdk;
    using Okta.Sdk.Abstractions;
    using Okta.Sdk.Abstractions.Configuration;

    // class ExceptionUtils
    // {
    //    public static void ThrowExceptionBasedOnErrorCode(ErrorClause errorClause, Boolean isRecoverable)
    //    {
    //        Logger logger = LogManager.GetCurrentClassLogger();
    //        int httpResponseCode = errorClause.httpResponseCode;
    //        switch (httpResponseCode)
    //        {
    //            case 401:
    //                logger.Debug("401 thrown");
    //                logger.Debug("message" + errorClause.errorSummary);
    //                throw new UnAuthorisedException(errorClause.errorSummary, isRecoverable);
    //            default:
    //                logger.Debug("Custom exception thrown");
    //                logger.Debug("message" + errorClause.errorSummary);
    //                throw new TMFAException(errorClause.errorSummary, isRecoverable);
    //        };
    //    }

    // public static void ThrowExceptionForPrimaryauthBasedOnErrorCode(ErrorClause errorClause, Boolean isRecoverable)
    //    {
    //        Logger logger = LogManager.GetCurrentClassLogger();
    //        int httpResponseCode = errorClause.httpResponseCode;
    //        switch (httpResponseCode)
    //        {
    //            case 401:
    //                logger.Debug("401 is thrown for primary authentication");
    //                errorClause.errorSummary = "We could not identify user in OKTA with this username or password. Please, check your credentials.";
    //                throw new UnAuthorisedException(errorClause.errorSummary, isRecoverable);
    //            default:
    //                logger.Debug("Custom exception thrown");
    //                logger.Debug("message" + errorClause.errorSummary);
    //                throw new TMFAException(errorClause.errorSummary, isRecoverable);
    //        };
    //    }
    // }

    public static class RegistryUtils
    {
        public static Tuple<bool, object> FindValueAtPath(string valueName, string registryPath)
        {
            var tuple = new Tuple<bool, object>(false, string.Empty);
            RegistryKey pathReference = Registry.LocalMachine.OpenSubKey(registryPath);
            if (pathReference != null)
            {
                string[] keys = pathReference.GetValueNames();
                if (keys.Contains(valueName))
                {
                    object value = pathReference.GetValue(valueName);
                    tuple = new Tuple<bool, object>(true, value);
                }
            }

            return tuple;
        }

        public static void CreateRegistryKeyIfNotCreated(string keyName, string keyPath)
        {
            keyPath = string.Format(keyPath + "\\" + keyName);
            RegistryKey pathReference = Registry.LocalMachine.OpenSubKey(keyPath);
            if (pathReference == null)
            {
                pathReference = Registry.LocalMachine.CreateSubKey(keyPath);
            }
        }

        public static List<string> GetAllSubkeys(string registryPathOfkey)
        {
            List<string> listOfSubkeys = new List<string>();
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryPathOfkey);
            if (registryKey != null)
            {
                string[] subkeysArray = registryKey.GetSubKeyNames();
                listOfSubkeys = new List<string>(subkeysArray);
            }

            return listOfSubkeys;
        }

        public static bool SearchForKeyInSuperKey(List<string> listOfKeys, string keyToSearch)
        {
            return listOfKeys.Contains(keyToSearch);
        }

        public static void WriteCredToRegistry(byte[] dataToWrite, string registryPath, string valueName)
        {
            RegistryKey key = Registry.LocalMachine.CreateSubKey(registryPath, true);
            key.SetValue(valueName, dataToWrite, RegistryValueKind.Binary);
            Console.WriteLine("written");
            key.Close();
            Console.WriteLine("closed");
        }

        public static void WriteCredToRegistry(string dataToWrite, string registryPath, string valueName)
        {
            Console.WriteLine("WriteCredToRegistry() begin");
            RegistryKey key = Registry.LocalMachine.CreateSubKey(registryPath);
            key.SetValue(valueName, dataToWrite, RegistryValueKind.String);
            Console.WriteLine("written");
            key.Close();
            Console.WriteLine("closed");
        }
    }

    public static class AuthenticationClientUtils
    {

        public static CustomAuthnClient CreateAuthenticationClient(IAppSettingsHelper appSettingsHelper)
        {
            string urlDetails = appSettingsHelper.GetBaseUrl();
            if (urlDetails != null)
            {
                OktaClientConfiguration oktaClientConfiguration = new OktaClientConfiguration
                {
                    OktaDomain = urlDetails,
                };
                var customClientParams = GetDefaultCustomClientParams(oktaClientConfiguration);
                CustomAuthnClient customAuthnClient = new CustomAuthnClient(customClientParams.Item1, customClientParams.Item2, customClientParams.Item3);
                return customAuthnClient;
            }
            else
            {
                throw new Exception("Could not reach out registry to get okta base URL");
            }
        }

        public static (IDataStore, OktaClientConfiguration, RequestContext) GetDefaultCustomClientParams(OktaClientConfiguration oktaClientConfiguration)
        {
            OktaClientConfiguration clientConfiguration = PrepareOktaClientConfiguration(oktaClientConfiguration);
            IDataStore dataStore = PrepareIDataStore(clientConfiguration);
            RequestContext requestContext = null;
            return (dataStore, clientConfiguration, requestContext);
        }

        private static IDataStore PrepareIDataStore(OktaClientConfiguration apiClientConfiguration)
        {
            OktaClientConfiguration oktaClientConfiguration = PrepareOktaClientConfiguration(apiClientConfiguration);
            Microsoft.Extensions.Logging.ILogger logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

            HttpClient httpClient = DefaultHttpClient.Create(
                oktaClientConfiguration.ConnectionTimeout,
                oktaClientConfiguration.Proxy,
                logger);

            AbstractResourceTypeResolverFactory resourceTypeResolverFactory = null;
            BaseOktaClient baseOktaClient = new BaseOktaClient();
            dynamic resourceFactory = new ResourceFactory(baseOktaClient, logger, resourceTypeResolverFactory);
            var requestExecutor = new DefaultRequestExecutor(oktaClientConfiguration, httpClient, logger);

            UserAgentBuilder userAgentBuilder = new UserAgentBuilder(
                "custom", typeof(CustomAuthnClient).GetTypeInfo().Assembly.GetName().Version);

            IDataStore dataStore = new DefaultDataStore(
                requestExecutor,
                new DefaultSerializer(),
                resourceFactory,
                logger,
                userAgentBuilder);
            return dataStore;
        }

        private static OktaClientConfiguration PrepareOktaClientConfiguration(OktaClientConfiguration apiClientConfiguration)
        {
            OktaClientConfiguration oktaClientConfiguration = GetConfigurationOrDefault(apiClientConfiguration);
            return oktaClientConfiguration;
        }

        private static OktaClientConfiguration GetConfigurationOrDefault(OktaClientConfiguration apiClientConfiguration = null)
        {
            string configurationFileRoot = Directory.GetCurrentDirectory();

            var homeOktaYamlLocation = HomePath.Resolve("~", ".okta", "okta.yaml");

            var applicationAppSettingsLocation = Path.Combine(configurationFileRoot ?? string.Empty, "appsettings.json");
            var applicationOktaYamlLocation = Path.Combine(configurationFileRoot ?? string.Empty, "okta.yaml");

            var configBuilder = new ConfigurationBuilder()
                .AddYamlFile(homeOktaYamlLocation, optional: true)
                .AddJsonFile(applicationAppSettingsLocation, optional: true)
                .AddYamlFile(applicationOktaYamlLocation, optional: true)
                .AddEnvironmentVariables("okta", "_", root: "okta")
                .AddEnvironmentVariables("okta_testing", "_", root: "okta")
                .AddObject(apiClientConfiguration, root: "okta:client")
                .AddObject(apiClientConfiguration, root: "okta:testing")
                .AddObject(apiClientConfiguration);

            var compiledConfig = new OktaClientConfiguration();
            configBuilder.Build().GetSection("okta").GetSection("client").Bind(compiledConfig);
            configBuilder.Build().GetSection("okta").GetSection("testing").Bind(compiledConfig);

            return compiledConfig;
        }
    }

}
