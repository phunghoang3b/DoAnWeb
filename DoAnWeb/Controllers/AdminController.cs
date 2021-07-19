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
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
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
                return View(sanpham);
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

                    var sanpham1 = db.tblSanPhams.FirstOrDefault(p => p.MaSP.Equals(sanpham.MaSP));
                    sanpham1.GiaTien = sanpham.GiaTien;
                    sanpham1.HinhAnh = fileName;
                    sanpham1.MaLoai = sanpham.MaLoai;
                    sanpham1.MaNCC = sanpham.MaNCC;
                    sanpham1.MaSP = sanpham.MaSP;
                    sanpham1.MaTH = sanpham.MaTH;
                    sanpham.MoTa = sanpham.MoTa;
                    sanpham.NgayCapNhat = sanpham.NgayCapNhat;

                    UpdateModel(sanpham1);
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
        [HttpPost]
        public ActionResult Themloaisanpham(FormCollection collection, tblLoaiSanPham lsp)
        {
            //Tạo biến loaisanpham và gán giá trị của người dùng nhập vào
            var loaisp = collection["TenLoai"];
            //nếu loaisanpham có giá trị == null (để trống)
            if (string.IsNullOrEmpty(loaisp))
            {
                ViewData["Loi"] = "Tên loại sản phẩm không được để trống";
            }
            else
            {
                string min = DateTime.Now.ToString("mm");
                string sec = DateTime.Now.ToString("ss");
                string MaLoaiSanPham = "L" + "" + min + "" + sec;
                lsp.MaLoai = MaLoaiSanPham;
                lsp.TenLoai = loaisp;
                db.tblLoaiSanPhams.InsertOnSubmit(lsp);
                db.SubmitChanges();
                return RedirectToAction("Loaisanpham");
            }
            return this.Themloaisanpham();
        }
        //Sua loai san pham
        [HttpGet]
        public ActionResult Sualoaisp(string id)
        {
            var loaisp = db.tblLoaiSanPhams.First(m => m.MaLoai == id);
            return View(loaisp);
        }
        [HttpPost]
        public ActionResult Sualoaisp(string id, FormCollection collection)
        {
            var loaisp = db.tblLoaiSanPhams.First(m => m.MaLoai == id);
            var lsp = collection["TenLoai"];
            loaisp.MaLoai = id;
            if (string.IsNullOrEmpty(lsp))
            {
                ViewData["Loi"] = "Loại sản phẩm  không được để trống";
            }
            else
            {
                loaisp.TenLoai = lsp;
                UpdateModel(lsp);
                db.SubmitChanges();
                return RedirectToAction("Loaisanpham");
            }
            return this.Sualoaisp(id);
        }
        [HttpGet]
        public ActionResult Xoaloaisp(string id)
        {
            var loaisp = db.tblLoaiSanPhams.First(m => m.MaLoai == id);
            return View(loaisp);
        }
        [HttpPost]
        public ActionResult Xoaloaisp(string id, FormCollection collection)
        {
            var loaisp = db.tblLoaiSanPhams.Where(m => m.MaLoai == id).First();
            db.tblLoaiSanPhams.DeleteOnSubmit(loaisp);
            db.SubmitChanges();
            return RedirectToAction("Loaisanpham");
        }

        //Quản lý nhà cung cấp
        public ActionResult Nhacungcap()
        {
            return View(db.tblNhaCungCaps.ToList());
        }
        [HttpGet]
        public ActionResult Themnhacungcap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Themnhacungcap(FormCollection collection, tblNhaCungCap ncc)
        {
            var tencungcap = collection["TenNCC"];
            var diachi = collection["DiaChi"];
            var sdt = collection["SDT"];
            if (string.IsNullOrEmpty(tencungcap))
            {
                ViewData["Loi"] = "Tên nhà cung cấp không được để trống";
            }
            else if (string.IsNullOrEmpty(diachi))
            {
                ViewData["Loi"] = "Phải nhập địa chỉ nhà cung cấp";
            }
            else if (string.IsNullOrEmpty(sdt))
            {
                ViewData["Loi"] = "Số điện thoại không được để trống";
            }
            else
            {
                string min = DateTime.Now.ToString("mm");
                string sec = DateTime.Now.ToString("ss");
                string MaNhaCungCap = "N" + "" + min + "" + sec;
                ncc.MaNCC = MaNhaCungCap;
                ncc.TenNCC = tencungcap;
                ncc.DiaChi = diachi;
                ncc.SDT = sdt;
                db.tblNhaCungCaps.InsertOnSubmit(ncc);
                db.SubmitChanges();
                return RedirectToAction("Nhacungcap");
            }
            return this.Themnhacungcap();
        }
        //Sua nha cung cap
        [HttpGet]
        public ActionResult Suanhacungcap(string id)
        {
            var nhacungcap = db.tblNhaCungCaps.First(m => m.MaNCC == id);
            return View(nhacungcap);
        }
        [HttpPost]
        public ActionResult Suanhacungcap(string id, FormCollection collection)
        {
            var ncc = db.tblNhaCungCaps.First(m => m.MaNCC == id);
            var tenncc = collection["TenNCC"];
            var diachi = collection["DiaChi"];
            var sdt = collection["SDT"];
            ncc.MaNCC = id;
            if (string.IsNullOrEmpty(tenncc))
            {
                ViewData["Loi"] = "Tên nhà cung cấp không được để trống";
            }
            else if (string.IsNullOrEmpty(diachi))
            {
                ViewData["Loi"] = "Phải nhập địa chỉ nhà cung cấp";
            }
            else if (string.IsNullOrEmpty(sdt))
            {
                ViewData["Loi"] = "Số điện thoại không được để trống";
            }
            else
            {
                ncc.TenNCC = tenncc;
                ncc.DiaChi = diachi;
                ncc.SDT = sdt;
                UpdateModel(ncc);
                db.SubmitChanges();
                return RedirectToAction("Nhacungcap");
            }
            return this.Suanhacungcap(id);
        }
        //xoa nha cung cap
        [HttpGet]
        public ActionResult Xoanhacungcap(string id)
        {
            var ncc = db.tblNhaCungCaps.First(m => m.MaNCC == id);
            return View(ncc);
        }
        [HttpPost]
        public ActionResult Xoanhacungcap(string id, FormCollection collection)
        {
            var ncc = db.tblNhaCungCaps.Where(m => m.MaNCC == id).First();
            db.tblNhaCungCaps.DeleteOnSubmit(ncc);
            db.SubmitChanges();
            return RedirectToAction("Nhacungcap");
        }

        //Quản lý Thương hiệu
        public ActionResult Thuonghieu()
        {
            return View(db.tblThuongHieus.ToList());
        }
        [HttpGet]
        public ActionResult Themmoithuonghieu()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Themmoithuonghieu(FormCollection collection, tblThuongHieu th)
        {
            var thuonghieu = collection["TenTH"];
            if (string.IsNullOrEmpty(thuonghieu))
            {
                ViewData["Loi"] = "Tên thương hiệu không được để trống";
            }
            else
            {
                string min = DateTime.Now.ToString("mm");
                string sec = DateTime.Now.ToString("ss");
                string MaThuongHieu = "T" + "" + min + "" + sec;
                th.MaTH = MaThuongHieu;
                th.TenTH = thuonghieu;
                db.tblThuongHieus.InsertOnSubmit(th);
                db.SubmitChanges();
                return RedirectToAction("Thuonghieu");
            }
            return this.Themmoithuonghieu();
        }
        //sua thuong hieu
        [HttpGet]
        public ActionResult Suathuonghieu(string id)
        {
            var thuonghieu = db.tblThuongHieus.First(m => m.MaTH == id);
            return View(thuonghieu);
        }
        [HttpPost]
        public ActionResult Suathuonghieu(string id, FormCollection collection)
        {
            var thuonghieu = db.tblThuongHieus.First(m => m.MaTH == id);
            var tenth = collection["TenTH"];
            thuonghieu.MaTH = id;
            if (string.IsNullOrEmpty(tenth))
            {
                ViewData["Loi"] = "Tên thương hiệu không được để trống tên";
            }
            else
            {
                thuonghieu.TenTH = tenth;
                UpdateModel(thuonghieu);
                db.SubmitChanges();
                return RedirectToAction("Thuonghieu");
            }
            return this.Suathuonghieu(id);
        }
        [HttpGet]
        public ActionResult Xoathuonghieu(string id)
        {
            var thuonghieu = db.tblThuongHieus.First(m => m.MaTH == id);
            return View(thuonghieu);
        }
        [HttpPost]
        public ActionResult Xoathuonghieu(string id, FormCollection collection)
        {
            var thuonghieu = db.tblThuongHieus.Where(m => m.MaTH == id).First();
            db.tblThuongHieus.DeleteOnSubmit(thuonghieu);
            db.SubmitChanges();
            return RedirectToAction("Thuonghieu");
        }

        //Quản lý khách hàng
        public ActionResult Khachhang()
        {
            return View(db.tblKhacHangs.ToList());
        }
        [HttpGet]
        public ActionResult Xoakhachhang(string id)
        {
            var khachhang = db.tblKhacHangs.First(m => m.MaKH == id);
            return View(khachhang);
        }
        [HttpPost]
        public ActionResult Xoakhachhang(string id, FormCollection collection)
        {
            var khachhang = db.tblKhacHangs.Where(m => m.MaKH == id).First();
            db.tblKhacHangs.DeleteOnSubmit(khachhang);
            db.SubmitChanges();
            return RedirectToAction("Khachhang");
        }

        //Quản lý Đơn hàng
        public ActionResult Donhang()
        {
            return View(db.tblDonHangs.ToList());
        }
        //[HttpGet]
        //public ActionResult Suadonhang(string id)
        //{
        //    var donhang = db.tblDonHangs.First(m => m.MaDH == id);
        //    return View(donhang);
        //}
        //[HttpPost]
        //public ActionResult Suadonhang(string id, FormCollection collection)
        //{
        //    var donhang = db.tblDonHangs.First(m => m.MaDH == id);
        //    var ngaylap = collection["NgayLap"];
        //    donhang.MaDH = id;
        //    if (string.IsNullOrEmpty(ngaylap))
        //    {
        //        ViewData["Loi"] = "Ngày lập không được để trống";
        //    }
        //    else
        //    {
        //        UpdateModel(donhang);
        //        db.SubmitChanges();
        //        return RedirectToAction("Donhang");
        //    }
        //    return this.Suadonhang(id);
        //}
    }
}