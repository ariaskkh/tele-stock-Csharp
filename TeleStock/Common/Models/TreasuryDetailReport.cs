namespace Common.Models
{
    public class TreasuryDetailReport 
    {
        // 사용 O
        public string ReceiptNumber { get; init; } // rcept_no, 접수번호
        public string AuditAttendance { get; init; } // adt_a_atn, 감사(사외이사가 아닌 감사위원)참석여부
        public string AcquisitionDate { get; init; } // aq_dd
        public string AcquisitionMethod { get; init; } // aq_mth
        public string AcquisitionPurpose { get; init; } // aq_pp
        public string AcquisitionWithinDevidendOrdinaryStock { get; init; } // aq_wtn_div_ostk, 취득 전 자기주식 보유현황 (배당가능 범위 내 취득)(보통주식)
        public string AcquisitionWithinDevidendOrdinaryStockRate { get; init; } // aq_wtn_div_ostk_rt, 취득 전 자기주식 보유현황(배당가능이익 범위 내 취득(주)(비율(%)))
        public string AcquisitionWithinDevidendExtraordinaryStock { get; init; } // aq_wtn_div_estk, 취득 전 자기주식 보유현황(배당가능이익 범위 내 취득(주)(기타주식))
        public string AcquisitionWithinDevidendExtraordinaryStockRate { get; init; } // aq_wtn_div_estk_rt, 취득 전 자기주식 보유현황(배당가능이익 범위 내 취득(주)(비율(%)))
        public string ExpectedAcquisitionStartDate { get; init; } // aqexpd_bgd
        public string ExpectedAcquisitionEndDate { get; init; } // aqexpd_edd
        public string AcquisitionPriceOfOrdinaryStock { get; init; } // aqpln_prc_ostk
        public string AcquisitionPriceOfExtraordinaryStock { get; init; } // aqpln_prc_estk
        public string AcquisitionNumberOfOrdinaryStock { get; init; } // aqpln_stk_ostk
        public string AcquisitionNumberOfExtraordinaryStock { get; init; } // aqpln_stk_estk
        public CorpClassType CorpClass { get; init; }
        public string CorpCode { get; init; }
        public string CorpName { get; init; }
        public string ConsignmentInvenstmentBrokerage { get; init; } // cs_iv_bk, 위탁 중개 사업자
        public string PurchaseLimitPerDayOfOrdinaryStock { get; init; } // d1_prodlm_ostk, 1일 매수 주문수량 한도(보통주식)
        public string PurchaseLimitPerDayOfExtraordinaryStock { get; init; } // d1_prodlm_estk, 1일 매수 주문수량 한도(기타주식)
        public string ExtraAcquisitionOrdinaryStock { get; init; } // eaq_ostk, 취득 전 자기주식 보유현황(기타취득(주)(보통주식))
        public string ExtraAcquisitionOrdinaryStockRate { get; init; } // eaq_ostk_rt, 취득 전 자기주식 보유현황(기타취득(주)(비율 %))
        public string ExtraAcquisitionExtraordinaryStock { get; init; } // eaq_estk, 취득 전 자기주식 보유현황(기타취득(주)(기타주식))
        public string ExtraAcquisitionExtraordinaryStockRate { get; init; } // eaq_estk_rt, 취득 전 자기주식 보유현황(기타취득(주)(비율 %))
        public string ExpectedHoldingPeriodStartData { get; init; } // hdexpd_bgd, 보유예상기간(시작일)
        public string ExpectedHoldingPeriodEndData { get; init; } // hdexpd_edd, 보유예상기간(종료일)
        public string OutsideDirectorAttendenceCount { get; init; } // od_a_at_t, 사외이사참석여부(참석(명))
        public string OutsideDirectorAbsentCount { get; init; } // od_a_at_b, 사외이사참석여부(참석(명))

        
        public bool IsOrdinaryStock => AcquisitionNumberOfOrdinaryStock != "-" || AcquisitionPriceOfOrdinaryStock != "-";

        public string ExpectedAcquisitionMoney()
        {
            if (IsOrdinaryStock)
            {
                return Math.Round((double)(long.Parse(AcquisitionPriceOfOrdinaryStock.Replace(",", "")) / 100000000)).ToString();
                
            }
            else
            {
                return Math.Round((double)(long.Parse(AcquisitionPriceOfExtraordinaryStock.Replace(",", "")) / 100000000)).ToString(); ;
            }
        }
    }
}
