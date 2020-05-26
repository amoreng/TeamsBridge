using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamsBridge.Models;

namespace TeamsBridge.Helpers
{
    public class TaskService : ControllerBase
    {
        private readonly IOptions<GraphSettings> _graphSettings;
        private readonly string _host;
        private readonly string _tenantID;
        private readonly string _clientID;
        private readonly string _clientSecret;
        private readonly string _userName;
        private readonly string _password;

        const string BaseURL = "https://graph.microsoft.com";
               
        public TaskService(IOptions<GraphSettings> app)
        {
            _graphSettings = app;
            _host = $"{_graphSettings.Value.Host}";
            _tenantID = $"{_graphSettings.Value.TenantID}";
            _clientID = $"{_graphSettings.Value.ClientID}";
            _clientSecret = $"{_graphSettings.Value.ClientSecret}";
            _userName = $"{_graphSettings.Value.UserName}";
            _password = $"{_graphSettings.Value.Password}";
        }
        /// <summary>
        /// Creates a new Task in the specific Planner Bucket
        /// </summary>
        /// <param name="bucket">JSON Body data includes PlanID and BucketID</param>
        /// <returns></returns>
        public async Task<IActionResult> CreateTask(Bucket bucket)
        {
           
            //get a valid token from Graph API, then call the Planner API to create a Planner Task.
            if (GraphUtils.DelegatedTokenRest(_host, _tenantID, _clientID, _userName, _password, _clientSecret, out string token))
            {
                if (!String.IsNullOrEmpty(token))
                {
                    var client = new RestClient(BaseURL)
                    {
                        Authenticator = new JwtAuthenticator(token)
                    };
                    var request = new RestRequest("/v1.0/planner/tasks", Method.POST);
                    request.AddHeader("Accept", "application/json");
                    var taskHead = new
                    {
                        planId = bucket.PlanID,
                        bucketId = bucket.BucketID,
                        title = "New Task from Teams Bridge"
                    };
                    var content = JsonConvert.SerializeObject(taskHead);
                    request.AddJsonBody(content);
                    IRestResponse response = await client.ExecuteAsync(request);
                    //contains the token                    
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.BadRequest:
                        case System.Net.HttpStatusCode.Unauthorized:                        
                        case System.Net.HttpStatusCode.Created:
                            {
                                //continue on and add task details
                                //dynamic json = response.Content;
                                var json = JsonConvert.DeserializeObject<CreatedTaskResponse>(response.Content);                               
                                var taskID = json.Id;
                                //call GetTaskDetails to get the current eTag for the Task Details, then update
                                var dtlResponse = await GetTaskDetails(token, taskID);
                                if(!string.IsNullOrEmpty(dtlResponse))
                                {
                                    var dtlJson = JsonConvert.DeserializeObject<CreatedTaskDetailResponse>(dtlResponse);
                                    var etag = dtlJson.OdataEtag;
                                    var updateResponse = await UpdateTaskDetails(etag, token, taskID);
                                    if (updateResponse)
                                    {
                                        return Ok(response.Content);
                                    }
                                    else
                                        return BadRequest(response.Content);
                                }
                                else
                                    return BadRequest(response.Content);

                            }
                        default:
                            {
                                return BadRequest(response.Content);

                            }
                    }
                }
                else
                    return BadRequest("Delegate token is null!");
            }
            else
                return BadRequest("Unable to get Delegate Token!");
        }

        /// <summary>
        /// Updates the Task Details of a Task object
        /// </summary>
        /// <param name="eTag">Comes from the json response from either a GET or a POST of a Task. Pass in Header If-Match</param>
        /// <param name="token">Bearer token should be passed as Bearer token in Header </param>
        /// <returns></returns>
        public async Task<bool> UpdateTaskDetails(string eTag, string token, string taskID)
        {
            if (!String.IsNullOrEmpty(token))
            {
                var client = new RestClient(BaseURL)
                {
                    Authenticator = new JwtAuthenticator(token)
                };
                var request = new RestRequest($"/v1.0/planner/tasks/{taskID}/details", Method.PATCH);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("If-Match", eTag);
                var content = new TaskDetails().GetNewSerializedTaskDetails();

                request.AddJsonBody(content);
                IRestResponse response = await client.ExecuteAsync(request);
                //contains the token                    
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Unauthorized:
                    case System.Net.HttpStatusCode.PreconditionFailed:
                        {
                            return false;
                        }
                    case System.Net.HttpStatusCode.OK:
                        {
                            //return content from OK response                        
                            return true;
                        }
                    case System.Net.HttpStatusCode.NoContent:
                        {
                            //bug in Graph that returns NoContent in an OK response
                            return true;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            else
                return false;         
        }

        /// <summary>
        /// Call GetTaskDetails after new Task is created to return the eTag required to upate the Task Details
        /// </summary>
        /// <param name="token">Bearer token</param>
        /// <param name="taskID">Task ID</param>
        /// <returns></returns>
        public async Task<string> GetTaskDetails(string token, string taskID)
        {
            if (!String.IsNullOrEmpty(token))
            {
                var client = new RestClient(BaseURL)
                {
                    Authenticator = new JwtAuthenticator(token)
                };
                var request = new RestRequest($"/v1.0/planner/tasks/{taskID}/details", Method.GET);
                request.AddHeader("Accept", "application/json");        

                IRestResponse response = await client.ExecuteAsync(request);
                //contains the token                    
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest:
                    case System.Net.HttpStatusCode.Unauthorized:
                    case System.Net.HttpStatusCode.PreconditionFailed:
                        {
                            return null;
                        }
                    case System.Net.HttpStatusCode.OK:
                        {
                            //return content from OK response                        
                            return response.Content;
                        }
                    default:
                        {
                            return null;
                        }
                }
            }
            else
                return "Delegate token is null!";
        }
    }
}
