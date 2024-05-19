using Common.Interfaces;
using System.Net;
using System.Text;
using TelegramBot;


namespace TelegramBot.Services
{
    public class TreasuryStockService
    {
        static readonly HttpClient _httpClient = new HttpClient();
        readonly ILogger _logger;
        public TreasuryStockService(ILogger logger)
        {
            this._logger = logger;
            SetHttpClient();
            GetData();
        }

        static void SetHttpClient()
        {
            // server가 get 요청 client의 브라우저 정보로 User-Agent 사용
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Chrome/58.0.3029.110");
        }

        void GetData()
        {
            GetStockOverview();
        }

        async void GetStockOverview()
        {
            var _pageNumber = "1"; // 페이지 번호
            var _pageCount = "100"; // 페이지 별 건수
            var startDate = "20240514"; // 20240517
            var endDate = GetTodayDate();
            var majorInfoReport = "B001";
            var majorInfoReportUrl = "http://opendart.fss.or.kr/api/list.json";
            var parameters = new Dictionary<string, string>
            {
                ["crtfc_key"] = PrivateData.DART_API_KEY,
                ["page_no"] = _pageNumber,
                ["page_count"] = _pageCount,
                ["bgn_de"] = startDate,
                ["end_de"] = endDate,
                ["pblntf_detail_ty"] = majorInfoReport,
            };
            var queryString = string.Join("&", parameters.Select(keyValue => $"{Uri.EscapeDataString(keyValue.Key)}={Uri.EscapeDataString(keyValue.Value)}"));
            var url = $"{majorInfoReportUrl}?{queryString}";

            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    _logger.Log(HttpStatusCode.OK.ToString());
                    _logger.Log(response.StatusCode.ToString());

                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        string body = await response.Content.ReadAsStringAsync();
                        _logger.Log($"body: {body}");
                    }
                    else
                    {
                        _logger.Log($"reason: {response.ReasonPhrase}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.Log($"HttpRequestException Message: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Log($"Exception: {ex.Message}");
            }
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
