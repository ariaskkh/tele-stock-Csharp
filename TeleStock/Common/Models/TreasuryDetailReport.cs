﻿using Newtonsoft.Json.Linq;

namespace Common.Models
{
    public class TreasuryDetailReport 
    {
        /// <summary> rcept_no, 접수번호 </summary>
        public string ReceiptNumber { get; init; }
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
        public bool HasAcquisitionPriceData => AcquisitionPriceOfOrdinaryStock != "-" && AcquisitionPriceOfExtraordinaryStock != "-";
        public TreasuryDetailReport() { }

        public TreasuryDetailReport(JToken detailJson)
        {

            ReceiptNumber = detailJson["rcept_no"].ToString();
            AuditAttendance = detailJson["adt_a_atn"].ToString();
            AcquisitionDate = detailJson["aq_dd"].ToString();
            AcquisitionMethod = detailJson["aq_mth"].ToString();
            AcquisitionPurpose = detailJson["aq_pp"].ToString();
            AcquisitionWithinDevidendOrdinaryStock = detailJson["aq_wtn_div_ostk"].ToString();
            AcquisitionWithinDevidendOrdinaryStockRate = detailJson["aq_wtn_div_ostk_rt"].ToString();
            AcquisitionWithinDevidendExtraordinaryStock = detailJson["aq_wtn_div_estk"].ToString();
            AcquisitionWithinDevidendExtraordinaryStockRate = detailJson["aq_wtn_div_estk_rt"].ToString();
            ExpectedAcquisitionStartDate = detailJson["aqexpd_bgd"].ToString();
            ExpectedAcquisitionEndDate = detailJson["aqexpd_edd"].ToString();
            AcquisitionPriceOfOrdinaryStock = detailJson["aqpln_prc_ostk"].ToString();
            AcquisitionPriceOfExtraordinaryStock = detailJson["aqpln_prc_estk"].ToString();
            AcquisitionNumberOfOrdinaryStock = detailJson["aqpln_stk_ostk"].ToString();
            AcquisitionNumberOfExtraordinaryStock = detailJson["aqpln_stk_estk"].ToString();
            CorpClass = (CorpClassType)Enum.Parse(typeof(CorpClassType), detailJson["corp_cls"].ToString());
            CorpCode = detailJson["corp_code"].ToString();
            CorpName = detailJson["corp_name"].ToString();
            ConsignmentInvenstmentBrokerage = detailJson["cs_iv_bk"].ToString();
            PurchaseLimitPerDayOfOrdinaryStock = detailJson["d1_prodlm_ostk"].ToString();
            PurchaseLimitPerDayOfExtraordinaryStock = detailJson["d1_prodlm_estk"].ToString();
            ExtraAcquisitionOrdinaryStock = detailJson["eaq_ostk"].ToString();
            ExtraAcquisitionOrdinaryStockRate = detailJson["eaq_ostk_rt"].ToString();
            ExtraAcquisitionExtraordinaryStock = detailJson["eaq_estk"].ToString();
            ExtraAcquisitionExtraordinaryStockRate = detailJson["eaq_estk_rt"].ToString();
            ExpectedHoldingPeriodStartData = detailJson["hdexpd_bgd"].ToString();
            ExpectedHoldingPeriodEndData = detailJson["hdexpd_edd"].ToString();
            OutsideDirectorAttendenceCount = detailJson["od_a_at_t"].ToString();
            OutsideDirectorAbsentCount = detailJson["od_a_at_b"].ToString();
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
