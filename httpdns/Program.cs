using System.Net;
using System.Text;

namespace httpdns
{
    public class MyHttpClientHandler : HttpClientHandler
    {
        public MyHttpClientHandler(CookieContainer cookie_container)
        {

            CookieContainer = cookie_container;     // Thay thế CookieContainer mặc định
            AllowAutoRedirect = false;                // không cho tự động Redirect
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            UseCookies = true;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                     CancellationToken cancellationToken)
        {
            Console.WriteLine("Bất đầu kết nối " + request.RequestUri.ToString());
            // Thực hiện truy vấn đến Server
            var response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine("Hoàn thành tải dữ liệu");
            return response;
        }
    }

    public class ChangeUri : DelegatingHandler
    {
        public ChangeUri(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            var host = request.RequestUri.Host.ToLower();
            Console.WriteLine($"Check in  ChangeUri - {host}");
            if (host.Contains("google.com"))
            {
                // Đổi địa chỉ truy cập từ google.com sang github
                request.RequestUri = new Uri("https://github.com/");
            }
            // Chuyển truy vấn cho base (thi hành InnerHandler)
            return base.SendAsync(request, cancellationToken);
        }
    }


    public class DenyAccessFacebook : DelegatingHandler
    {
        public DenyAccessFacebook(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                     CancellationToken cancellationToken)
        {

            var host = request.RequestUri.Host.ToLower();
            Console.WriteLine($"Check in DenyAccessFacebook - {host}");
            if (host.Contains("facebook.com"))
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(Encoding.UTF8.GetBytes("Không được truy cập"));
                return await Task.FromResult<HttpResponseMessage>(response);
            }
            // Chuyển truy vấn cho base (thi hành InnerHandler)
            return await base.SendAsync(request, cancellationToken);
        }
    }
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var url = "https://postman-echo.com/post";
            var cookieContainer = new CookieContainer();

            //tao 1 chuoi handler()
            // delegating handler
            var bottomHandle = new MyHttpClientHandler(cookieContainer);
            var changeUriHandler = new ChangeUri(bottomHandle);
            var denyAccessFacebook = new DenyAccessFacebook(changeUriHandler);

            //using var handler = new SocketsHttpHandler();
            //handler.AllowAutoRedirect = true;
            //handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            //handler.UseCookies = true;
            //handler.CookieContainer = cookieContainer;


            using var httpClient = new HttpClient(denyAccessFacebook);

            using var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri(url);
            httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("key1", "value1"));
            parameters.Add(new KeyValuePair<string, string>("key2", "value2-1"));
            parameters.Add(new KeyValuePair<string, string>("key2", "value2-2"));

            httpRequestMessage.Content = new FormUrlEncodedContent(parameters);

            var respone = await httpClient.SendAsync(httpRequestMessage);
            cookieContainer.GetCookies(new Uri(url)).ToList().ForEach(c =>
            {
                Console.WriteLine("------------------------");
                Console.WriteLine($"{c.Name}:{c.Value}");
                Console.WriteLine("------------------------");
            });

            var html = await respone.Content.ReadAsStringAsync();

            Console.WriteLine(html);
        }
    }
}
