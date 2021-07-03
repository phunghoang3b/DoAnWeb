using DoAnWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DoAnWeb.Controllers
{
    public class GiohangController : Controller
    {
        // GET: Giohang
        //Tạo đối tượng data chứa dữ liệu từ model dbBanGame đã tạo
        dbQLBanGameDataContext data = new dbQLBanGameDataContext();
        //Lấy giỏ hàng
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì khởi tạo listGioHang
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        //Thêm hàng vào giỏ hàng
        public ActionResult ThemGioHang(string iMaSP, string strURL)
        {
            //Lay ra session gio hang
            List<Giohang> lstGiohang = Laygiohang();
            //Kiem tar san pham nay ton tai trong session ["Giohang"] chua?
            Giohang sanpham = lstGiohang.Find(n => n.iMaSP == iMaSP);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMaSP);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        //Tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        //Tinh tong tien
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }
        //Xay dung trang gio hang
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "GameProduct");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGiohang);
        }
        //Tao Partial view de hien thi thong tin gio hang
        public ActionResult GiohangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult Index()
        {
            return View();
        }
        //Xoa gio hang
        public ActionResult XoaGiohang(string iMaSP)
        {
            //lay gio hang tu session
            List<Giohang> lstGiohang = Laygiohang();
            //Kiem tra san pham da co trong session["Giohang"]
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaSP == iMaSP);
            //Neu ton tai thi cho sua so luong san pham
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMaSP == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "GameProduct");
            }
            return RedirectToAction("GioHang");
        }
        //Cập nhật giỏ hàng
        public ActionResult CapnhatGiohang(string iMaSP, FormCollection f)
        {
            //Lấy giỏ hàng từ session
            List<Giohang> lstGiohang = Laygiohang();
            //Kiem tra sach da co trong session ["GioHang"]
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaSP == iMaSP);
            //Nếu tồn tại thì cho sửa số lượng
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        //Xóa tất cả thông tin trong giỏ hàng
        public ActionResult XoaTatcaGiohang()
        {
            //Lấy gio hàng từ session
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "GameProduct");
        }
        //Hiển thị View DatHang de cap nhat cac thông tin cho don hang
        [HttpGet]
        public ActionResult Dathang()
        {
            //Kiểm tra đăng nhập
            if (Session["Username"] == null || Session["Username"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "GameProduct");
            }
            //Lấy giỏ hàng từ session
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult Dathang(FormCollection collection)
        {
            //Thêm đơn hàng
            tblDonHang ddh = new tblDonHang();
            tblKhacHang kh = (tblKhacHang)Session["Username"];
            List<Giohang> gh = Laygiohang();
            string min = DateTime.Now.ToString("mm");
            string sec = DateTime.Now.ToString("ss");
            string MaDonHang = "D" + "" + min + "" + sec;
            ddh.MaDH = MaDonHang;
            ddh.MaKH = kh.MaKH;
            ddh.NgayLap = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.NgayGiao = DateTime.Parse(ngaygiao);
            ddh.TinhTrangGiaoHang = false;
            ddh.Dathanhtoan = false;
            data.tblDonHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                tblChiTietDonHang ctdh = new tblChiTietDonHang();
                ctdh.MaDH = ddh.MaDH;
                ctdh.MaSP = item.iMaSP;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                ctdh.MaKH = ddh.MaKH;
                data.tblChiTietDonHangs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        //Xác nhận đơn hàng
        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}