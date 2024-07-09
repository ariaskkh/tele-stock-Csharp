using System.Text.Json;

namespace Common.Models
{
    public class MinorityShareholderStatusReport
    {
        /// <summary> "rcept_no", 접수번호 </summary>
        public string ReceiptNumber { get; init; }
        /// <summary> "corp_cls", 법인구분 </summary>
        public CorpClassType CorpClass { get; init; }
        /// <summary> "corp_code, 고유번호 </summary>
        public string CorpCode { get; init; }
        /// <summary> "corp_name, 법인명 </summary>
        public string CorpName { get; init; }
        /// <summary> "se", 소액주주  </summary>
        public string Separation { get; init; } 
        /// <summary> "shrholdr_co, 주주수 </summary>
        public string ShareholderCount { get; init; }
        /// <summary> "shrholdr_tot_co", 전체 주주수 </summary>
        public string ShareholderTotalCount { get; init; }
        /// <summary> "shrholdr_rate", 주주 비율 </summary>
        public string ShareholderRate { get; init; }
        /// <summary> "hold_stock_co", 보유 주식수 </summary>
        public string HoldStockCount { get; init; }
        /// <summary> "stock_tot_co", 총발행 주식수 </summary>
        public string StockTotalCount { get; init; }
        /// <summary> "hold_stock_rate", 보유 주식 비율 </summary>
        public string HoldStockRate { get; init; } 

        public MinorityShareholderStatusReport(JsonElement minorityShareholderJson, string receiptNumber)
        {
            ReceiptNumber = receiptNumber; // minority데이터에 없어서 detail에서 가져옴. key로 쓰임
            CorpClass = (CorpClassType)Enum.Parse(typeof(CorpClassType), minorityShareholderJson.GetProperty("corp_cls").GetString() ?? string.Empty);
            CorpCode = minorityShareholderJson.GetProperty("corp_code").GetString() ?? string.Empty;
            CorpName = minorityShareholderJson.GetProperty("corp_name").GetString() ?? string.Empty;
            Separation = minorityShareholderJson.GetProperty("se").GetString() ?? string.Empty;
            ShareholderCount = minorityShareholderJson.GetProperty("shrholdr_co").GetString() ?? string.Empty;
            ShareholderTotalCount = minorityShareholderJson.GetProperty("shrholdr_tot_co").GetString() ?? string.Empty;
            ShareholderRate = minorityShareholderJson.GetProperty("shrholdr_rate").GetString() ?? string.Empty;
            HoldStockCount = minorityShareholderJson.GetProperty("hold_stock_co").GetString() ?? string.Empty;
            StockTotalCount = minorityShareholderJson.GetProperty("stock_tot_co").GetString() ?? string.Empty;
            HoldStockRate = minorityShareholderJson.GetProperty("hold_stock_rate").GetString() ?? string.Empty;
        }
    }
}
