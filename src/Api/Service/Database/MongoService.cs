using MongoDB.Driver;

namespace MetroPortoAPI.Api.Service.Database;

public abstract class MongoService<T>
{
    protected readonly IMongoCollection<T> _collection;
    protected readonly ILogger _logger;

    protected MongoService(IMongoDatabase database, ILogger logger, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
        _logger = logger;
    }

    protected async Task ImportFromCsvAsync(string filePath, Func<string[], T> parseFunction)
    {
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"File not found: {filePath}");
            return;
        }

        try
        {
            _logger.LogInformation($"Importing data from {filePath}");

            // Delete all existing documents
            await _collection.DeleteManyAsync(Builders<T>.Filter.Empty);

            var entities = new List<T>();
            string[] lines = await File.ReadAllLinesAsync(filePath);

            if (lines.Length <= 1)
            {
                _logger.LogWarning($"No data found in {filePath}");
                return;
            }

            // Skip header line
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] fields = line.Split(',');
                T entity = parseFunction(fields);
                entities.Add(entity);
            }

            if (entities.Count > 0)
            {
                await _collection.InsertManyAsync(entities);
                _logger.LogInformation($"Imported {entities.Count} records from {filePath}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error importing data from {filePath}");
            throw;
        }
    }
}