using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserProgress.Service.Models
{
    /// <summary>
    /// UserPrograms CosmosDB Entity
    /// Partition Key = /username
    /// Unique Key 1 = /username,/type
    /// </summary>
    public class UserProgram
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "programs")]
        public List<Program> Programs { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

    public class UserProgramCreateModel
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "programs")]
        public List<Program> Programs { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}
