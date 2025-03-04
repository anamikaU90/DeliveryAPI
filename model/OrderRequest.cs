using System.Collections.Generic;
using System.Text.Json.Serialization;

public class OrderRequest
{
    [JsonPropertyName("products")]
    public Dictionary<string,int> Products{get; set;}

}