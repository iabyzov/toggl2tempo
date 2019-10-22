﻿using System;
using System.Collections.Generic;
using Toggl.Interfaces;

namespace Toggl.Services
{
    public class TagService : ITagService
    {
        private IApiService ToggleSrv { get; set; }


        public TagService(string apiKey)
            : this(new ApiService(apiKey))
        {

        }

        public TagService(IApiService srv)
        {
            ToggleSrv = srv;
        }

        /// <summary>
        /// 
        /// https://www.toggl.com/public/api#get_tags
        /// </summary>
        /// <returns></returns>
        public List<Client> List()
        {

            throw new NotImplementedException();
        }
    }
}
