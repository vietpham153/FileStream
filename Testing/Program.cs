using System;
using System.IO;
using System.Text;

namespace Test
{
    class Product
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }

        public void Save(Stream stream)
        {
            // Lưu Id (4 byte)
            var byte_id = BitConverter.GetBytes(Id);
            stream.Write(byte_id, 0, 4);

            // Lưu Price (8 byte)
            var byte_price = BitConverter.GetBytes(Price);
            stream.Write(byte_price, 0, 8);

            // Lưu chiều dài chuỗi Name (4 byte)
            var byte_name = Encoding.UTF8.GetBytes(Name);
            var byte_length = BitConverter.GetBytes(byte_name.Length);
            stream.Write(byte_length, 0, 4);

            // Lưu nội dung chuỗi Name
            stream.Write(byte_name, 0, byte_name.Length);
        }

        public void Restore(Stream stream)
        {
            // Đọc Id (4 byte)
            var byte_id = new byte[4];
            stream.Read(byte_id, 0, 4);
            Id = BitConverter.ToInt32(byte_id, 0);

            // Đọc Price (8 byte)
            var byte_price = new byte[8];
            stream.Read(byte_price, 0, 8);
            Price = BitConverter.ToDouble(byte_price, 0);

            // Đọc chiều dài chuỗi Name (4 byte)
            var byte_length = new byte[4];
            stream.Read(byte_length, 0, 4);
            int leng = BitConverter.ToInt32(byte_length, 0);

            // Đọc nội dung chuỗi Name
            var byte_name = new byte[leng];
            stream.Read(byte_name, 0, leng);
            Name = Encoding.UTF8.GetString(byte_name, 0, leng);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string path = "test.txt";

            // Mở file với FileMode.Create, tạo mới hoặc ghi đè nếu file tồn tại
            using var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

            // Tạo và lưu Product
            Product product1 = new Product() { Id = 123, Name = "IP12", Price = 1000 };
            product1.Save(stream);

            // Di chuyển con trỏ về đầu file trước khi đọc
            stream.Seek(0, SeekOrigin.Begin);

            // Khôi phục Product từ stream
            Product product2 = new Product();
            product2.Restore(stream);

            // Hiển thị dữ liệu đã khôi phục
            Console.WriteLine($"Id: {product2.Id}, Name: {product2.Name}, Price: {product2.Price}");
        }
    }
}
