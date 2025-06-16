using TransitGtfs.Api.Models;

namespace TransitGtfs.Api.Interfaces;

public interface ITransfersService
{
    Task<List<Transfer>> GetAllAsync();
    Task<List<Transfer>?> GetByFromStopIdAsync(string fromStopId);
    Task ImportDataAsync(string directoryPath);
}