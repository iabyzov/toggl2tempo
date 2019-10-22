using System;
using Dapplo.HttpExtensions;
using Dapplo.HttpExtensions.JsonNet;
using Dapplo.Jira.Domains;

namespace Tempo.Services
{
    public class WorklogService : IProjectDomain, IWorkDomain, IUserDomain, ISessionDomain, IIssueDomain, IFilterDomain, IAttachmentDomain, IServerDomain, IAgileDomain, ITempoDomain, IGreenhopperDomain
    {
        private string _password;
        private string _user;
        private string _tempoToken;

        /// <inheritdoc />
        /// <summary>
        ///     Store the specific HttpBehaviour, which contains a IHttpSettings and also some additional logic for making a
        ///     HttpClient which works with Jira
        /// </summary>
        public IHttpBehaviour Behaviour { get; }

        public IHttpBehaviour TempoBehaviour { get; private set; }

        /// <inheritdoc />
        /// <summary>The base URI for your JIRA server</summary>
        public Uri JiraBaseUri { get; }

        /// <inheritdoc />
        /// <summary>The rest URI for your JIRA server</summary>
        public Uri JiraRestUri { get; }

        /// <inheritdoc />
        /// <summary>The agile rest URI for your JIRA server</summary>
        public Uri JiraAgileRestUri { get; }

        /// <inheritdoc />
        /// <summary>The base URI for JIRA auth api</summary>
        public Uri JiraAuthUri { get; }

        public Uri JiraTempoUri { get; }

        /// <inheritdoc />
        /// <summary>Issue domain</summary>
        public IIssueDomain Issue => this;

        /// <inheritdoc />
        /// <summary>Attachment domain</summary>
        public IAttachmentDomain Attachment => this;

        /// <inheritdoc />
        /// <summary>Project domain</summary>
        public IProjectDomain Project => this;

        /// <inheritdoc />
        /// <summary>User domain</summary>
        public IUserDomain User => this;

        /// <inheritdoc />
        /// <summary>Session domain</summary>
        public ISessionDomain Session => this;

        /// <inheritdoc />
        /// <summary>Filter domain</summary>
        public IFilterDomain Filter => this;

        /// <inheritdoc />
        /// <summary>Work domain</summary>
        public IWorkDomain Work => this;

        /// <inheritdoc />
        /// <summary>Server domain</summary>
        public IServerDomain Server => this;

        /// <inheritdoc />
        /// <summary>Agile domain</summary>
        public IAgileDomain Agile => this;

        /// <inheritdoc />
        /// <summary>
        ///     Create the JiraApi object, here the HttpClient is configured
        /// </summary>
        /// <param name="baseUri">Base URL, e.g. https://yourjiraserver</param>
        /// <param name="httpSettings">IHttpSettings or null for default</param>
        public WorklogService(Uri baseUri, IHttpSettings httpSettings = null)
            : this(baseUri)
        {
            Behaviour = ConfigureBehaviour(new HttpBehaviour(), httpSettings);
        }

        /// <summary>Constructor for only the Uri, used internally</summary>
        /// <param name="baseUri"></param>
        private WorklogService(Uri baseUri)
        {
            JiraBaseUri = baseUri;
            JiraRestUri = baseUri.AppendSegments((object)"rest", (object)"api", (object)"2");
            JiraAuthUri = baseUri.AppendSegments((object)"rest", (object)"auth", (object)"1");
            var baseTempoUri = new Uri("https://api.tempo.io");
            JiraTempoUri = baseTempoUri.AppendSegments("core", "3");
            JiraAgileRestUri = baseUri.AppendSegments((object)"rest", (object)"agile", (object)"1.0");
            JiraGreenhopperRestUri = baseUri.AppendSegments((object)"rest", (object)"greenhopper", (object)"1.0");
        }

        /// <summary>
        ///     Helper method to configure the IChangeableHttpBehaviour
        /// </summary>
        /// <param name="behaviour">IChangeableHttpBehaviour</param>
        /// <param name="httpSettings">IHttpSettings</param>
        /// <returns>the behaviour, but configured as IHttpBehaviour </returns>
        private IHttpBehaviour ConfigureBehaviour(IChangeableHttpBehaviour behaviour, IHttpSettings httpSettings = null)
        {
            behaviour.HttpSettings = httpSettings ?? HttpExtensionsGlobals.HttpSettings;
            behaviour.JsonSerializer = new JsonNetJsonSerializer();
            behaviour.OnHttpRequestMessageCreated = httpMessage =>
            {
                httpMessage?.Headers.TryAddWithoutValidation("X-Atlassian-Token", "nocheck");
                if (!string.IsNullOrEmpty(_user) && _password != null)
                {
                    httpMessage?.SetBasicAuthorization(_user, _password);
                }

                return httpMessage;
            };
            return behaviour;
        }

        public void SetBearer(string token)
        {
            _tempoToken = token;
            TempoBehaviour = ConfigureTempoBehaviour(new HttpBehaviour());
        }

        private IHttpBehaviour ConfigureTempoBehaviour(IChangeableHttpBehaviour behaviour)
        {
            behaviour.JsonSerializer = new JsonNetJsonSerializer();
            behaviour.OnHttpRequestMessageCreated = httpMessage =>
            {
                if (!string.IsNullOrEmpty(_tempoToken))
                    httpMessage.SetBearer(_tempoToken);
                return httpMessage;
            };
            return behaviour;
        }

        /// <summary>Set Basic Authentication for the current client</summary>
        /// <param name="user">username</param>
        /// <param name="password">password</param>
        public void SetBasicAuthentication(string user, string password)
        {
            _user = user;
            _password = password;
        }

        public ITempoDomain Tempo => this;

        public Uri JiraGreenhopperRestUri { get; }

        public IGreenhopperDomain Greenhopper => this;
    }
}
