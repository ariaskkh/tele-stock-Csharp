using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using TelegramBot;

enum BusinessReportType
{
    FIRST_QUARTER = 11013, // 분기보고서
    SECOND_QUARTER = 11012, // 반기보고서
    THIRD_QUARTER = 11014, // 분기보고서
    FOURTH_QUARTER = 11011, // 사업보고서
}

namespace TelegramBot.Services
{
    public class TreasuryStockService
    {
        static readonly HttpClient _httpClient = new HttpClient();
        readonly ILogger _logger;
        //private string FILE_PATH = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../TelegramBot/Data/"));

        private List<JsonElement> _majorInfoDataList = new();
        private List<JsonElement> _detailDataList = new();
        private List<JsonElement> _minorityShareholderDataList = new();
        private Dictionary<string, TreasuryStock> _treasuryStockDict = new();

        public TreasuryStockService(ILogger logger)
        {
            this._logger = logger;
            SetHttpClient();
        }

        static void SetHttpClient()
        {
            // server가 get 요청 client의 브라우저 정보로 User-Agent 사용
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Chrome/58.0.3029.110");
        }

        public async Task UpdateDataAsync()
        {
            _majorInfoDataList = await GetMajorInfoDataOfAllCorpsAsync();
            _detailDataList = await GetTreasuryDetailDataAsync(_majorInfoDataList);
            _minorityShareholderDataList = await GetMinorityShareholderStatusDataAsync(_majorInfoDataList);
            _treasuryStockDict = MergeData(_majorInfoDataList, _detailDataList, _minorityShareholderDataList);
        }

        async Task<List<JsonElement>> GetMajorInfoDataOfAllCorpsAsync()
        {
            var _pageNumber = 1; // 페이지 번호
            var _pageCount = 2; // 페이지 별 건수
            var startDate = "20240510"; // TEST
            var endDate = GetTodayDate();
            var majorInfoReport = "B001";
            var baseUrl = "http://opendart.fss.or.kr/api/list.json";
            var parameters = new Dictionary<string, string>
            {
                ["crtfc_key"] = PrivateData.DART_API_KEY,
                ["page_count"] = _pageCount.ToString(),
                ["bgn_de"] = startDate,
                ["end_de"] = endDate,
                ["pblntf_detail_ty"] = majorInfoReport,
            };

            var majorInfoResult = new List<JsonElement>();

            while (true)
            {
                try
                {
                    parameters["page_no"] = _pageNumber.ToString();
                    var queryString = GetQueryString(parameters);
                    var url = $"{baseUrl}?{queryString}";
                    // HttpClientMessage에 dispose()가 있으므로 resouce release를 위해 using 사용
                    using (var response = await _httpClient.GetAsync(url))
                    {
                        if (HttpStatusCode.OK == response.StatusCode)
                        {
                            string body = await response.Content.ReadAsStringAsync();
                            using (var doc = JsonDocument.Parse(body))
                            {
                                JsonElement overviewData = doc.RootElement.GetProperty("list");
                                var filteredOverviewData = FilterMajorInfoData(overviewData);

                                foreach (var overviewObj in filteredOverviewData)
                                {
                                    majorInfoResult.Add(overviewObj.Clone()); // using 밖에서 dispose 되기 때문에 clone() 필요
                                }

                                var totalPage = doc.RootElement.GetProperty("total_page").GetInt32();
                                if (_pageNumber == totalPage)
                                    break;
                                _pageNumber++;
                            }
                        }
                        else
                        {
                            _logger.Log($"Invalid response  - OverviewData : {response.ReasonPhrase}");
                            return default;
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.Log($"HttpRequestException - OverviewData: {ex.Message}");
                    return default;
                }
                catch (Exception ex)
                {
                    _logger.Log($"Exception - OverviewData: {ex.Message}");
                    return default;
                }
            }
            
            // TODO: cache check logic
            return majorInfoResult;
        }

        private List<JsonElement> FilterMajorInfoData(JsonElement OverviewArray)
        {
            var keyword = "주요사항보고서(자기주식취득결정)"; // TODO: 처분, 신탁도 케이스 대응하기
            var KOSPI = "Y";
            var KOSDAQ = "K";
            List<JsonElement> overviewArray = new();

            foreach (var overviewObject in OverviewArray.EnumerateArray())
            {
                string? reportNm = overviewObject.GetProperty("report_nm").GetString();
                string? corpCls = overviewObject.GetProperty("corp_cls").GetString();

                if (reportNm == keyword && (corpCls == KOSPI || corpCls == KOSDAQ))
                {
                    overviewArray.Add(overviewObject);
                }
            }
            return overviewArray;
        }

        // TODO: caching
        //private async void SaveOverviewJson(List<JsonElement> result)
        //{
        //    string finalJson = JsonSerializer.Serialize(result);
        //    await File.WriteAllTextAsync(Path.Combine(FILE_PATH, "list.json"), finalJson);
        //}

        private async Task<List<JsonElement>> GetTreasuryDetailDataAsync(List<JsonElement> overviewData)
        {
            if (_majorInfoDataList.Count == 0)
            {
                _logger.Log("자기주식 관련 overview 데이터가 존재하지 않습니다.");
                return default;
            }

            var companyList = new List<string>();
            var detailResult = new List<JsonElement>();

            foreach (var overviewObj in overviewData)
            {
                var corpCode = overviewObj.GetProperty("corp_code").ToString();
                var receptData = overviewObj.GetProperty("rcept_dt").ToString();

                if (companyList.Contains(corpCode))
                    continue;
                companyList.Add(corpCode);

                var baseUrl = "https://opendart.fss.or.kr/api/tsstkAqDecsn.json";
                var parameters = new Dictionary<string, string>
                {
                    ["crtfc_key"] = PrivateData.DART_API_KEY,
                    ["corp_code"] = corpCode,
                    ["bgn_de"] = receptData,
                    ["end_de"] = receptData,
                };

                var queryString = GetQueryString(parameters);
                var url = $"{baseUrl}?{queryString}";

                try
                {
                    using (var response = await _httpClient.GetAsync(url))
                    {
                        if (HttpStatusCode.OK == response.StatusCode)
                        {
                            string body = await response.Content.ReadAsStringAsync();
                            using (var doc = JsonDocument.Parse(body))
                            {
                                // RootElement가 뭔지 확인해보기
                                JsonElement detailData = doc.RootElement.GetProperty("list");
                                detailResult.Add(detailData[0].Clone());
                            }
                        }
                        else
                        {
                            _logger.Log($"Invalid response - detailData: {response.ReasonPhrase}");
                            return default;
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.Log($"HttpRequestException - detailData: {ex.Message}");
                    return default;
                }
                catch (Exception ex)
                {
                    _logger.Log($"Exception - detailData: {ex.Message}");
                    return default;
                }
            }

            return detailResult;
        }

        private static string GetTodayDate()
        {
            var dateForm = "yyyyMMdd";
            var today = DateTime.Now; // ex. 2024-05-18 오후 8:11:16
            if (today.DayOfWeek == DayOfWeek.Saturday)
                return today.AddDays(-1).ToString(dateForm);
            else if (today.DayOfWeek == DayOfWeek.Sunday)
                return today.AddDays(-1).ToString(dateForm);
            else
                return today.ToString(dateForm);
        }

        private async Task<List<JsonElement>> GetMinorityShareholderStatusDataAsync(List<JsonElement> overviewData)
        {
            List<JsonElement> floatingDataResult = new();
            List<BusinessReportType> latestReportCodeList = GetLatestReportCode();
            foreach (var overviewJson in overviewData)
            {
                var baseUrl = "https://opendart.fss.or.kr/api/mrhlSttus.json";
                var parameters = new Dictionary<string, string>
                {
                    ["crtfc_key"] = PrivateData.DART_API_KEY,
                    ["corp_code"] = overviewJson.GetProperty("corp_code").ToString(),
                    ["bsns_year"] = GetLatestReportBusinessYear(latestReportCodeList[0]), // 사업 년도
                    ["reprt_code"] = ((int)latestReportCodeList[0]).ToString(), // 보고서 코드
                };

                bool isFetchSuccess = await TryFetchMinorityShareholderData(baseUrl, parameters, floatingDataResult);

                if (!isFetchSuccess)
                {
                    parameters["bsns_year"] = GetLatestReportBusinessYear(latestReportCodeList[1]);
                    parameters["reprt_code"] = ((int)latestReportCodeList[1]).ToString();

                    isFetchSuccess = await TryFetchMinorityShareholderData(baseUrl, parameters, floatingDataResult);

                    if (!isFetchSuccess)
                    {
                        _logger.Log($"No floating data fetch for corp_code: {parameters["corp_code"]}");
                    }
                }
            }
            /*
             - corp_code: 고유번호
             - corp_name: 법인명
             - se: 구분 (ex. 소액주주)
             - hold_stock_co: 보유 주식 수
             - stock_tot_co: 총발행 주식수
             - hold_stock_rate: 보유 주식 비율
            */
            return floatingDataResult;
        }

        private async Task<bool> TryFetchMinorityShareholderData(string url, Dictionary<string, string> parameters, List<JsonElement> result)
        {
            var queryString = GetQueryString(parameters);
            var requestUrl = $"{url}?{queryString}";

            try
            {
                using (var response = await _httpClient.GetAsync(requestUrl))
                {
                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        string body = await response.Content.ReadAsStringAsync();
                        using (var doc = JsonDocument.Parse(body))
                        {
                            // TODO. JsonElement말고 JObject로 했을 때 이스케이프 문자열로 바뀌는지 확인
                            if (doc.RootElement.TryGetProperty("list", out JsonElement minorityShareholderData)
                                && minorityShareholderData.GetArrayLength() > 0
                                && minorityShareholderData[0].TryGetProperty("hold_stock_rate", out var holdStockRate)
                                && holdStockRate.ToString() != "-"
                                )
                            {
                                result.Add(minorityShareholderData[0].Clone());
                                return true;
                            }
                        }
                    }
                    else
                    {
                        _logger.Log($"Invalid response - floatingStockRateData: {response.ReasonPhrase}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.Log($"HttpRequestException - floatingStockRateData: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Log($"Exception - floatingStockRateData: {ex.Message}");
            }
            return false;
        }

        // TODO: 장전 모든 회사들에 대한 최신 보고서 종류 및 년도 업데이트 필요
        // 정기 보고서의 종류를 결정하기 위한 함수
        private List<BusinessReportType> GetLatestReportCode(int settlementMonth = 12)
        {
            int reportTypeNumer = Enum.GetValues(typeof(BusinessReportType)).Length;
            const int TotalMonth = 12;
            const int baseSettlementMonth = 12;
            var interval = TotalMonth / reportTypeNumer;
            var today = DateTime.Today.Month;
            List<BusinessReportType> reportCodeList = new();
            if (settlementMonth == baseSettlementMonth)
            {
                if (today <= interval * 1) // 1분기
                {
                    reportCodeList.Add(BusinessReportType.FOURTH_QUARTER);
                    reportCodeList.Add(BusinessReportType.THIRD_QUARTER);
                }
                else if (today <= interval * 2) // 2분기
                {
                    reportCodeList.Add(BusinessReportType.FIRST_QUARTER);
                    reportCodeList.Add(BusinessReportType.FOURTH_QUARTER);
                }
                else if (today <= interval * 3) // 3분기
                {
                    reportCodeList.Add(BusinessReportType.SECOND_QUARTER);
                    reportCodeList.Add(BusinessReportType.FIRST_QUARTER);
                }
                else // 4분기
                {
                    reportCodeList.Add(BusinessReportType.THIRD_QUARTER);
                    reportCodeList.Add(BusinessReportType.SECOND_QUARTER);
                }
            }
            else
            {
                // TODO: 결산월이 12월이 아닌 경우에 대한 예외처리
            }
            return reportCodeList;
        }

        private string GetLatestReportBusinessYear(BusinessReportType reportCode)
        {
            var year = DateTime.Now.Year;
            if (reportCode == BusinessReportType.FOURTH_QUARTER)
            {
                return (year - 1).ToString();
            }
            return year.ToString();
        }

        private string GetQueryString(Dictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(KeyValue => $"{Uri.EscapeDataString(KeyValue.Key)}={Uri.EscapeDataString(KeyValue.Value)}"));
        }

        /* 필요한 데이터
         * corp_name, stock_code, report_nm, aqpln_stk_ostk/aqpln_stk_estk, acquisition_stock_floating_rate, aq_mth, aq_pp, aqexpd_bgd, aqexpd_edd, report_number
         */
        // 처음부터 DIctionary로 저장했어야 했나 고민
        private Dictionary<string, TreasuryStock> MergeData(List<JsonElement> majorInfoDataList, List<JsonElement> detailDataList, List<JsonElement> minorityShareholderDataList)
        {
            var treasuryStockDict = new Dictionary<string, TreasuryStock>();

            foreach (JsonElement majorInfoData in majorInfoDataList)
            {
                var reciptNumber = majorInfoData.GetProperty("rcept_no").GetString() ?? string.Empty; // unique key
                var corpNameOfMajorInfo = majorInfoData.GetProperty("corp_name").GetString() ?? string.Empty;

                var treasurystock = new TreasuryStock
                {
                    CorpName = corpNameOfMajorInfo,
                    StockCode = majorInfoData.GetProperty("stock_code").GetString() ?? string.Empty,
                    ReportName = majorInfoData.GetProperty("report_nm").GetString() ?? string.Empty,
                    ReceiptNumber = reciptNumber,
                    
                };
                treasuryStockDict[reciptNumber] = treasurystock;
            }

            foreach (JsonElement item in detailDataList)
            {
                var reciptNumber = item.GetProperty("rcept_no").GetString() ?? string.Empty;
                var corpNameOfDetailData = item.GetProperty("corp_name").GetString() ?? string.Empty;

                var oridinaryStockAcqusitionNumber = item.GetProperty("aqpln_stk_ostk").ToString() ?? string.Empty;
                var extraoridinaryStockAcqusitionNumber = item.GetProperty("aqpln_stk_estk").ToString() ?? string.Empty;

                var acquisitionStockNumber = oridinaryStockAcqusitionNumber == "-" ? extraoridinaryStockAcqusitionNumber : oridinaryStockAcqusitionNumber ;
                acquisitionStockNumber = acquisitionStockNumber.Replace(",", "");
                var minorityShareholderData = minorityShareholderDataList.FirstOrDefault(minorityShareholderData => minorityShareholderData.GetProperty("corp_name").ToString() == corpNameOfDetailData);


                var floatingStockNumber = minorityShareholderData.GetProperty("hold_stock_co")
                        .GetString()
                        .Replace(",", "")
                        ?? string.Empty;
                var acquisitionRateOfFloatingStock = Math.Round((double.Parse(acquisitionStockNumber) / double.Parse(floatingStockNumber) * 100), 2).ToString();


                if (treasuryStockDict.TryGetValue(reciptNumber, out TreasuryStock treasuryStock))
                {
                    treasuryStock.AcquisitionMethod = item.GetProperty("aq_mth").ToString() ?? string.Empty;
                    treasuryStock.AcquisitionPurpose = item.GetProperty("aq_pp").ToString() ?? string.Empty;
                    treasuryStock.AcquisitionStartDate = item.GetProperty("aqexpd_bgd").ToString() ?? string.Empty;
                    treasuryStock.AcquisitionEndDate = item.GetProperty("aqexpd_edd").ToString() ?? string.Empty;
                    treasuryStock.AcquisitionRateOfFloatingStock = acquisitionRateOfFloatingStock;
                }
            }
            return treasuryStockDict;
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
        public List<string> GetMessages()
        {
            _logger.Log("메세지 보내기 성공!");
            return new List<string> { "이거시 바로", "메세지다" };
        }
    }
}
