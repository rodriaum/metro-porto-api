using MetroPorto.Api.Models;

namespace MetroPorto.Api.Interfaces;

public interface ITransfersService
{
    Task<List<Transfer>> GetAllAsync();
    Task<List<Transfer>?> GetByFromStopIdAsync(string fromStopId);
    Task ImportDataAsync(string directoryPath);
}