using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsBridge.Models
{
    /// <summary>
    /// Represents the Task response from Graph
    /// </summary>
    public class CreatedTaskResponse
    {
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }

        [JsonProperty("planId")]
        public string PlanId { get; set; }

        [JsonProperty("bucketId")]
        public string BucketId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("orderHint")]
        public string OrderHint { get; set; }

        [JsonProperty("assigneePriority")]
        public string AssigneePriority { get; set; }

        [JsonProperty("percentComplete")]
        public long PercentComplete { get; set; }

        [JsonProperty("startDateTime")]
        public object StartDateTime { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTimeOffset CreatedDateTime { get; set; }

        [JsonProperty("dueDateTime")]
        public object DueDateTime { get; set; }

        [JsonProperty("hasDescription")]
        public bool HasDescription { get; set; }

        [JsonProperty("previewType")]
        public string PreviewType { get; set; }

        [JsonProperty("completedDateTime")]
        public object CompletedDateTime { get; set; }

        [JsonProperty("completedBy")]
        public object CompletedBy { get; set; }

        [JsonProperty("referenceCount")]
        public long ReferenceCount { get; set; }

        [JsonProperty("checklistItemCount")]
        public long ChecklistItemCount { get; set; }

        [JsonProperty("activeChecklistItemCount")]
        public long ActiveChecklistItemCount { get; set; }

        [JsonProperty("conversationThreadId")]
        public object ConversationThreadId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("createdBy")]
        public CreatedBy CreatedBy { get; set; }

        [JsonProperty("appliedCategories")]
        public AppliedCategories AppliedCategories { get; set; }

        [JsonProperty("assignments")]
        public AppliedCategories Assignments { get; set; }
    }

    public partial class AppliedCategories
    {
    }

    public partial class CreatedBy
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }

    public partial class User
    {
        [JsonProperty("displayName")]
        public object DisplayName { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}

