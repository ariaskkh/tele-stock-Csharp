using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.Database
{
    public interface ITreasuryStockDocument
    {
        Task Save<T1, T2>(Dictionary<T1, T2> treasuryStockDict, FileOption? option);
        //bool CheckFileExistInThePath(string path, string fileName);
        //Task<Dictionary<T1, T2>> Load();
    }

    public class TreasuryStockDocument : ITreasuryStockDocument
    {
        public async Task Save<T1, T2>(Dictionary<T1, T2> treasuryStockDict, FileOption option)
        {
            JObject json = JObject.FromObject(treasuryStockDict);
            await File.WriteAllTextAsync(Path.Combine(option.FilePath, option.FileName), json.ToString());
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
