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

    }
}
