using Azure.Search.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AGL.HDSIndexCreator.Webjob.Models
{
    public class ServiceOrder
    {
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("account_number")]
        public string account_number { get; set; }
        [SimpleField(IsKey = true, IsFilterable = true)]
        [JsonPropertyName("service_order_no")]
        public string service_order_no { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("service_order_type")]
        public string service_order_type { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("created_date")]
        public string created_date { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("requested_date")]
        public string requested_date { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("comments")]
        public string comments { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("serice_order_status")]
        public string serice_order_status { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("completion_notes")]
        public string completion_notes { get; set; }
    }
}
