using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using HttpUtils;

using Scribe.Core.ConnectorApi;
using Scribe.Core.ConnectorApi.Actions;
using Scribe.Core.ConnectorApi.Metadata;
using Scribe.Core.ConnectorApi.Query;
using Scribe.Core.ConnectorApi.Exceptions;
using Scribe.Core.ConnectorApi.Logger;
using Scribe.Connector.Common.Reflection.Data;

using CDK.Objects;
using CDK.Common;
using System.Linq;
using System.Net.Http;
using CDK.Models;
using static CDK.ConnectionHelper;
using Scribe.Core.ConnectorApi.Cryptography;
using System.Web;

namespace CDK
{

    class ConnectorService
    {
        #region Instaniation
        public RestClient client = new RestClient();
        private Reflector reflector;
        public bool IsConnected { get; set; }
        public Guid ConnectorTypeId { get; }

        private DateTime? LastConnected { get; set; }
        private ConnectionProperties properties { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public enum SupportedActions
        {
            Query,
            Update
        }
        #endregion

        #region Connection
        public void Connection(IDictionary<string, string> propDictionary)
        {
            //capture props from connection form
            if (propDictionary == null)
                throw new InvalidConnectionException("Connection Properties are NULL");
            var connectorProps = new ConnectionProperties();
            connectorProps.BaseUrl = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.BaseUrl, ConnectionPropertyLabels.BaseUrl);
            connectorProps.AuthUrl = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.AuthUrl, ConnectionPropertyLabels.AuthUrl);
            connectorProps.Username = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.Username, ConnectionPropertyLabels.Username);
            connectorProps.Password = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.Password, ConnectionPropertyLabels.Password);
            connectorProps.client_id = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.ClientId, ConnectionPropertyLabels.ClientId);
            connectorProps.client_secret = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.ClientSecret, ConnectionPropertyLabels.ClientSecret);
            //decrypt passwords coming in
            connectorProps.Password = Decryptor.Decrypt_AesManaged(connectorProps.Password, Connector.CryptoKey);
            if (string.IsNullOrEmpty(connectorProps.Password))
                throw new InvalidConnectionException(string.Format("A value is required for '{0}'", ConnectionPropertyLabels.Password));
            connectorProps.client_secret = Decryptor.Decrypt_AesManaged(connectorProps.client_secret, Connector.CryptoKey);
            if (string.IsNullOrEmpty(connectorProps.client_secret))
                throw new InvalidConnectionException(string.Format("A value is required for '{0}'", ConnectionPropertyLabels.ClientSecret));
            //remove slash for future concate calls on URLs
            if (connectorProps.BaseUrl.ToString().EndsWith("/"))
            { connectorProps.BaseUrl = connectorProps.BaseUrl.Remove(connectorProps.BaseUrl.Length - 1); }
            if (connectorProps.AuthUrl.ToString().EndsWith("/"))
            { connectorProps.AuthUrl = connectorProps.AuthUrl.Remove(connectorProps.AuthUrl.Length - 1); }
            // now make an API call for the token
            var client = new RestClient(connectorProps.AuthUrl);
            client.Method = HttpVerb.POST;
            client.ContentType = "application/x-www-form-urlencoded";
            client.Accept = "application/json";
            //Http Auth input
            var form = new ConnectInput();
            form.client_id = connectorProps.client_id;
            form.client_secret = connectorProps.client_secret;
            form.Username = connectorProps.Username;
            form.Password = connectorProps.Password;
            //Convert to Form Data
            var formData = new FormUrlEncodedContent(form.ToKeyValue());
            client.PostData = formData.ReadAsStringAsync().Result;
            //wait for above async method to make Http call
            if (!string.IsNullOrEmpty(client.PostData))
            {
                try
                {
                    var result = client.MakeRequest("");
                    //if no exception, connect was successful
                    var output = JsonConvert.DeserializeObject<ConnectOutput>(result);
                    connectorProps.access_token = output.access_token;
                    connectorProps.expires_in = output.expires_in;
                    properties = connectorProps;
                    LastConnected = DateTime.UtcNow; //use this later for ensuring connection
                    //for returning void on Connect
                    reflector = new Reflector(Assembly.GetExecutingAssembly());
                    IsConnected = true;
                }
                catch (RESTRequestException ex)
                {
                    IsConnected = false;
                    throw new InvalidConnectionException(ex.Message);
                }
            }
            IsConnected = true;
        }

        private static string getRequiredPropertyValue(IDictionary<string, string> properties, string key, string label)
        {
            var value = getPropertyValue(properties, key);
            if (string.IsNullOrEmpty(value))
                throw new InvalidConnectionException(string.Format("A value is required for '{0}'", label));
            return value;
        }

        private static string getPropertyValue(IDictionary<string, string> properties, string key)
        {
            var value = "";
            properties.TryGetValue(key, out value);
            return value;
        }

        public void Disconnect()
        {
            IsConnected = false;
        }

        #endregion

        #region Operations
        public OperationResult Update(DataEntity dataEntity, Dictionary<string, object> matchCriteria)
        {
            //EnsureConnected();
            var entityName = dataEntity.ObjectDefinitionFullName;
            var operationResult = new OperationResult();

            switch (entityName)
            {
                //Sample code
                case EntityNames.RealTimeSearch:
                    var quote = ToScribeModel<Models.RealTimeSearch.Rootobject>(dataEntity);
                    var quoteRequest = HttpCall(entityName, quote, matchCriteria, "update");
                    operationResult.Success = new[] { true };
                    operationResult.ObjectsAffected = new[] { 1 };
                    break;
                default:
                    throw new ArgumentException($"{entityName} is not supported for Create.");
            }
            return operationResult;
        }

        private T ToScribeModel<T>(DataEntity input) where T : new()
        {
            T scribeModel;
            try
            {
                scribeModel = reflector.To<T>(input);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error translating from DataEntity to ScribeModel: " + e.Message, e);
            }
            return scribeModel;
        }

        private string HttpCall<T>(string entityName, T data, Dictionary<string, object> matchCriteria, string action)
        {
            //Sample code
            client.Accept = "application/json";
            client.ContentType = "application/json";
            client.Method = HttpVerb.PUT;            //check action to set verb here instead
            var dtSettings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTH:mm:ss.fffZ" };

            switch (entityName)
            {
                case EntityNames.RealTimeSearch:
                    try
                    {
                        matchCriteria.TryGetValue("Id", out var quoteId);
                        client.EndPoint = properties.BaseUrl + "/" + quoteId.ToString() + "?auto-product-resave=true&changes=true";
                        client.PostData = JsonConvert.SerializeObject(data, Formatting.Indented, dtSettings);
                        var response = client.MakeRequest("");
                    }
                    catch (RESTRequestException ex)
                    {
                        Logger.Write(Logger.Severity.Error,
                            $"Error on update for entity: {entityName}.", ex.InnerException.Message);
                        throw new InvalidExecuteOperationException($"Error on update for {entityName}: " + ex.Message);
                    }
                    break;
            }

            return null;
        }

        #endregion

        #region Query
        public IEnumerable<DataEntity> ExecuteQuery(Query query)
        {
            var entityName = query.RootEntity.ObjectDefinitionFullName;
            var constraints = BuildConstraintDictionary(query.Constraints);

            switch (entityName)
            {
                case EntityNames.Items:
                    return QueryApi<Models.Items.Rootobject>
                        (query, reflector, constraints, entityName, client, properties);
                case EntityNames.RealTimeSearch:
                    return QueryApi<Models.RealTimeSearch.Rootobject>
                        (query, reflector, constraints, entityName, client, properties);
                default:
                    throw new InvalidExecuteQueryException(
                        $"The {entityName} entity is not supported for query.");
            }
        }

        public static IEnumerable<DataEntity> QueryApi<T>(Query query, Reflector r, Dictionary<string, object> filters, string entityName, RestClient client, ConnectionProperties props)
        {
            client.Method = HttpVerb.GET;
            client.Accept = "application/json";
            client.PostData = "";
            client.AuthToken = props.access_token;
            client.EndPoint = buildRequestUrl(filters, entityName, props);

            switch (entityName)
            {
                case EntityNames.RealTimeSearch:
                    try
                    {
                        var response = client.MakeRequest("");
                        var data = JsonConvert.DeserializeObject<T>(response);
                        return r.ToDataEntities(new[] { data }, query.RootEntity);
                    }
                    catch (RESTRequestException ex)
                    {
                        Logger.Write(Logger.Severity.Error,
                            $"Error on query for entity: {entityName}.", ex.InnerException.Message);
                        throw new InvalidExecuteQueryException($"Error on query for {entityName}: " + ex.Message);
                    }
                case EntityNames.Items:
                    try
                    {
                        var response = client.MakeRequest("");
                        var data = JsonConvert.DeserializeObject<Models.Items.Rootobject>(response);
                        var results = data.Items;
                        return r.ToDataEntities(results, query.RootEntity);
                    }
                    catch (RESTRequestException ex)
                    {
                        Logger.Write(Logger.Severity.Error,
                            $"Error on query for entity: {entityName}.", ex.InnerException.Message);
                        throw new InvalidExecuteQueryException($"Error on query for {entityName}: " + ex.Message);
                    }
                default:
                    throw new InvalidExecuteQueryException($"The {entityName} entity is not supported for query.");
            }
        }

        private static Dictionary<string, object> BuildConstraintDictionary(Expression queryExpression)
        {
            var constraints = new Dictionary<string, object>();

            if (queryExpression == null)
                return constraints;

            if (queryExpression.ExpressionType == ExpressionType.Comparison)
            {
                // only 1 filter
                addCompEprToConstraints(queryExpression as ComparisonExpression, ref constraints);
            }
            else if (queryExpression.ExpressionType == ExpressionType.Logical)
            {
                // Multiple filters
                addLogicalEprToConstraints(queryExpression as LogicalExpression, ref constraints);
            }
            else
                throw new InvalidExecuteQueryException("Unsupported filter type: " + queryExpression.ExpressionType.ToString());

            return constraints;
        }

        private static void addLogicalEprToConstraints(LogicalExpression exp, ref Dictionary<string, object> constraints)
        {
            if (exp.Operator != LogicalOperator.And)
                throw new InvalidExecuteQueryException("Unsupported operator in filter: " + exp.Operator.ToString());

            if (exp.LeftExpression.ExpressionType == ExpressionType.Comparison)
                addCompEprToConstraints(exp.LeftExpression as ComparisonExpression, ref constraints);
            else if (exp.LeftExpression.ExpressionType == ExpressionType.Logical)
                addLogicalEprToConstraints(exp.LeftExpression as LogicalExpression, ref constraints);
            else
                throw new InvalidExecuteQueryException("Unsupported filter type: " + exp.LeftExpression.ExpressionType.ToString());

            if (exp.RightExpression.ExpressionType == ExpressionType.Comparison)
                addCompEprToConstraints(exp.RightExpression as ComparisonExpression, ref constraints);
            else if (exp.RightExpression.ExpressionType == ExpressionType.Logical)
                addLogicalEprToConstraints(exp.RightExpression as LogicalExpression, ref constraints);
            else
                throw new InvalidExecuteQueryException("Unsupported filter type: " + exp.RightExpression.ExpressionType.ToString());
        }

        private static void addCompEprToConstraints(ComparisonExpression exp, ref Dictionary<string, object> constraints)
        {
            if (exp.Operator != ComparisonOperator.Equal)
                throw new InvalidExecuteQueryException(string.Format(StringMessages.OnlyEqualsOperatorAllowed, exp.Operator.ToString(), exp.LeftValue.Value));

            var constraintKey = exp.LeftValue.Value.ToString();
            if (constraintKey.LastIndexOf(".") > -1)
            {
                // need to remove "objectname." if present
                constraintKey = constraintKey.Substring(constraintKey.LastIndexOf(".") + 1);
            }
            constraints.Add(constraintKey, exp.RightValue.Value.ToString());
        }

        private static string buildRequestUrl(Dictionary<string, object> filters, string entityName, ConnectionProperties props)
        {
            var uri = new UriBuilder(props.BaseUrl);
            var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
            foreach (var kvp in filters.ToDictionary(k => k.Key, k => k.Value.ToString()).ToArray())
                queryBuilder.Add(kvp.Key, kvp.Value);

            switch (entityName)
            {
                case EntityNames.RealTimeSearch:
                case EntityNames.Items:
                    uri.Path = "/api/v1/search/RealTimeSearch";
                    uri.Query = queryBuilder.ToString();
                    return uri.ToString();
                case "something else":
                    uri.Path = props.BaseUrl + "else";
                    uri.Query = queryBuilder.ToString();
                    return uri.ToString();
                default:
                    throw new InvalidExecuteQueryException($"The {entityName} entity is not supported for query.");
            }
        }
        #endregion

        #region Metadata
        public IMetadataProvider GetMetadataProvider()
        {
            return reflector.GetMetadataProvider();
        }

        public IEnumerable<IActionDefinition> RetrieveActionDefinitions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IObjectDefinition> RetrieveObjectDefinitions(bool shouldGetProperties = false, bool shouldGetRelations = false)
        {
            throw new NotImplementedException();
        }

        public IObjectDefinition RetrieveObjectDefinition(string objectName, bool shouldGetProperties = false,
            bool shouldGetRelations = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMethodDefinition> RetrieveMethodDefinitions(bool shouldGetParameters = false)
        {
            throw new NotImplementedException();
        }

        public IMethodDefinition RetrieveMethodDefinition(string objectName, bool shouldGetParameters = false)
        {
            throw new NotImplementedException();
        }

        public void ResetMetadata()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}