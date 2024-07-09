using System.Text.Json;

namespace Common.Models
{
    public enum CorpClassType
    {
        Y, // 코스피
        K, // 코스닥
        N, // 코넥스
        E, // 기타
    }

    public class MajorInfoReport
    {
        public string ReceiptNumber { get; init; } // unique
        public CorpClassType CorpClass { get; init; } 
        public string CorpCode { get; init; }
        public string CorpName { get; init; }
        public string ReceiptDate { get; init; }
        public string ReportName { get; init; }
        public string StockCode { get; init; }
        public string Remarks { get; init; }

        public MajorInfoReport() { }

        public MajorInfoReport(JsonElement majorInfo)
        {
            ReceiptNumber = majorInfo.GetProperty("rcept_no").GetString() ?? string.Empty;
            ReceiptDate = majorInfo.GetProperty("rcept_dt").GetString() ?? string.Empty;
            CorpClass = (CorpClassType)Enum.Parse(typeof(CorpClassType), majorInfo.GetProperty("corp_cls").GetString() ?? string.Empty);
            CorpCode = majorInfo.GetProperty("corp_code").GetString() ?? string.Empty;
            CorpName = majorInfo.GetProperty("corp_name").GetString() ?? string.Empty;
            ReportName = majorInfo.GetProperty("report_nm").GetString() ?? string.Empty;
            StockCode = majorInfo.GetProperty("stock_code").GetString() ?? string.Empty;
            Remarks = majorInfo.GetProperty("rm").GetString() ?? string.Empty;
        }
    }
}
