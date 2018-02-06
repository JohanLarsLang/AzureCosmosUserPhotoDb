﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;

namespace databaslab4
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhotoId { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsApproved { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
