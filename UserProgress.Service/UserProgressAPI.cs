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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents;

namespace UserProgress.Service
{
    public static class UserProgressAPI
    {
        [FunctionName("InitializeUserProgram")]
        public static async Task<IActionResult> InitializeUserProgram(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "progress/program/{username}/{progId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "UserProgress",
                collectionName: "UserPrograms",
                ConnectionStringSetting = "AzureWebJobsStorage",
                PartitionKey = "{username}",
                SqlQuery = "SELECT top 1 * FROM c where c.status = 'active'")]
                IEnumerable<UserProgram> userPrograms,
            [CosmosDB(
                databaseName: "UserProgress",
                collectionName: "UserPrograms",
                ConnectionStringSetting = "AzureWebJobsStorage")] DocumentClient userProgsCient,
            //[CosmosDB(
            //    databaseName: "UserProgress",
            //    collectionName: "UserPrograms",
            //    ConnectionStringSetting = "AzureWebJobsStorage")]
            //    IAsyncCollector<UserProgram> userProgramOut,
            ILogger log, string username, string progId)
        {
            log.LogInformation("Initializa a new user program");

            List<Document> output = new List<Document>();
            var userProgram = userPrograms.FirstOrDefault();

            if (userProgram != null)
            {
                // If the program has already been started let the caller know and don't continue
                foreach (var program in userProgram.Programs)
                {
                    if (program.Id == progId)
                    {
                        return new ConflictObjectResult("Program already started");
                    }
                }

                // Get the program details
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var inputProgDetails = JsonConvert.DeserializeObject<UserProgramDetail>(requestBody);

                // If the program hasn't started then you're here, update the active program list
                userProgram.Programs.Add(new Program() { 
                    Id = inputProgDetails.ProgId,
                    Name = inputProgDetails.ProgName
                });


                Uri collectionUri = UriFactory.CreateDocumentCollectionUri("UserProgress", "UserPrograms");
                IDocumentQuery<UserProgram> query = userProgsCient.CreateDocumentQuery<UserProgram>(collectionUri)
                    .Where(p => p.Username.Equals(username))
                    .Where(p => p.Status.Equals("active"))
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    foreach (Document doc in await query.ExecuteNextAsync())
                    {
                        UserProgram replacement = (dynamic)doc;
                        replacement.Programs.Add(new Program()
                        {
                            Id = inputProgDetails.ProgId,
                            Name = inputProgDetails.ProgName
                        });
                        output.Add(await userProgsCient.ReplaceDocumentAsync(doc.SelfLink, replacement));

                    }
                }


            }





            // Create the framework in the program details container


            return new OkObjectResult(output);
        }

        [FunctionName("CreateUserProgramListing")]
        public static async Task<IActionResult> CreateUserProgramListing(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "progress/listing")] HttpRequest req,
            [CosmosDB(
                databaseName: "UserProgress",
                collectionName: "UserPrograms",
                ConnectionStringSetting = "AzureWebJobsStorage")]
                IAsyncCollector<UserProgram> userProgramOut,
            ILogger log)
        {
            log.LogInformation("Create a new user program");

            // Read the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<UserProgramCreateModel>(requestBody);

            var userprogram = new UserProgram()
            {
                Username = input.Username,
                Programs = input.Programs,
                Status = input.Status
            };

            try
            {
                await userProgramOut.AddAsync(userprogram);
            }
            catch (Exception e)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(userprogram);
        }
    }
}
