﻿using System.Text.Json;

namespace Common.Models
{
    public class TreasuryDetailReport 
    {
        /// <summary> rcept_no, 접수번호 </summary>
        public string ReceiptNumber { get; set; }
        /// <summary> adt_a_atn, 감사(사외이사가 아닌 감사위원)참석여부 </summary>
        public string AuditAttendance { get; init; }
        /// <summary> aq_dd </summary>
        public string AcquisitionDate { get; init; }
        /// <summary> aq_mth </summary>
        public string AcquisitionMethod { get; init; }
        /// <summary> aq_pp  </summary>
        public string AcquisitionPurpose { get; init; }
        /// <summary> aq_wtn_div_ostk, 취득 전 자기주식 보유현황 (배당가능 범위 내 취득)(보통주식) </summary>
        public string AcquisitionWithinDevidendOrdinaryStock { get; init; }
        /// <summary> aq_wtn_div_ostk_rt, 취득 전 자기주식 보유현황(배당가능이익 범위 내 취득(주)(비율(%))) </summary>
        public string AcquisitionWithinDevidendOrdinaryStockRate { get; init; }
        /// <summary> aq_wtn_div_estk, 취득 전 자기주식 보유현황(배당가능이익 범위 내 취득(주)(기타주식)) </summary>
        public string AcquisitionWithinDevidendExtraordinaryStock { get; init; }
        /// <summary> aq_wtn_div_estk_rt, 취득 전 자기주식 보유현황(배당가능이익 범위 내 취득(주)(비율(%))) </summary>
        public string AcquisitionWithinDevidendExtraordinaryStockRate { get; init; }
        /// <summary> aqexpd_bgd </summary>
        public string ExpectedAcquisitionStartDate { get; init; }
        /// <summary> aqexpd_edd </summary>
        public string ExpectedAcquisitionEndDate { get; init; }
        /// <summary> aqpln_prc_ostk </summary>
        public string AcquisitionPriceOfOrdinaryStock { get; init; }
        /// <summary> aqpln_prc_estk </summary>
        public string AcquisitionPriceOfExtraordinaryStock { get; init; }
        /// <summary> aqpln_stk_ostk </summary>
        public string AcquisitionNumberOfOrdinaryStock { get; init; }
        /// <summary> aqpln_stk_estk </summary>
        public string AcquisitionNumberOfExtraordinaryStock { get; init; } 
        public CorpClassType CorpClass { get; init; }
        public string CorpCode { get; init; }
        public string CorpName { get; init; }
        /// <summary> cs_iv_bk, 위탁 중개 사업자 </summary>
        public string ConsignmentInvenstmentBrokerage { get; init; }
        /// <summary> d1_prodlm_ostk, 1일 매수 주문수량 한도(보통주식) </summary>
        public string PurchaseLimitPerDayOfOrdinaryStock { get; init; }
        /// <summary> d1_prodlm_estk, 1일 매수 주문수량 한도(기타주식) </summary>
        public string PurchaseLimitPerDayOfExtraordinaryStock { get; init; }
        /// <summary> eaq_ostk, 취득 전 자기주식 보유현황(기타취득(주)(보통주식)) </summary>
        public string ExtraAcquisitionOrdinaryStock { get; init; }
        /// <summary> eaq_ostk_rt, 취득 전 자기주식 보유현황(기타취득(주)(비율 %)) </summary>
        public string ExtraAcquisitionOrdinaryStockRate { get; init; }
        /// <summary> eaq_estk, 취득 전 자기주식 보유현황(기타취득(주)(기타주식)) </summary>
        public string ExtraAcquisitionExtraordinaryStock { get; init; }
        /// <summary> eaq_estk_rt, 취득 전 자기주식 보유현황(기타취득(주)(비율 %)) </summary>
        public string ExtraAcquisitionExtraordinaryStockRate { get; init; }
        /// <summary> hdexpd_bgd, 보유예상기간(시작일) </summary>
        public string ExpectedHoldingPeriodStartData { get; init; }
        /// <summary> hdexpd_edd, 보유예상기간(종료일) </summary>
        public string ExpectedHoldingPeriodEndData { get; init; }
        /// <summary> od_a_at_t, 사외이사참석여부(참석(명)) </summary>
        public string OutsideDirectorAttendenceCount { get; init; }
        /// <summary> od_a_at_b, 사외이사참석여부(참석(명)) </summary>
        public string OutsideDirectorAbsentCount { get; init; }
        
        public bool IsOrdinaryStock => AcquisitionNumberOfOrdinaryStock != "-" || AcquisitionPriceOfOrdinaryStock != "-";
        public bool HasAcquisitionPriceData => AcquisitionPriceOfOrdinaryStock != "-" || AcquisitionPriceOfExtraordinaryStock != "-";
        /// <summary> 공시 정정으로 ReceiptNumber 변경 여부</summary>
        public bool Corrected { get; set; } = false;
        public TreasuryDetailReport() { }

        public TreasuryDetailReport(JsonElement detailJson)
        {

            ReceiptNumber = detailJson.GetProperty("rcept_no").GetString() ?? string.Empty;
            AuditAttendance = detailJson.GetProperty("adt_a_atn").GetString() ?? string.Empty;
            AcquisitionDate = detailJson.GetProperty("aq_dd").GetString() ?? string.Empty;
            AcquisitionMethod = detailJson.GetProperty("aq_mth").GetString() ?? string.Empty;
            AcquisitionPurpose = detailJson.GetProperty("aq_pp").GetString() ?? string.Empty;
            AcquisitionWithinDevidendOrdinaryStock = detailJson.GetProperty("aq_wtn_div_ostk").GetString() ?? string.Empty;
            AcquisitionWithinDevidendOrdinaryStockRate = detailJson.GetProperty("aq_wtn_div_ostk_rt").GetString() ?? string.Empty;
            AcquisitionWithinDevidendExtraordinaryStock = detailJson.GetProperty("aq_wtn_div_estk").GetString() ?? string.Empty;
            AcquisitionWithinDevidendExtraordinaryStockRate = detailJson.GetProperty("aq_wtn_div_estk_rt").GetString() ?? string.Empty;
            ExpectedAcquisitionStartDate = detailJson.GetProperty("aqexpd_bgd").GetString() ?? string.Empty;
            ExpectedAcquisitionEndDate = detailJson.GetProperty("aqexpd_edd").GetString() ?? string.Empty;
            AcquisitionPriceOfOrdinaryStock = detailJson.GetProperty("aqpln_prc_ostk").GetString() ?? string.Empty;
            AcquisitionPriceOfExtraordinaryStock = detailJson.GetProperty("aqpln_prc_estk").GetString() ?? string.Empty;
            AcquisitionNumberOfOrdinaryStock = detailJson.GetProperty("aqpln_stk_ostk").GetString() ?? string.Empty;
            AcquisitionNumberOfExtraordinaryStock = detailJson.GetProperty("aqpln_stk_estk").GetString() ?? string.Empty;
            CorpClass = (CorpClassType)Enum.Parse(typeof(CorpClassType), detailJson.GetProperty("corp_cls").GetString() ?? string.Empty);
            CorpCode = detailJson.GetProperty("corp_code").GetString() ?? string.Empty;
            CorpName = detailJson.GetProperty("corp_name").GetString() ?? string.Empty;
            ConsignmentInvenstmentBrokerage = detailJson.GetProperty("cs_iv_bk").GetString() ?? string.Empty;
            PurchaseLimitPerDayOfOrdinaryStock = detailJson.GetProperty("d1_prodlm_ostk").GetString() ?? string.Empty;
            PurchaseLimitPerDayOfExtraordinaryStock = detailJson.GetProperty("d1_prodlm_estk").GetString() ?? string.Empty;
            ExtraAcquisitionOrdinaryStock = detailJson.GetProperty("eaq_ostk").GetString() ?? string.Empty;
            ExtraAcquisitionOrdinaryStockRate = detailJson.GetProperty("eaq_ostk_rt").GetString() ?? string.Empty;
            ExtraAcquisitionExtraordinaryStock = detailJson.GetProperty("eaq_estk").GetString() ?? string.Empty;
            ExtraAcquisitionExtraordinaryStockRate = detailJson.GetProperty("eaq_estk_rt").GetString() ?? string.Empty;
            ExpectedHoldingPeriodStartData = detailJson.GetProperty("hdexpd_bgd").GetString() ?? string.Empty;
            ExpectedHoldingPeriodEndData = detailJson.GetProperty("hdexpd_edd").GetString() ?? string.Empty;
            OutsideDirectorAttendenceCount = detailJson.GetProperty("od_a_at_t").GetString() ?? string.Empty;
            OutsideDirectorAbsentCount = detailJson.GetProperty("od_a_at_b").GetString() ?? string.Empty;
        }

        public string ExpectedAcquisitionMoney()
        {
            if (!HasAcquisitionPriceData)
            {
                return "-";
            }

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
