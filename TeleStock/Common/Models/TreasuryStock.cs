using Newtonsoft.Json.Linq;

namespace Common.Models
{
    public class TreasuryStock
    {
        public string ReceiptNumber { get; init; }
        public string CorpName { get; init; }
        public string StockCode { get; init; }
        public string ReportName { get; init; }
        public string AcquisitionMethod { get; init; }
        public string AcquisitionPurpose { get; init; }
        public string ExpectedAcquisitionStartDate { get; init; }
        public string ExpectedAcquisitionEndDate { get; init; }
        public string PlannedAcquisitionPriceOfOrdinaryStock { get; init; }
        public string PlannedAcquisitionPriceOfExtraordinaryStock { get; init; }
        public bool IsOrdinaryStock { get; init; }
        public string ExpectedAcquisitionMoney { get; init; }
        public string AcquisitionRateOfFloatingStock { get; init; }

        public TreasuryStock(MajorInfoReport majorInfoReport, TreasuryDetailReport detailReport, string holdStockCount)
        {
            ReceiptNumber = majorInfoReport.ReceiptNumber;
            CorpName = majorInfoReport.CorpName;
            StockCode = majorInfoReport.StockCode;
            ReportName = majorInfoReport.ReportName;
            AcquisitionMethod = detailReport.AcquisitionMethod;
            AcquisitionPurpose = detailReport.AcquisitionPurpose;
            ExpectedAcquisitionStartDate = detailReport.ExpectedAcquisitionStartDate;
            ExpectedAcquisitionEndDate = detailReport.ExpectedAcquisitionEndDate;
            PlannedAcquisitionPriceOfOrdinaryStock = detailReport.AcquisitionPriceOfOrdinaryStock;
            IsOrdinaryStock = detailReport.IsOrdinaryStock;
            ExpectedAcquisitionMoney = detailReport.ExpectedAcquisitionMoney();
            AcquisitionRateOfFloatingStock = GetAcquisitionRateOfFloatingStock(detailReport, holdStockCount);
        }

        // db에서 Load할 때
        public TreasuryStock(JObject obj)
        {
            var a =obj["ReceiptNumber"];
            ReceiptNumber = (string)obj["ReceiptNumber"];
            CorpName = (string)obj["CorpName"];
            StockCode = (string)obj["StockCode"];
            ReportName = (string)obj["ReportName"];
            AcquisitionMethod = (string)obj["AcquisitionMethod"];
            AcquisitionPurpose = (string)obj["AcquisitionPurpose"];
            ExpectedAcquisitionStartDate = (string)obj["ExpectedAcquisitionStartDate"];
            ExpectedAcquisitionEndDate = (string)obj["ExpectedAcquisitionEndDate"];
            PlannedAcquisitionPriceOfOrdinaryStock = (string)obj["PlannedAcquisitionPriceOfOrdinaryStock"];
            IsOrdinaryStock = (bool)obj["IsOrdinaryStock"];
            ExpectedAcquisitionMoney = (string)obj["ExpectedAcquisitionMoney"];
            AcquisitionRateOfFloatingStock = (string)obj["AcquisitionRateOfFloatingStock"];
        }

        private static string GetAcquisitionRateOfFloatingStock(TreasuryDetailReport detailReport, string holdStockCount)
        {
            var acquisitionStockNumber = detailReport.AcquisitionNumberOfOrdinaryStock != "-"
                    ? detailReport.AcquisitionNumberOfOrdinaryStock
                    : detailReport.AcquisitionNumberOfExtraordinaryStock ?? string.Empty;

            var floatingStockNumber = holdStockCount.Replace(",", "") ?? string.Empty;
            var acquisitionRateOfFloatingStock = Math.Round(
                (double.Parse(acquisitionStockNumber) / double.Parse(floatingStockNumber) * 100),
                2)
                .ToString();
            return acquisitionRateOfFloatingStock;
        }
    }
}
