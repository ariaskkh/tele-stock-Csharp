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
    }
}
