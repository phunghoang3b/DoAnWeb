using DoAnWeb.Models;
using PagedList;
using System;
using System.IO;
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
        [ValidateInput(false)]
        public ActionResult Themmoisanpham(tblSanPham sanpham, HttpPostedFileBase fileupload)
        {
            //Đưa du liệu vào dropdownload
            ViewBag.MaLoai = new SelectList(db.tblLoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(db.tblThuongHieus.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            ViewBag.MaNCC = new SelectList(db.tblNhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            //Kiểm tra đường dẫn file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn lại đường dẫn";
                return View();
            }
            //Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file, lưu ý bổ sung thư viện using System.IO;
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Content/Hinhsanpham"), fileName);
                    //Kiểm tra hình ảnh tồn tại chưa?
                    string min = DateTime.Now.ToString("mm");
                    string sec = DateTime.Now.ToString("ss");
                    string MaSanPham = "S" + "" + min + "" + sec;
                    sanpham.MaSP = MaSanPham;
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn 
                        fileupload.SaveAs(path);
                    }
                    sanpham.HinhAnh = fileName;
                    //Lưu vào CSDL
                    db.tblSanPhams.InsertOnSubmit(sanpham);
                    db.SubmitChanges();
                }
                return RedirectToAction("Sanpham");
            }
        }

        //Hiển thị sản phẩm
        public ActionResult Chitietsanpham(string id)
        {
            //Lấy ra đối tượng sản phẩm theo mã 
            tblSanPham sanpham = db.tblSanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }

        //Xóa sản phẩm
        [HttpGet]
        public ActionResult Xoasanpham(string id)
        {
            //Lấy ra đối tượng sản phẩm cần xóa theo mã 
            tblSanPham sanpham = db.tblSanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }
        [HttpPost, ActionName("Xoasanpham")]
        public ActionResult Xacnhanxoa(string id)
        {
            tblSanPham sanpham = db.tblSanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.tblSanPhams.DeleteOnSubmit(sanpham);
            db.SubmitChanges();
            return RedirectToAction("Sanpham");
        }
        //Chỉnh sửa sản phẩm 
        [HttpGet]
        public ActionResult Suasanpham(string id)
        {
            //lay ra doi tuong san pham theo ma
            tblSanPham sanpham = db.tblSanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu vao Dropdownlist
            //Lay ds tu table loai san pham, sắp xếp tang dan theo ten loai san pham, chọn lấy giá trị hiện ra tên 
            ViewBag.MaLoai = new SelectList(db.tblLoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(db.tblThuongHieus.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            ViewBag.MaNCC = new SelectList(db.tblNhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            return View(sanpham);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasanpham(tblSanPham sanpham, HttpPostedFileBase fileupload)
        {
            ViewBag.MaLoai = new SelectList(db.tblLoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(db.tblThuongHieus.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            ViewBag.MaNCC = new SelectList(db.tblNhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            //Kiem tra duong dan file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn hình ảnh";
                return View();
            }
            //Them vao csdl
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten file, luu y bo sung thu vien using system.io;
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/Content/Hinhsanpham"), fileName);
                    //Kiem tra hinh anh ton tai chua
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //luu hinh anh vao duong dan
                        fileupload.SaveAs(path);
                    }
                    sanpham.HinhAnh = fileName;
                    //luu vao csdl
                    UpdateModel(sanpham);
                    db.SubmitChanges();
                }
                return RedirectToAction("Sanpham");
            }
        }

        //Phần quản lý Loại Sản Phẩm 
        public ActionResult Loaisanpham()
        {
            return View(db.tblLoaiSanPhams.ToList());
        }
        [HttpGet]
        public ActionResult Themloaisanpham()
        {
            return View();
        }
    }
}