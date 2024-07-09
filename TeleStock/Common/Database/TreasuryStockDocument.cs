using Common.Interfaces;
using Common.Models;
using System.Text.Json;

namespace Common.Database
{
    public interface ITreasuryStockDocument
    {
        Task SaveAsync(Dictionary<string, TreasuryStock> treasuryStockDict, FileOption? option);
        Task<Dictionary<string, TreasuryStock>> LoadAsync(FileOption? option);
    }

    public class TreasuryStockDocument : ITreasuryStockDocument
    {
        private ILogger _logger;

        public TreasuryStockDocument(ILogger logger)
        {
            _logger = logger;
        }
        public async Task SaveAsync(Dictionary<string, TreasuryStock> treasuryStockDict, FileOption option)
        {
            try
            {
                var updatedSaavedData = await GetUpdatedSavedData(treasuryStockDict, option);
                string json = JsonSerializer.Serialize(updatedSaavedData);
                await File.WriteAllTextAsync(Path.Combine(option.FilePath, option.FileName), json.ToString());
            }
            catch (Exception ex)
            {
                _logger.Log($"An error occured while saving the data: {ex.Message}");
                throw;
            }
        }

        private async Task<Dictionary<string, TreasuryStock>> GetUpdatedSavedData(Dictionary<string, TreasuryStock> treasuryStockDict, FileOption option)
        {
            var savedData = await LoadAsync(option);
            if (savedData?.Any() ?? false)
            {
                foreach (var stock in treasuryStockDict)
                {
                    savedData[stock.Key] = stock.Value;
                }
            }
            return savedData;
        }

        // 파일 없을 때 LoadAsync 에러 나는지 확인
        public async Task<Dictionary<string, TreasuryStock>> LoadAsync(FileOption option)
        {
            try
            {
                var path = Path.Combine(option.FilePath, option.FileName);
                if (File.Exists(path))
                {
                    
                    string jsonData = await File.ReadAllTextAsync(path);
                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        var treasuryStockDict = JsonSerializer.Deserialize<Dictionary<string, TreasuryStock>>(jsonData);
                        return treasuryStockDict;
                    }
                    else
                    {
                        return new Dictionary<string, TreasuryStock>();
                    }
                }
                else
                {
                    return new Dictionary<string, TreasuryStock>();
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"An error occured while loading the data: {ex.Message}");
                throw;
            }
        }
    }

    public class FileOption
    {
        public string FileName { get; set; } = "TreasuryStock.json";
        /// <summary> Default Path: '~TelegramBotClient\bin\Debug\net8.0-windows' </summary>
        public string FilePath = "../../../../TelegramBot/Data";

        public FileOption() { }
        
        public FileOption(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }
    }
}
