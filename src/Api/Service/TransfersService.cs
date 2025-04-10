using Api.Interfaces;
using Api.Models;
using MongoDB.Driver;

namespace Api.Service;

public class TransfersService : BaseGtfsService<Transfer>, ITransfersService
    {
        public TransfersService(IMongoDatabase database, ILogger<TransfersService> logger)
            : base(database, logger, "transfers")
        {
        }

        public async Task<List<Transfer>> GetAllAsync()
        {
            return await _collection.Find(Builders<Transfer>.Filter.Empty).ToListAsync();
        }

        public async Task<List<Transfer>> GetByFromStopIdAsync(string fromStopId)
        {
            return await _collection.Find(t => t.FromStopId == fromStopId).ToListAsync();
        }

        public async Task ImportDataAsync(string directoryPath)
        {
            string filePath = Path.Combine(directoryPath, "transfers.txt");
            await ImportFromCsvAsync(filePath, fields => new Transfer
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                FromStopId = fields[0],
                ToStopId = fields[1],
                TransferType = int.Parse(fields[2])
            });
        }
    }