
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public bool Corrected { get; set; } = false;

        [JsonConstructor]
        public TreasuryStock(
        string receiptNumber,
        string corpName,
        string stockCode,
        string reportName,
        string acquisitionMethod,
        string acquisitionPurpose,
        string expectedAcquisitionStartDate,
        string expectedAcquisitionEndDate,
        string plannedAcquisitionPriceOfOrdinaryStock,
        string plannedAcquisitionPriceOfExtraOrdinaryStock,
        bool isOrdinaryStock,
        string expectedAcquisitionMoney,
        string acquisitionRateOfFloatingStock,
        bool corrected)
        {
            ReceiptNumber = receiptNumber;
            CorpName = corpName;
            StockCode = stockCode;
            ReportName = reportName;
            AcquisitionMethod = acquisitionMethod;
            AcquisitionPurpose = acquisitionPurpose;
            ExpectedAcquisitionStartDate = expectedAcquisitionStartDate;
            ExpectedAcquisitionEndDate = expectedAcquisitionEndDate;
            PlannedAcquisitionPriceOfOrdinaryStock = plannedAcquisitionPriceOfOrdinaryStock;
            PlannedAcquisitionPriceOfExtraordinaryStock = plannedAcquisitionPriceOfExtraOrdinaryStock;
            IsOrdinaryStock = isOrdinaryStock;
            ExpectedAcquisitionMoney = expectedAcquisitionMoney;
            AcquisitionRateOfFloatingStock = acquisitionRateOfFloatingStock;
            Corrected = corrected;
        }

        public TreasuryStock(MajorInfoReport majorInfoReport, TreasuryDetailReport detailReport, string? holdStockCount)
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
            PlannedAcquisitionPriceOfExtraordinaryStock = detailReport.AcquisitionPriceOfExtraordinaryStock;
            IsOrdinaryStock = detailReport.IsOrdinaryStock;
            ExpectedAcquisitionMoney = detailReport.ExpectedAcquisitionMoney();
            AcquisitionRateOfFloatingStock = GetAcquisitionRateOfFloatingStock(detailReport, holdStockCount);
            Corrected = detailReport.Corrected;
        }

        private static string GetAcquisitionRateOfFloatingStock(TreasuryDetailReport detailReport, string? holdStockCount)
        {
            if (string.IsNullOrEmpty(holdStockCount) || holdStockCount == "-")
            {
                return "-";
            }
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
