using AP.Architecture;
using AP.Architecture.Providers;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace PAW3.Mvc.ServiceLocator;

public interface IServiceTaskService
{
    Task<IEnumerable<T>> GetDataAsync<T>(string name) where T : class;
    Task<T?> GetByIdAsync<T>(string name, int id) where T : class;
    Task<bool> CreateAsync<T>(string name, T entity) where T : class;
    Task<bool> UpdateAsync<T>(string name, int id, T entity) where T : class;
    Task<bool> DeleteAsync(string name, int id);
}

public class TaskApiClient(IRestProvider restProvider, IConfiguration configuration) : IServiceTaskService
{
    private const string ServiceLocatorBaseUrl = "https://localhost:7068/api/";

    public async Task<IEnumerable<T>> GetDataAsync<T>(string name) where T : class
    {
        var endpoint = $"{ServiceLocatorBaseUrl}{GetApiEndpoint(name)}";
        var response = await restProvider.GetAsync(endpoint, name);
        return await JsonProvider.DeserializeAsync<IEnumerable<T>>(response);
    }

    public async Task<T?> GetByIdAsync<T>(string name, int id) where T : class
    {
        try
        {
            var endpoint = $"{ServiceLocatorBaseUrl}{GetApiEndpoint(name)}/{(id)}";
            var response = await restProvider.GetAsync(endpoint, id.ToString());
            return await JsonProvider.DeserializeAsync<T>(response);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CreateAsync<T>(string name, T entity) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(entity);
            var endpoint = $"{ServiceLocatorBaseUrl}{GetApiEndpoint(name)}";
            var response = await restProvider.PostAsync(endpoint, json);
            return !string.IsNullOrEmpty(response);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync<T>(string name, int id, T entity) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(entity);
            var endpoint = $"{ServiceLocatorBaseUrl}{GetApiEndpoint(name)}/{(id)}";
            var response = await restProvider.PutAsync(endpoint, id.ToString(), json);
            return !string.IsNullOrEmpty(response);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string name, int id)
    {
        try
        {
            var endpoint = $"{ServiceLocatorBaseUrl}{GetApiEndpoint(name)}/{(id)}";
            var response = await restProvider.DeleteAsync(endpoint, id.ToString());
            return !string.IsNullOrEmpty(response);
        }
        catch
        {
            return false;
        }
    }

    private string GetApiEndpoint(string name)
    {
        return name.ToLower() switch
        {
            "task" => "TaskApi",
            _ => throw new ArgumentException($"Unknown endpoint: {name}")
        };
    }
}