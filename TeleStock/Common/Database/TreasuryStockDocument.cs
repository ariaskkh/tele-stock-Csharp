using Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Common.Database
{
    public interface ITreasuryStockDocument
    {
        Task SaveAsync(Dictionary<string, TreasuryStock> treasuryStockDict, FileOption? option);
        Task<Dictionary<string, TreasuryStock>> LoadAsync(FileOption? option);
        //bool CheckFileExistInThePath(string path, string fileName);
    }

    public class TreasuryStockDocument : ITreasuryStockDocument
    {
        public async Task SaveAsync(Dictionary<string, TreasuryStock> treasuryStockDict, FileOption option)
        {
            JObject json = JObject.FromObject(treasuryStockDict);
            await File.WriteAllTextAsync(Path.Combine(option.FilePath, option.FileName), json.ToString());
        }

        public async Task<Dictionary<string, TreasuryStock>> LoadAsync(FileOption option)
        {
            string jsonData = await File.ReadAllTextAsync(Path.Combine(option.FilePath, option.FileName));

            var jsonObject = JObject.Parse(jsonData);
            return jsonObject
                .ToObject<Dictionary<string, JObject>>()
                .Select(keyValuePair => new TreasuryStock(keyValuePair.Value))
                .ToDictionary(
                (treasuryStock) => treasuryStock.ReceiptNumber,
                (treasuryStock) => treasuryStock
                );
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
