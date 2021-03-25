using Azure.Search.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AGL.HDSIndexCreator.Webjob.Models
{
    public class InteractionDetails
    {
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("account_number")]
        public string account_number { get; set; }

        [SimpleField(IsKey = true, IsFilterable = true)]
        [JsonPropertyName("reference_id")]
        public string reference_id { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("transaction_type")]
        public string transaction_type { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("channel")]
        public string channel { get; set; }

       // [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("date")]
        public DateTimeOffset date { get; set; }

        //[SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("time")]
        public TimeSpan time { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("description")]
        public string description { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("status")]
        public string status { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("created_by")]
        public string created_by { get; set; }

    }
}

