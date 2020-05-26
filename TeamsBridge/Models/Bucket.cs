using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsBridge.Models
{
    public class Bucket
    {
        /// <summary>
        /// The Bucket DTO will be passed into the Planner controller via the body of the call to the API
        /// These should be passed in from the client call as the JSON body
        /// </summary>
        public string PlanID { get; set; }
        public string BucketID { get; set; }
    }
}
