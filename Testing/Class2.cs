using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public partial class SanPham
    {
        public void GetInfor()
        {
            Console.WriteLine($"Name: {name}, price: {money}, Details: {details}, QRCode: {QRcode}");
        }
    }
}
