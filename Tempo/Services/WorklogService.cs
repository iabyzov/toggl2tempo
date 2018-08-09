using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Common;
using Dapplo.HttpExtensions;
using Dapplo.HttpExtensions.JsonNet;
using Dapplo.Jira;
using Dapplo.Jira.Domains;
using Tempo.DataObjects;

namespace Tempo.Services
{
    public class WorklogService : IProjectDomain, IJiraDomain, IWorklogService, IWorkDomain, IUserDomain, ISessionDomain, IIssueDomain, IFilterDomain, IAttachmentDomain, IServerDomain, IAgileDomain, ITempoDomain
    {
        private string _password;
        private string _user;
        private string _tempoToken;

        /// <summary>
        ///     Store the specific HttpBehaviour, which contains a IHttpSettings and also some additional logic for making a
        ///     HttpClient which works with Jira
        /// </summary>
        public IHttpBehaviour Behaviour { get; set; }

        public IHttpBehaviour TempoBehaviour { get; set; }

        /// <summary>The base URI for your JIRA server</summary>
        public Uri JiraBaseUri { get; }

        /// <summary>The rest URI for your JIRA server</summary>
        public Uri JiraRestUri { get; }

        /// <summary>The agile rest URI for your JIRA server</summary>
        public Uri JiraAgileRestUri { get; }

        /// <summary>The base URI for JIRA auth api</summary>
        public Uri JiraAuthUri { get; }

        public Uri JiraTempoUri { get; }

        /// <summary>Issue domain</summary>
        public IIssueDomain Issue
        {
            get
            {
                return (IIssueDomain)this;
            }
        }

        /// <summary>Attachment domain</summary>
        public IAttachmentDomain Attachment
        {
            get
            {
                return (IAttachmentDomain)this;
            }
        }

        /// <summary>Project domain</summary>
        public IProjectDomain Project
        {
            get
            {
                return (IProjectDomain)this;
            }
        }

        /// <summary>User domain</summary>
        public IUserDomain User
        {
            get
            {
                return (IUserDomain)this;
            }
        }

        /// <summary>Session domain</summary>
        public ISessionDomain Session
        {
            get
            {
                return (ISessionDomain)this;
            }
        }

        /// <summary>Filter domain</summary>
        public IFilterDomain Filter
        {
            get
            {
                return (IFilterDomain)this;
            }
        }

        /// <summary>Work domain</summary>
        public IWorkDomain Work
        {
            get
            {
                return (IWorkDomain)this;
            }
        }

        /// <summary>Server domain</summary>
        public IServerDomain Server => (IServerDomain)this;

        /// <summary>Agile domain</summary>
        public IAgileDomain Agile
        {
            get
            {
                return (IAgileDomain)this;
            }
        }

        /// <summary>
        ///     Create the JiraApi object, here the HttpClient is configured
        /// </summary>
        /// <param name="baseUri">Base URL, e.g. https://yourjiraserver</param>
        /// <param name="httpSettings">IHttpSettings or null for default</param>
        public WorklogService(Uri baseUri, IHttpSettings httpSettings = null)
            : this(baseUri)
        {
            this.Behaviour = this.ConfigureBehaviour((IChangeableHttpBehaviour)new HttpBehaviour(), httpSettings);
        }

        /// <summary>Constructor for only the Uri, used internally</summary>
        /// <param name="baseUri"></param>
        private WorklogService(Uri baseUri)
        {
            this.JiraBaseUri = baseUri;
            this.JiraRestUri = baseUri.AppendSegments((object)"rest", (object)"api", (object)"2");
            this.JiraAuthUri = baseUri.AppendSegments((object)"rest", (object)"auth", (object)"1");
            var baseTempoUri = new Uri("https://api.tempo.io");
            JiraTempoUri = baseTempoUri.AppendSegments("rest-legacy", "tempo-timesheets", "3");
            this.JiraAgileRestUri = baseUri.AppendSegments((object)"rest", (object)"agile", (object)"1.0");
            this.JiraGreenhopperRestUri = baseUri.AppendSegments((object)"rest", (object)"greenhopper", (object)"1.0");
        }

        public WorklogService(Uri baseUri, string cookieHeader) : this(baseUri, (IHttpSettings)null)
        {
            this.Behaviour.CookieContainer.SetCookies(baseUri, cookieHeader);
        }

        /// <summary>Factory method to create the jira client</summary>
        //public static IWorklogService Create(Uri baseUri, IHttpSettings httpSettings = null)
        //{
        //    return (IWorklogService)new WorklogService(baseUri, httpSettings);
        //}

        /// <summary>
        ///     Helper method to configure the IChangeableHttpBehaviour
        /// </summary>
        /// <param name="behaviour">IChangeableHttpBehaviour</param>
        /// <param name="httpSettings">IHttpSettings</param>
        /// <returns>the behaviour, but configured as IHttpBehaviour </returns>
        private IHttpBehaviour ConfigureBehaviour(IChangeableHttpBehaviour behaviour, IHttpSettings httpSettings = null)
        {
            behaviour.HttpSettings = httpSettings ?? HttpExtensionsGlobals.HttpSettings;
            behaviour.JsonSerializer = (IJsonSerializer)new JsonNetJsonSerializer();
            behaviour.OnHttpRequestMessageCreated = (Func<HttpRequestMessage, HttpRequestMessage>)(httpMessage =>
            {
                if (httpMessage != null)
                    httpMessage.Headers.TryAddWithoutValidation("X-Atlassian-Token", "nocheck");
                if (!string.IsNullOrEmpty(this._user) && this._password != null && httpMessage != null)
                    httpMessage.SetBasicAuthorization(this._user, this._password);
                return httpMessage;
            });
            return (IHttpBehaviour)behaviour;
        }

        public void SetBearer(string token)
        {
            _tempoToken = token;
            TempoBehaviour = ConfigureTempoBehaviour(new HttpBehaviour());
        }

        private IHttpBehaviour ConfigureTempoBehaviour(HttpBehaviour behaviour)
        {
            behaviour.JsonSerializer = (IJsonSerializer)new JsonNetJsonSerializer();
            behaviour.OnHttpRequestMessageCreated = (Func<HttpRequestMessage, HttpRequestMessage>)(httpMessage =>
            {
                if (!string.IsNullOrEmpty(_tempoToken))
                    httpMessage.SetBearer(_tempoToken);
                return httpMessage;
            });
            return (IHttpBehaviour)behaviour;
        }

        /// <summary>Set Basic Authentication for the current client</summary>
        /// <param name="user">username</param>
        /// <param name="password">password</param>
        public void SetBasicAuthentication(string user, string password)
        {
            this._user = user;
            this._password = password;
        }

        public Worklog Add(Worklog worklog)
        {
            throw new NotImplementedException();
        }

        public ITempoDomain Tempo => (ITempoDomain)this;

        public Uri JiraGreenhopperRestUri { get; }

        public IGreenhopperDomain Greenhopper => (IGreenhopperDomain) this;
    }
}
