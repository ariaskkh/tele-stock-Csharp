namespace Common.Models
{
    public class MinorityShareholderStatusReport
    {
        public string ReceiptNumber { get; init; } // "rcept_no", 접수번호
        public CorpClassType CorpClass { get; init; } // "corp_cls", 법인구분
        public string CorpCode { get; init; } // "corp_code, 고유번호
        public string CorpName { get; init; } // "corp_name, 법인명
        public string Separation { get; init; } // "se", 소액주주
        public string ShareholderCount { get; init; } // "shrholdr_co, 주주수
        public string ShareholderTotalCount {  get; init; } // "shrholdr_tot_co", 전체 주주수
        public string ShareholderRate { get; init; } // "shrholdr_rate", 주주 비율
        public string HoldStockCount { get; init; } // "hold_stock_co", 보유 주식수
        public string StockTotalCount { get; init; } // "stock_tot_co", 총발행 주식수
        public string HoldStockRate { get; init; } // "hold_stock_rate", 보유 주식 비율
    }
}
