using lib.Models;
using lib.Utils;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;

namespace lib.Services;

public class BroennoeysundApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private static readonly SemaphoreSlim _semaphore = new(10, 10);

    public BroennoeysundApiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _baseUrl = config["BroennoeysundUrl"]!;
    }

    public async Task<List<Company>> LookupCompanies(string name, int size = 100)
    {
        if (string.IsNullOrEmpty(name))
            return [];

        try // simple "rate" limit, prevent excess search queries from clogging up the back-end.
        {
            await _semaphore.WaitAsync();
        }
        catch
        {
            return [];
        }

        try
        {
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
        finally
        {
            _semaphore.Release();
        }
    }
}