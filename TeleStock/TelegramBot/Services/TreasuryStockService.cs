using Common.Interfaces;
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
            _overviewData = await GetStockOverview();
            if (_overviewData.Count == 0)
            {
                _logger.Log("해당 조건의 overview 데이터가 존재하지 않습니다.");
                return;
            }
        }

        async Task<List<JsonElement>> GetStockOverview()
        {
            var _pageNumber = 1; // 페이지 번호
            var _pageCount = 2; // 페이지 별 건수
            var startDate = "20240517"; // TEST
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

            var resultAll = new List<JsonElement>();


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
                            using (JsonDocument doc = JsonDocument.Parse(body))
                            {
                                JsonElement overviewData = doc.RootElement.GetProperty("list");
                                var filteredOverviewData = FilterOverviewData(overviewData);

                                foreach (var item in filteredOverviewData)
                                {
                                    resultAll.Add(item.Clone()); // using 밖에서 dispose 되기 때문에 clone() 필요
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
                    _logger.Log($"HttpRequestException Message: {ex.Message}");
                    return default;
                }
                catch (Exception ex)
                {
                    _logger.Log($"Exception: {ex.Message}");
                    return default;
                }
            }
            
            // TODO: cache check logic
            return resultAll;
        }

        private List<JsonElement> FilterOverviewData(JsonElement OverviewArray)
        {
            var keyword = "자기주식";
            var KOSPI = "Y";
            var KOSDAQ = "K";
            List<JsonElement> overviewArray = new();

            foreach (var overviewObject in OverviewArray.EnumerateArray())
            {
                string? reportNm = overviewObject.GetProperty("report_nm").GetString();
                string? corpCls = overviewObject.GetProperty("corp_cls").GetString();

                if (reportNm.Contains(keyword) && (corpCls == KOSPI || corpCls == KOSDAQ))
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
