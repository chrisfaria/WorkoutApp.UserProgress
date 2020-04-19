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
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "progId")]
        public string ProdId { get; set; }

        [JsonProperty(PropertyName = "totalDays")]
        public int TotalDays { get; set; }

        [JsonProperty(PropertyName = "dayNo")]
        public int DayNo { get; set; }

        [JsonProperty(PropertyName = "exercises")]
        public List<Exersice> Exersices { get; set; }
    }
}
