using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserProgress.Service.Models
{
    /// <summary>
    /// WorkoutSets CosmosDB Entity
    /// Partition Key = /username
    /// Unique Key 1 = /username,/progId,/date,/exerciseId,/exerciseNo,/setNo
    /// </summary>
    public class WorkoutSet
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "progId")]
        public string ProgId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "exerciseId")]
        public string ExerciseId { get; set; }

        [JsonProperty(PropertyName = "exerciseName")]
        public string ExerciseName { get; set; }

        [JsonProperty(PropertyName = "muscleGroup")]
        public string MuscleGroup { get; set; }

        [JsonProperty(PropertyName = "exerciseNo")]
        public int ExerciseNo { get; set; }

        [JsonProperty(PropertyName = "setNo")]
        public int SetNo { get; set; }

        [JsonProperty(PropertyName = "repsCompleted")]
        public int RepsCompleted { get; set; } = 0;

        [JsonProperty(PropertyName = "weightUsedLbs")]
        public int WeightUsedLbs { get; set; } = 0;

        [JsonProperty(PropertyName = "skipped")]
        public bool Skipped { get; set; } = false;
    }
}
