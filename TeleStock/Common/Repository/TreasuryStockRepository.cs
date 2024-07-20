using Common.Interfaces;
using Common.Models;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Common.Repository
{
    public interface ITreasuryStockRepository
    {
        Task SaveAsync(Dictionary<string, TreasuryStock> treasuryStockDict, FileOption? option);
        Task<Dictionary<string, TreasuryStock>> LoadAllAsync(FileOption? option);
    }

    public class TreasuryStockRepository : ITreasuryStockRepository
    {
        private ILogger _logger;

        public TreasuryStockRepository(ILogger logger)
        {
            _logger = logger;
        }
        public async Task SaveAsync(Dictionary<string, TreasuryStock> treasuryStockDict, FileOption option)
        {
            try
            {
                var updatedSaavedData = await GetUpdatedSavedData(treasuryStockDict, option);
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true,
                };
                string json = JsonSerializer.Serialize(updatedSaavedData, options);
                await File.WriteAllTextAsync(Path.Combine(option.FilePath, option.FileName), json);
            }
            catch (Exception ex)
            {
                _logger.Log($"An error occured while saving the data: {ex.Message}");
                throw;
            }
        }

        private async Task<Dictionary<string, TreasuryStock>> GetUpdatedSavedData(Dictionary<string, TreasuryStock> treasuryStockDict, FileOption option)
        {
            var savedData = await LoadAllAsync(option);
            foreach (var stock in treasuryStockDict)
            {
                if (savedData.ContainsKey(stock.Key))
                {
                }
                else
                {
                    savedData[stock.Key] = stock.Value;
                }

            }
            return savedData;
        }

        // 파일 없을 때 LoadAsync 에러 나는지 확인
        public async Task<Dictionary<string, TreasuryStock>> LoadAllAsync(FileOption option)
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
