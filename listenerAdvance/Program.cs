using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace listenerAdvance
{
    internal class Program
    {

        class MyHttpServer
        {
            private HttpListener listener;
            public MyHttpServer(string[] prefixes)
            {
                if (!HttpListener.IsSupported)
                {
                    throw new Exception("Not Supported");
                }
                listener = new HttpListener();
                foreach (string s in prefixes)
                {
                    listener.Prefixes.Add(s);
                }
            }
            public async Task Start()
            {
                listener.Start();
                Console.WriteLine("Http Server is Start");

                do
                {
                    Console.WriteLine(DateTime.Now.ToLongTimeString() + " Waiting a client connect");
                    var context = await listener.GetContextAsync();
                    ProcessRequest(context);
                    Console.WriteLine(DateTime.Now.ToLongTimeString() + " Client connected");
                } while (listener.IsListening);
            }

            async Task ProcessRequest(HttpListenerContext context)
            {
                HttpListenerRequest request = context.Request;
                HttpListenerResponse respone = context.Response;

                Console.WriteLine($"{request.HttpMethod} {request.RawUrl}{request.Url.AbsolutePath}");

                var outputStream = respone.OutputStream;
                switch(request.Url.AbsolutePath)
                {
                    case "/":
                        {
                            var buffer = Encoding.UTF8.GetBytes("<h1>Hello World</h1>");
                            respone.ContentLength64 = buffer.Length;
                            await outputStream.WriteAsync(buffer, 0, buffer.Length);
                        }
                    break;
                    case "/json":
                        {
                            respone.Headers.Add("Content-Type", "application/json");

                            var product = new
                            {
                                Name = "Macbook",
                                Price = 2000
                            };

                            var json = JsonConvert.SerializeObject(product);
                            var buffer = Encoding.UTF8.GetBytes(json);
                            respone.ContentLength64 = buffer.Length;
                            await outputStream.WriteAsync(buffer, 0, buffer.Length);
                        }
                        break;


                }
                outputStream.Close();
            }

        }

        static async Task Main(string[] args)
        {
            var server = new MyHttpServer(new string[] { "http://localhost:8080/" });
            await server.Start();
        }
    }
}
