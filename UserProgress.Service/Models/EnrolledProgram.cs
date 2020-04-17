using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserProgress.Service.Models
{
    public class EnrolledProgram
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public List<Program> Programs { get; set; }
        public string Status { get; set; }
    }

    public class EnrolledProgramCreateModel
    {
        public string UserName { get; set; }
        public List<Program> Programs { get; set; }
        public string Status { get; set; }
    }

    public class EnrolledProgramTableEntity : TableEntity
    {
        [JsonProperty(PropertyName = "username")]
        public string UserNamen { get; set; }
        public List<Program> Programs { get; set; }
        public string Status { get; set; }
    }
}
