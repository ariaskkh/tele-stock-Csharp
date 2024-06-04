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
    }
}
