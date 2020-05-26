using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamsBridge.Models
{
    /// <summary>
    /// Represents the TaskDetails for a new Task. This is used to create standard tasks upon request
    /// </summary>
    public class TaskDetails
    {
       
        /// <summary>
        /// Creates a serialized string of Task standard task details
        /// </summary>
        /// <returns></returns>
       public string GetNewSerializedTaskDetails()
        {
            var details = new TaskDetailsData()
            {
                Description = "Task Description goes here",
                PreviewType = "noPreview",
                Checklist = new Dictionary<Guid, ChecklistItem>()
            };
            var checklist1 = new ChecklistItem()
            {

                Type = "microsoft.graph.plannerChecklistItem",
                Title = "Checklist Item 1",
                IsChecked = false
            };
            var checklist2 = new ChecklistItem()
            {

                Type = "microsoft.graph.plannerChecklistItem",
                Title = "Checklist Item 2",
                IsChecked = false
            };
            details.Checklist.Add(Guid.NewGuid(), checklist1);
            details.Checklist.Add(Guid.NewGuid(), checklist2);
            var content = JsonConvert.SerializeObject(details);
            return content;
        }
    }
    /// <summary>
    /// Represents the TaskDetail data that will be serialized and sent to the Graph API. Each Checklist item should be added after the CheckList is instantiated
    /// </summary>
    public class TaskDetailsData
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("previewType")]
        public string PreviewType { get; set; }

        [JsonProperty("checklist")]
        public Dictionary<Guid, ChecklistItem> Checklist { get; set; }
    }

    /// <summary>
    /// Represents the individual checklist item
    /// </summary>
    public class ChecklistItem
    {
        [JsonProperty("@odata.type")]
        public string Type { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("isChecked")]
        public bool IsChecked { get; set; }
    }

}


