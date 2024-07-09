
using System.Text.Json;

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
            Corrected = detailReport.Corrected;
        }

        // db에서 Load할 때
        public TreasuryStock(JsonElement obj)
        {
            ReceiptNumber = obj.GetProperty("ReceiptNumber").GetString() ?? string.Empty;
            CorpName = obj.GetProperty("CorpName").GetString() ?? string.Empty;
            StockCode = obj.GetProperty("StockCode").GetString() ?? string.Empty;
            ReportName = obj.GetProperty("ReportName").GetString() ?? string.Empty;
            AcquisitionMethod = obj.GetProperty("AcquisitionMethod").GetString() ?? string.Empty;
            AcquisitionPurpose = obj.GetProperty("AcquisitionPurpose").GetString() ?? string.Empty;
            ExpectedAcquisitionStartDate = obj.GetProperty("ExpectedAcquisitionStartDate").GetString() ?? string.Empty;
            ExpectedAcquisitionEndDate = obj.GetProperty("ExpectedAcquisitionEndDate").GetString() ?? string.Empty;
            PlannedAcquisitionPriceOfOrdinaryStock = obj.GetProperty("PlannedAcquisitionPriceOfOrdinaryStock").GetString() ?? string.Empty;
            IsOrdinaryStock = obj.GetProperty("IsOrdinaryStock").GetBoolean();
            ExpectedAcquisitionMoney = obj.GetProperty("ExpectedAcquisitionMoney").GetString() ?? string.Empty;
            AcquisitionRateOfFloatingStock = obj.GetProperty("AcquisitionRateOfFloatingStock").GetString() ?? string.Empty;
            Corrected = obj.GetProperty("Corrected").GetBoolean();
        }

        private static string GetAcquisitionRateOfFloatingStock(TreasuryDetailReport detailReport, string holdStockCount)
        {
            if (holdStockCount == "-")
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
