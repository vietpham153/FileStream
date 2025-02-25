using System.Net;
using System.Text;

namespace httplistener
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (HttpListener.IsSupported)
            {
                Console.WriteLine("Supported");
            }
            else
            {
                Console.WriteLine("Not Supported");
                throw new Exception("Not Supported");
            }

            var server = new HttpListener();

            server.Prefixes.Add("http://localhost:8080/");
            server.Start();
            Console.WriteLine("Server is listening");

            do
            {
                var context = await server.GetContextAsync();

                Console.WriteLine("Server connected");

                var respone = context.Response;
                var outputStream = respone.OutputStream;
                respone.Headers.Add("Content-Type", "text/html");

                var html = "<h1>Hello World</h1>";
                var buffer = Encoding.UTF8.GetBytes(html);
                await outputStream.WriteAsync(buffer, 0, buffer.Length);
                outputStream.Close();
            } while (server.IsListening);

        }
    }
}
