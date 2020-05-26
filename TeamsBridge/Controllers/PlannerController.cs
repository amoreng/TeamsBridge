using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using TeamsBridge.Helpers;
using TeamsBridge.Models;

namespace TeamsBridge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlannerController : ControllerBase
    {
        
        private readonly TaskService _taskService;

        public PlannerController(TaskService taskService)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService)); ;
        }

        // GET: api/Planner
        [HttpGet]
        public string Get()
        {
            return "I'm Alive";
        }



        // POST: api/Planner
        [HttpPost]//[FromBody] string value
        public async Task<IActionResult> Post([FromBody] Bucket bucket)
        {
           var response = await _taskService.CreateTask(bucket);
           return response;
        }

    }
}
