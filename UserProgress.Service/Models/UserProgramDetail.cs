using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserProgress.Service.Models
{
    /// <summary>
    /// UserProgramDetails CosmosDB Entity
    /// Partition Key = /username
    /// Unique Key 1 = /username,/progId
    /// </summary>
    public class UserProgramDetail
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "progId")]
        public string ProgId { get; set; }

        [JsonProperty(PropertyName = "progName")]
        public string ProgName { get; set; }

        [JsonProperty(PropertyName = "totalDays")]
        public int TotalDays { get; set; }

        [JsonProperty(PropertyName = "dayNo")]
        public int DayNo { get; set; } = 0;

        [JsonProperty(PropertyName = "exercises")]
        public List<Exersice> Exersices { get; set; }
    }

    public class UserProgramDetailCreateModel
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "progId")]
        public string ProgId { get; set; }

        [JsonProperty(PropertyName = "progName")]
        public string ProgName { get; set; }

        [JsonProperty(PropertyName = "totalDays")]
        public int TotalDays { get; set; }

        [JsonProperty(PropertyName = "exercises")]
        public List<Exersice> Exersices { get; set; }
    }
}
