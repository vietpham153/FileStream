using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public partial class SanPham
    {
        public void InsertInfor(string name, int money, string details, string QRcode)
        {
            this.name = name;
            this.money = money;
            this.details = details;
            this.QRcode = QRcode;
        }
    }
}
