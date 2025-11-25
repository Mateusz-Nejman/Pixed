using Pixed.Core.Models;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Common.Services;

public interface IHistoryService
{
    void Register(PixedModel model);
    Task AddToHistory(PixedModel model, bool setIsEmpty = true);
    void CopyHistoryFrom(PixedModel from, PixedModel to);
    int GetHistoryIndex(PixedModel model);
    string GetHistoryId(PixedModel model, int index);
    void SetHistoryIndex(PixedModel model, int index);
    int GetHistoryCount(PixedModel model);
    Task<Stream?> GetHistoryItem(PixedModel model, string id);
    Task ClearTempFiles();
}