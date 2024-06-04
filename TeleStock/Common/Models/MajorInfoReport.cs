using Newtonsoft.Json.Linq;

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

        public MajorInfoReport(JToken majorInfo)
        {
            var a = majorInfo["rcept_no"];
            ReceiptNumber = majorInfo["rcept_no"].ToString();
            ReceiptDate = majorInfo["rcept_dt"].ToString();
            CorpClass = (CorpClassType)Enum.Parse(typeof(CorpClassType), majorInfo["corp_cls"].ToString());
            CorpCode = majorInfo["corp_code"].ToString();
            CorpName = majorInfo["corp_name"].ToString();
            ReportName = majorInfo["report_nm"].ToString();
            StockCode = majorInfo["stock_code"].ToString();
            Remarks = majorInfo["rm"].ToString();
        }
    }
}
