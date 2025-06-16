using MongoDB.Driver;

namespace TransitGtfsApi.Service.Database;

public abstract class MongoService<T>
{
    protected readonly IMongoCollection<T> _collection;
    protected readonly ILogger _logger;

    protected MongoService(IMongoDatabase database, ILogger logger, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
        _logger = logger;
    }

    protected async Task ImportFromCsvAsync(string filePath, Func<Dictionary<string, string?>, T> parseFunction)
    {
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"File not found: {filePath}");
            return;
        }

        try
        {
            _logger.LogInformation($"Importing data from {filePath}");

            var entities = new List<T>();
            string[] lines = await File.ReadAllLinesAsync(filePath);

            if (lines.Length <= 1)
            {
                _logger.LogWarning($"No data found in {filePath}");
                return;
            }

            string[] headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(',');
                var rowData = new Dictionary<string, string?>();

                for (int j = 0; j < headers.Length; j++)
                {
                    if (j < values.Length)
                    {
                        rowData[headers[j]] = string.IsNullOrWhiteSpace(values[j]) ? null : values[j];
                    }
                }

                T entity = parseFunction(rowData);
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
            _logger.LogError(ex, $"\nError importing data from {filePath}");
            throw;
        }
    }
}