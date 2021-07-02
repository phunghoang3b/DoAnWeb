using System;
using System.Linq;

namespace DoAnWeb.Models
{
    public class Giohang
    {
        //Tạo đối tượng data chứa dữ liệu từ model dbBanGame đã tạo
        dbQLBanGameDataContext data = new dbQLBanGameDataContext();
        public string iMaSP { set; get; }
        public string sTenSP { set; get; }
        public string sHinhAnh { set; get; }
        public Double dDonGia { set; get; }
        public int iSoLuong { set; get; }
        public Double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        //Khởi tạo giỏ hàng theo MaSP được truyền vào với Soluong mặc định là 1
        public Giohang(string MaSP)
        {
            iMaSP = MaSP;
            tblSanPham game = data.tblSanPhams.Single(n => n.MaSP == iMaSP);
            sTenSP = game.TenSP;
            sHinhAnh = game.HinhAnh;
            dDonGia = double.Parse(game.GiaTien.ToString());
            iSoLuong = 1;
        }
    }
}