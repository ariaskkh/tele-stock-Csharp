using System.Text;

namespace Common.Models
{
    public class Message
    {
        private TreasuryStock stock;
        public string MessageContent { get; set; } = string.Empty;
        
        public Message(TreasuryStock stock)
        {
            this.stock = stock;
            SetMessageForm();
        }

        /* [텔레 노출 form ex]
        휠라홀딩스(081660)
        자식주식<취득> 결정(신탁)

        금액(원) : 100 억
        유동주식대비(소액주주 기준): 0.66 %
        취득방법: 유가증권시장을 통한 장내 매수
        취득목적: 주식가격의 안정을 통한 주주가치 제고
        시작일 : 2024-03-21
        종료일 : 2024-09-20
        http://dart.fss.or.kr/dsaf001/main.do?rcpNo=report_number
        */
        public void SetMessageForm()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"< {stock.CorpName} - {stock.StockCode} >");
            sb.AppendLine($"{stock.ReportName}");
            sb.AppendLine();
            if (stock.IsOrdinaryStock)
            {
                sb.AppendLine($"금액(원): {stock.ExpectedAcquisitionMoney}억 (보통주식)");
            }
            else
            {
                sb.AppendLine($"금액(원): {stock.ExpectedAcquisitionMoney}억 (기타주식)");
            }
            sb.AppendLine($"유동주식수 대비 취득: {stock.AcquisitionRateOfFloatingStock} % (소액주주 기준)");
            sb.AppendLine($"취득방법: {stock.AcquisitionMethod}");
            sb.AppendLine($"취득목적: {stock.AcquisitionPurpose}");
            sb.AppendLine($"시작일: {stock.ExpectedAcquisitionStartDate}");
            sb.AppendLine($"종료일: {stock.ExpectedAcquisitionEndDate}");
            sb.AppendLine($"http://dart.fss.or.kr/dsaf001/main.do?rcpNo={stock.ReceiptNumber}");

            MessageContent = sb.ToString();
        }
    }
}
