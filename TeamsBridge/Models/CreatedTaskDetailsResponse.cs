using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsBridge.Models
{
  /// <summary>
  /// Represents the TaskDetails response from Graph
  /// </summary>
   public partial class CreatedTaskDetailResponse
    {
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("previewType")]
        public string PreviewType { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("references")]
        public Checklist References { get; set; }

        [JsonProperty("checklist")]
        public Checklist Checklist { get; set; }
    }

    public partial class Checklist
    {
    }
}

