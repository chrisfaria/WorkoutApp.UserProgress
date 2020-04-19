using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserProgress.Service.Models
{
    public class Exersice
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "muscleGroup")]
        public string MuscleGroup { get; set; }

        [JsonProperty(PropertyName = "setCount")]
        public int SetCount { get; set; }

        [JsonProperty(PropertyName = "repCount")]
        public int RepCount { get; set; }

        [JsonProperty(PropertyName = "lastWeightUsed")]
        public int LastWeightUsed { get; set; }
    }
}
