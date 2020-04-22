using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserProgress.Service.Models
{
    public class Program
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
