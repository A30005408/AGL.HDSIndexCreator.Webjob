using Azure.Search.Documents.Indexes;
using System.Text.Json.Serialization;

namespace AGL.HDSIndexCreator.Webjob.Models
{
    public class Customer
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        [JsonPropertyName("account_number")]
        public string account_number { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("customer_type")]
        public string customer_type { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("account_name")]
        public string account_name { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("first_name")]
        public string first_name { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("last_name")]
        public string last_name { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("site_identifier")]
        public string site_identifier { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("account_status")]
        public string account_status { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("account_open_date")]
        public string account_open_date { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("account_closed_date")]
        public string account_closed_date { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("email_address")]
        public string email_address { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("mobile_no")]
        public string mobile_no { get; set; }
        [SearchableField(IsFilterable = true, IsSortable = true)]
        [JsonPropertyName("phone_no")]
        public string phone_no { get; set; }
        
    }
}
