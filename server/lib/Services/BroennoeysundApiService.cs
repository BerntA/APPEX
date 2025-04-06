using lib.Models;
using lib.Utils;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;

namespace lib.Services;

public class BroennoeysundApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public BroennoeysundApiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _baseUrl = config["BroennoeysundUrl"]!;
    }

    public async Task<List<Company>> LookupCompanies(string name, int size = 100)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
                return [];

            using var response = await _httpClient.GetAsync($"{_baseUrl}/api/enheter?navn={name}&size={size}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jsonContent = JsonNode.Parse(content);

            if (jsonContent is null || jsonContent["_embedded"] is null || jsonContent["_embedded"]["enheter"] is null)
                return [];

            return JsonUtils.DeserializeObject<List<Company>>(jsonContent["_embedded"]["enheter"].ToJsonString());
        }
        catch
        {
            return [];
        }
    }
}