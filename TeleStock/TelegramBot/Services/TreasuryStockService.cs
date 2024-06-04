using Common.Extensions;
using Common.Interfaces;
using Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

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
        private string FILE_PATH = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../TelegramBot/Data/"));

        private List<MajorInfoReport> _majorInfoReportList = new();
        private List<TreasuryDetailReport> _detailReportList = new();
        private List<MinorityShareholderStatusReport> _minorityShareholderDataList = new();
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
            _majorInfoReportList = await GetMajorInfoReportList();
            _detailReportList = await GetTreasuryDetailReport(_majorInfoReportList);
            _minorityShareholderDataList = await GetMinorityShareholderStatusData(_detailReportList);
            _treasuryStockDict = MergeData(_majorInfoReportList, _detailReportList, _minorityShareholderDataList);
            SaveOverviewJson(_treasuryStockDict, FILE_PATH);
        }

        async Task<List<MajorInfoReport>> GetMajorInfoReportList()
        {   
            List<MajorInfoReport> majorInfoReportList = await GetMajorInfoReportListAsync();
            // TODO: cache check logic
            return FilterMajorInfoReport(majorInfoReportList);
        }

        private async Task<List<MajorInfoReport>?> GetMajorInfoReportListAsync()
        {
            var _pageNumber = 0; // 페이지 번호
            var _pageCount = 100; // 페이지 별 건수
            var startDate = "20240529"; // TEST
            var endDate = GetTodayDate();
            var majorInfoReportNumber = "B001";
            var baseUrl = "http://opendart.fss.or.kr/api/list.json";
            var parameters = new Dictionary<string, string>
            {
                ["crtfc_key"] = PrivateData.DART_API_KEY,
                ["page_count"] = _pageCount.ToString(),
                ["bgn_de"] = startDate,
                ["end_de"] = endDate,
                ["pblntf_detail_ty"] = majorInfoReportNumber,
            };
            var majorInfoReportList = new List<MajorInfoReport>();
            bool stopLoop = false;
            while (!stopLoop)
            {
                parameters["page_no"] = (++_pageNumber).ToString();
                try
                {
                    var result = await FetchMajorInfoReportAsync(baseUrl.GetUrlWithQuery(parameters), _pageNumber);
                    if (result.majorInfoReportList != null)
                    {
                        majorInfoReportList.AddRange(result.majorInfoReportList);
                    }
                    if (result.stopLoop)
                    {
                        stopLoop = true;
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
            return majorInfoReportList;
        }

        private async Task<(List<MajorInfoReport> majorInfoReportList, bool stopLoop)> FetchMajorInfoReportAsync(string url, int _pageNumber)
        {
            List<MajorInfoReport> majorInfoReportList = new();
            bool stopLoop = false;
            using (var response = await _httpClient.GetAsync(url))
            {
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(body);
                    if (json.TryGetValue("list", out JToken majorInfoJson))
                    {
                        majorInfoReportList = majorInfoJson.Select(majorInfo => new MajorInfoReport(majorInfo)).ToList();
                    }
                    var totalPage = json["total_page"].ToObject<int>();
                    if (_pageNumber == totalPage)
                        stopLoop = true;
                    return (majorInfoReportList, stopLoop);
                }
                else
                {
                    _logger.Log($"Invalid response  - OverviewData : {response.ReasonPhrase}");
                    return (default, stopLoop);
                }
            }
        }

        private static List<MajorInfoReport> FilterMajorInfoReport(List<MajorInfoReport> majorInfoReportList)
        {
            return majorInfoReportList.Where(majorInfo => IsTreasuryStockReport(majorInfo)).ToList();

            bool IsTreasuryStockReport(MajorInfoReport majorInfo)
            {
                var keyword = "주요사항보고서(자기주식취득결정)"; // TODO: 처분, 신탁도 케이스 대응하기
                var KOSPI = CorpClassType.Y;
                var KOSDAQ = CorpClassType.K;
                return (majorInfo.ReportName == keyword) && (majorInfo.CorpClass == KOSPI || majorInfo.CorpClass == KOSDAQ);
            }
        }

        // TODO: caching
        //private static async void SaveOverviewJson(Dictionary<string, TreasuryStock> treasuryStockDick, string filePath)
        //{
        //    string json = JsonConvert.SerializeObject(treasuryStockDick);
        //    await File.WriteAllTextAsync(Path.Combine(filePath, "list.json"), json);
        //}

        private async Task<List<TreasuryDetailReport>> GetTreasuryDetailReport(List<MajorInfoReport> MajorInfoReportList)
        {
            if (_majorInfoReportList is not null && _majorInfoReportList.Count == 0)
            {
                _logger.Log("자기주식 관련 overview 데이터가 존재하지 않습니다.");
                return default;
            }

            var companyList = new List<string>();
            var detailReportList = new List<TreasuryDetailReport>();

            foreach (MajorInfoReport MajorInfoReport in MajorInfoReportList)
            {
                var corpCode = MajorInfoReport.CorpCode;
                var receptData = MajorInfoReport.ReceiptDate;

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
                try
                {
                    var result  = await FetchTreasuryDetailReportAsync(baseUrl.GetUrlWithQuery(parameters));
                    if (result != null)
                    {
                        detailReportList.AddRange(result);
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

            return detailReportList;
        }

        private async Task<List<TreasuryDetailReport>> FetchTreasuryDetailReportAsync(string url)
        {
            List<TreasuryDetailReport> detailReportList = new();
            using (var response = await _httpClient.GetAsync(url))
            {
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(body);

                    if (json.TryGetValue("list", out JToken detailReportsJson) && detailReportsJson.Any())
                    {
                        detailReportList.Add(new TreasuryDetailReport(detailReportsJson.FirstOrDefault()));
                    }
                }
                else
                {
                    _logger.Log($"Invalid response - detailData: {response.ReasonPhrase}");
                    return default;
                }
            }
            return detailReportList;
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

        private async Task<List<MinorityShareholderStatusReport>> GetMinorityShareholderStatusData(List<TreasuryDetailReport> detailReportList)
        {
            List<MinorityShareholderStatusReport> minorityShareholderReportList = new();
            List<BusinessReportType> latestReportCodeList = GetLatestReportCode();

            foreach (var detailReport in detailReportList)
            {
                var result = await GetMinorityShareholderDataFromLatestReport(latestReportCodeList, detailReport);
                if (result.isFetchSuccess && result.minorityShareholderReport!= null)
                {
                    minorityShareholderReportList.Add(result.minorityShareholderReport);
                }
                else
                {
                    _logger.Log($"No minorityShareholderReport for corp_code: {detailReport.CorpCode}");
                }
            }
            return minorityShareholderReportList;
        }

        private async Task<(bool isFetchSuccess, MinorityShareholderStatusReport minorityShareholderReport)> GetMinorityShareholderDataFromLatestReport(
            List<BusinessReportType> latestReportCodeList,
            TreasuryDetailReport detailReport)
        {
            var baseUrl = "https://opendart.fss.or.kr/api/mrhlSttus.json";
            var parameters = new Dictionary<string, string>
            {
                ["crtfc_key"] = PrivateData.DART_API_KEY,
                ["corp_code"] = detailReport.CorpCode,
            };

            foreach (var reportCode in latestReportCodeList)
            {
                parameters["bsns_year"] = GetLatestReportBusinessYear(reportCode); // 사업 년도
                parameters["reprt_code"] = ((int)reportCode).ToString(); // 보고서 코드

                var result = await TryGetMinorityShareholderReportAsync(baseUrl, parameters, detailReport.ReceiptNumber);
                if (result.isFetchSuccess)
                {
                    return result;
                }
            }
            return (false, null);
        }



        private async Task<(bool isFetchSuccess, MinorityShareholderStatusReport minorityShareholderReport)> TryGetMinorityShareholderReportAsync(string baseUrl, Dictionary<string, string> parameters, string receiptNumber)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(baseUrl.GetUrlWithQuery(parameters)))
                {
                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        string body = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(body);

                        if (json.TryGetValue("list", out JToken minorityShareholderReportsJson) && minorityShareholderReportsJson.Any())
                        {
                            var minorityShareholderReport = new MinorityShareholderStatusReport(minorityShareholderReportsJson.FirstOrDefault(), receiptNumber);
                            return (true, minorityShareholderReport);
                        }
                        else
                        {
                            _logger.Log("Fail to get list property from minorityShareholderData");
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
            return (false, null);
        }

        // TODO: 장전 모든 회사들에 대한 최신 보고서 종류 및 년도 업데이트 필요
        // 정기 보고서의 종류를 결정하기 위한 함수
        private static List<BusinessReportType> GetLatestReportCode(int settlementMonth = 12)
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

        private static string GetLatestReportBusinessYear(BusinessReportType reportCode)
        {
            var year = DateTime.Now.Year;
            if (reportCode == BusinessReportType.FOURTH_QUARTER)
            {
                return (year - 1).ToString();
            }
            return year.ToString();
        }

        private Dictionary<string, TreasuryStock> MergeData(List<MajorInfoReport> majorInfoReportList, List<TreasuryDetailReport> detailReportList, List<MinorityShareholderStatusReport> minorityShareholderReportList)
        {
            var treasuryStockDict = new Dictionary<string, TreasuryStock>();

            return majorInfoReportList.Select(majorInfoReport =>
            {
                var reciptNumber = majorInfoReport.ReceiptNumber ?? string.Empty; // unique key
                var detailReport = detailReportList.Find(detaildata => detaildata.ReceiptNumber == reciptNumber);
                var minorityShareholderReport = minorityShareholderReportList.Find(minoritydata => minoritydata.ReceiptNumber == reciptNumber);
                return new TreasuryStock(majorInfoReport, detailReport, minorityShareholderReport.HoldStockCount);
            }).ToDictionary(
                (treasuryStock => treasuryStock.ReceiptNumber),
                (treasuryStock => treasuryStock));
        }

        public List<Message> GetMessages()
        {
            return _treasuryStockDict.Select(keyValuePair => keyValuePair.Value)
                    .Select(stock => new Message(stock))
                    .ToList();
        }
    }
}
