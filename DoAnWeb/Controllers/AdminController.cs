using DoAnWeb.Models;
using PagedList;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnWeb.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbQLBanGameDataContext db = new dbQLBanGameDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Sanpham(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            //return View(db.tblSanPhams.ToList());
            return View(db.tblSanPhams.ToList().OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //Gan gia tri nguoi dung nhap cho cac bien
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập !";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu !";
            }
            else
            {
                //Gan gia tri cho doi tuong tao moi
                tblTaiKhoan ad = db.tblTaiKhoans.SingleOrDefault(n => n.Username == tendn && n.Password == matkhau);
                if (ad != null)
                {
                    //ViewBag.Thongbao = "Chúc mừng đăng nhập thành công"
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Themmoisanpham()
        {
            //Dua du lieu vao dropdownlist
            //Lay ds tu table loai san pham, sap xep tang dan theo ten loai, chon lay gia tri MaLoai,thì hien thị TenLoai
            ViewBag.MaLoai = new SelectList(db.tblLoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(db.tblThuongHieus.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            ViewBag.MaNCC = new SelectList(db.tblNhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            return View();
        }
        [HttpPost]
        public ActionResult Themmoisanpham(tblSanPham sanpham, HttpPostedFileBase fileupload)
        {
            return View();
        }
    }
}