namespace SearchService.Services
{
    public class AuctionSvcHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AuctionSvcHttpClient(HttpClient httpClient,IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task
    }
}