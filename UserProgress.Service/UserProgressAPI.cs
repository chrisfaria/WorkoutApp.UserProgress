using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserProgress.Service.Models;

namespace UserProgress.Service
{
    public static class UserProgressAPI
    {
        [FunctionName("CreateUserProgram")]
        public static async Task<IActionResult> CreateUserProgram(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "progress/program")] HttpRequest req,
            [CosmosDB(
                databaseName: "WorkoutApp",
                collectionName: "UserPrograms",
                ConnectionStringSetting = "AzureWebJobsStorage")]
                IAsyncCollector<UserProgramTableEntity> userProgramOut,
            ILogger log)
        {
            log.LogInformation("Create a new user program");

            // Read the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<UserProgramCreateModel>(requestBody);

            var userprogram = new UserProgramTableEntity()
            {
                UserName = input.UserName,
                Programs = input.Programs,
                Status = input.Status
            };

            await userProgramOut.AddAsync(userprogram);

            return new OkObjectResult(userprogram);
        }
    }
}
