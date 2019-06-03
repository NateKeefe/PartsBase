using System;
using Scribe.Core.ConnectorApi.ConnectionUI;

namespace CDK
{
    public static class ConnectionHelper
    {
        #region Constants
        public class ConnectionProperties
        {
            public string BaseUrl { get; set; }
            public string AuthUrl { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        internal static class ConnectionPropertyKeys
        {
            public const string BaseUrl = "BaseUrl";
            public const string AuthUrl = "AuthUrl";
            public const string Username = "Username";
            public const string Password = "Password";
            public const string ClientId = "ClientId";
            public const string ClientSecret = "ClientSecret";

        }

        internal static class ConnectionPropertyLabels
        {
            public const string BaseUrl = "BaseUrl";
            public const string AuthUrl = "AuthUrl";
            public const string Username = "Username";
            public const string Password = "Password";
            public const string ClientId = "ClientId";
            public const string ClientSecret = "ClientSecret";
        }

        private const string HelpLink = "https://apiservices.partsbase.com/docs/index/";
        #endregion

        #region Form Definition Builders
        public static FormDefinition GetConnectionFormDefintion()
        {

            var formDefinition = new FormDefinition
            {
                CompanyName = Connector.CompanyName,
                CryptoKey = Connector.CryptoKey,
                HelpUri = new Uri(HelpLink)
            };

            formDefinition.Add(BuildBaseUrlDefinition(0));
            formDefinition.Add(BuildAutheUrlDefinition(1));
            formDefinition.Add(BuildUsernameDefinition(2));
            formDefinition.Add(BuildPasswordDefinition(3));
            formDefinition.Add(BuildClientIdDefinition(4));
            formDefinition.Add(BuildClientSecretDefinition(5));

            return formDefinition;
        }

        private static EntryDefinition BuildBaseUrlDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Text,
                IsRequired = false,
                Label = ConnectionPropertyLabels.BaseUrl,
                PropertyName = ConnectionPropertyKeys.BaseUrl,
                Order = order,
            };

            return entryDefinition;
        }

        private static EntryDefinition BuildAutheUrlDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Text,
                IsRequired = false,
                Label = ConnectionPropertyLabels.AuthUrl,
                PropertyName = ConnectionPropertyKeys.AuthUrl,
                Order = order,
            };

            return entryDefinition;
        }

        private static EntryDefinition BuildUsernameDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Text,
                IsRequired = false,
                Label = ConnectionPropertyLabels.Username,
                PropertyName = ConnectionPropertyKeys.Username,
                Order = order,
            };

            return entryDefinition;
        }

        private static EntryDefinition BuildPasswordDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Password,
                IsRequired = false,
                Label = ConnectionPropertyLabels.Password,
                PropertyName = ConnectionPropertyKeys.Password,
                Order = order,
            };

            return entryDefinition;
        }

        private static EntryDefinition BuildClientIdDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Text,
                IsRequired = false,
                Label = ConnectionPropertyLabels.ClientId,
                PropertyName = ConnectionPropertyKeys.ClientId,
                Order = order,
            };

            return entryDefinition;
        }

        private static EntryDefinition BuildClientSecretDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Password,
                IsRequired = false,
                Label = ConnectionPropertyLabels.ClientSecret,
                PropertyName = ConnectionPropertyKeys.ClientSecret,
                Order = order,
            };

            return entryDefinition;
        }
        #endregion
    }
}