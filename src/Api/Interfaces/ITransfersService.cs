using Api.Models;

namespace Api.Interfaces;

public interface ITransfersService
{
    Task<List<Transfer>> GetAllAsync();
    Task<List<Transfer>> GetByFromStopIdAsync(string fromStopId);
    Task ImportDataAsync(string directoryPath);
}