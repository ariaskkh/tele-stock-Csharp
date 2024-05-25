using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using TelegramBot;


namespace TelegramBot.Services
{
    public class TreasuryStockService
    {
        static readonly HttpClient _httpClient = new HttpClient();
        readonly ILogger _logger;
        //private string FILE_PATH = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../TelegramBot/Data/"));

        private List<JsonElement> _overviewData = new();
        private List<JsonElement> _detailData = new();

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
                    var queryString = string.Join("&", parameters.Select(keyValue => $"{Uri.EscapeDataString(keyValue.Key)}={Uri.EscapeDataString(keyValue.Value)}"));
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
                            _logger.Log($"reason: {response.ReasonPhrase}");
                            return default;
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.Log($"HttpRequestException OverviewData: {ex.Message}");
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

                var queryString = string.Join("&", parameters.Select(KeyValue => $"{Uri.EscapeDataString(KeyValue.Key)}={Uri.EscapeDataString(KeyValue.Value)}"));
                var url = $"{treasuryStockUrl}?{queryString}";

                try
                {
                    using ( var response = await _httpClient.GetAsync(url) )
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

        private string GetTodayDate()
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

        public List<string> GetMessages()
        {
            _logger.Log("메세지 보내기 성공!");
            return new List<string> { "이거시 바로", "메세지다" };
        }
    }
}
