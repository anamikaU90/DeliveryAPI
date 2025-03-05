using System.Collections.Generic;
using System.Text.Json.Serialization;

public class OrderRequest
{
    [JsonPropertyName("order")]
    public Dictionary<string,int> Order{get; set;}

}