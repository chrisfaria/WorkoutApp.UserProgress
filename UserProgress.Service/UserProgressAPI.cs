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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "progress/program")] HttpRequest req,
            [CosmosDB(
                databaseName: "UserProgress",
                collectionName: "UserPrograms",
                ConnectionStringSetting = "AzureWebJobsStorage")] DocumentClient userProgsCient,
            [CosmosDB(
                databaseName: "UserProgress",
                collectionName: "UserProgramDetails",
                ConnectionStringSetting = "AzureWebJobsStorage")]
                IAsyncCollector<UserProgramDetail> userProgDetsOut,
            ILogger log)
        {
            List<Document> output = new List<Document>();

            log.LogInformation("Initializa a new user program");

            // Get the program details
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var inputProgDetails = JsonConvert.DeserializeObject<UserProgramDetailCreateModel>(requestBody);

            // Get the active program for this user
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("UserProgress", "UserPrograms");
            IDocumentQuery<UserProgram> query = userProgsCient.CreateDocumentQuery<UserProgram>(collectionUri)
                .Where(p => p.Username.Equals(inputProgDetails.Username))
                .Where(p => p.Status.Equals("active"))
                .AsDocumentQuery();

            try
            {
                while (query.HasMoreResults)
                {
                    foreach (Document doc in await query.ExecuteNextAsync())
                    {
                        UserProgram userProgram = (dynamic)doc;
                    
                        // If the program has already been started let the caller know and don't continue
                        foreach (var program in userProgram.Programs)
                        {
                            if (program.Id == inputProgDetails.ProgId)
                            {
                                return new ConflictObjectResult("Program already started");
                            }
                        }

                        // If the program hasn't started then you're here, update the active program list
                        userProgram.Programs.Add(new Program()
                        {
                            Id = inputProgDetails.ProgId,
                            Name = inputProgDetails.ProgName
                        });
                        output.Add(await userProgsCient.ReplaceDocumentAsync(doc.SelfLink, userProgram));

                        // Map the create model to the database required model
                        var progDetail = new UserProgramDetail()
                        {
                            Username = inputProgDetails.Username,
                            ProgId = inputProgDetails.ProgId,
                            ProgName = inputProgDetails.ProgName,
                            TotalDays = inputProgDetails.TotalDays,
                            Exersices = inputProgDetails.Exersices
                        };

                        // Create the framework in the program details container
                        await userProgDetsOut.AddAsync(progDetail);
                    }
                }
            }
            catch (Exception e)
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(output);
        }

        [FunctionName("StartTodaysWorkout")]
        public static async Task<IActionResult> StartTodaysWorkout(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "progress/program/start")] HttpRequest req,
            [CosmosDB(
                databaseName: "UserProgress",
                collectionName: "UserProgramDetails",
                ConnectionStringSetting = "AzureWebJobsStorage")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("Starting today's program");

            // Get the active program for this user
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("UserProgress", "UserProgramDetails");
            IDocumentQuery<UserProgramDetail> query = client.CreateDocumentQuery<UserProgramDetail>(collectionUri)
                .Where(p => p.Id.Equals(req.Query["id"]))
                .Where(p => p.Username.Equals(req.Query["username"]))
                .AsDocumentQuery();

            try
            {
                while (query.HasMoreResults)
                {
                    //Document d = query.ExecuteNextAsync().Result.FirstOrDefault();
                    foreach (Document doc in await query.ExecuteNextAsync())
                    {
                        UserProgramDetail userProgDetail = (dynamic)doc;

                        // Update the day number count
                        userProgDetail.DayNo++;
                        await client.ReplaceDocumentAsync(doc.SelfLink, userProgDetail);

                        // Return the details used to build todays workout
                        return new OkObjectResult(userProgDetail);
                    }
                }
            }
            catch (Exception e)
            {
                return new BadRequestResult();
            }

            return new BadRequestObjectResult("Program detail wasn't able to update");
        }

        [FunctionName("AddUserWorkoutSet")]
        public static async Task<IActionResult> AddUserWorkoutSet(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "progress/program/workoutset")] HttpRequest req,
            [CosmosDB(
                databaseName: "UserProgress",
                collectionName: "WorkoutSets",
                ConnectionStringSetting = "AzureWebJobsStorage")]
                IAsyncCollector<WorkoutSet> workoutSet,
            ILogger log)
        {
            log.LogInformation("Add a new workout set logging the users workout progress");

            // Read the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<WorkoutSet>(requestBody);

            try
            {
                await workoutSet.AddAsync(input);
            }
            catch (Exception e)
            {
                return new BadRequestResult();
            }
            return new OkObjectResult(input);
        }

        [FunctionName("CreateUserProgramListing")]
        public static async Task<IActionResult> CreateUserProgramListing(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "progress/program/listing")] HttpRequest req,
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
                return new BadRequestResult();
            }
            return new OkObjectResult(userprogram);
        }
    }
}
