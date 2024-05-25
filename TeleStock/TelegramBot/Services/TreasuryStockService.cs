using Common.Interfaces;
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

        private List<JsonElement> _overviewData = new();
        private List<JsonElement> _detailData = new();
        private List<JsonElement> _floatingStockRate = new();

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

        public async Task GetData()
        {
            _overviewData = await GetOverviewData();
            _detailData = await GetDetailData(_overviewData);
            _floatingStockRate = await GetFloatingStockRateData(_overviewData);
        }

        async Task<List<JsonElement>> GetOverviewData()
        {
            var _pageNumber = 1; // 페이지 번호
            var _pageCount = 2; // 페이지 별 건수
            var startDate = "20240510"; // TEST
            var endDate = GetTodayDate();
            var majorInfoReport = "B001";
            var majorInfoReportUrl = "http://opendart.fss.or.kr/api/list.json";
            var parameters = new Dictionary<string, string>
            {
                ["crtfc_key"] = PrivateData.DART_API_KEY,
                ["page_count"] = _pageCount.ToString(),
                ["bgn_de"] = startDate,
                ["end_de"] = endDate,
                ["pblntf_detail_ty"] = majorInfoReport,
            };

            var overviewResult = new List<JsonElement>();

            while (true)
            {
                try
                {
                    parameters["page_no"] = _pageNumber.ToString();
                    var queryString = GetQueryString(parameters);
                    var url = $"{majorInfoReportUrl}?{queryString}";
                    // HttpClientMessage에 dispose()가 있으므로 resouce release를 위해 using 사용
                    using (var response = await _httpClient.GetAsync(url))
                    {
                        if (HttpStatusCode.OK == response.StatusCode)
                        {
                            string body = await response.Content.ReadAsStringAsync();
                            using (var doc = JsonDocument.Parse(body))
                            {
                                JsonElement overviewData = doc.RootElement.GetProperty("list");
                                var filteredOverviewData = FilterOverviewData(overviewData);

                                foreach (var overviewObj in filteredOverviewData)
                                {
                                    overviewResult.Add(overviewObj.Clone()); // using 밖에서 dispose 되기 때문에 clone() 필요
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
            return overviewResult;
        }

        private List<JsonElement> FilterOverviewData(JsonElement OverviewArray)
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

        private async Task<List<JsonElement>> GetDetailData(List<JsonElement> overviewData)
        {
            if (_overviewData.Count == 0)
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

                var treasuryStockUrl = "https://opendart.fss.or.kr/api/tsstkAqDecsn.json";
                var parameters = new Dictionary<string, string>
                {
                    ["crtfc_key"] = PrivateData.DART_API_KEY,
                    ["corp_code"] = corpCode,
                    ["bgn_de"] = receptData,
                    ["end_de"] = receptData,
                };

                var queryString = GetQueryString(parameters);
                var url = $"{treasuryStockUrl}?{queryString}";

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
                                detailResult.Add(detailData.Clone());
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

        private async Task<List<JsonElement>> GetFloatingStockRateData(List<JsonElement> overviewData)
        {
            List<JsonElement> floatingDataResult = new();
            List<BusinessReportType> latestReportCodeList = GetLatestReportCode();
            foreach (var overviewJson in overviewData)
            {
                var floatingStockUrl = "https://opendart.fss.or.kr/api/mrhlSttus.json";
                var parameters = new Dictionary<string, string>
                {
                    ["crtfc_key"] = PrivateData.DART_API_KEY,
                    ["corp_code"] = overviewJson.GetProperty("corp_code").ToString(),
                    ["bsns_year"] = GetLatestReportBusinessYear(latestReportCodeList[0]), // 사업 년도
                    ["reprt_code"] = ((int)latestReportCodeList[0]).ToString(), // 보고서 코드
                };

                bool isFetchSuccess = await TryFetchFloatingStockRateData(floatingStockUrl, parameters, floatingDataResult);

                if (!isFetchSuccess)
                {
                    parameters["bsns_year"] = GetLatestReportBusinessYear(latestReportCodeList[1]);
                    parameters["reprt_code"] = ((int)latestReportCodeList[1]).ToString();

                    isFetchSuccess = await TryFetchFloatingStockRateData(floatingStockUrl, parameters, floatingDataResult);

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

        private async Task<bool> TryFetchFloatingStockRateData(string url, Dictionary<string, string> parameters, List<JsonElement> result)
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
                            if (doc.RootElement.TryGetProperty("list", out JsonElement floatingData) && floatingData.GetArrayLength() > 0);
                            {
                                result.Add(floatingData.Clone());
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

        public List<string> GetMessages()
        {
            _logger.Log("메세지 보내기 성공!");
            return new List<string> { "이거시 바로", "메세지다" };
        }
    }
}
