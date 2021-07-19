using DoAnWeb.Models;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DoAnWeb.Controllers
{
    public class GameProductController : Controller
    {
        //Tao 1 doi tuong chua toan bo CSDL tu dbQLBanGame   
        dbQLBanGameDataContext data = new dbQLBanGameDataContext();

        private List<tblSanPham> Laygamemoi(int count)
        {
            //Sap xep giam dan theo NgayCapNhat, lay count dong dau
            return data.tblSanPhams.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        // GET: GameProduct
        public ActionResult Index(int? page)
        {
            //Tạo biến quy định số sản phẩm trên mỗi trang
            int pageSize = 9;
            //Tạo biến số trang
            int pageNum = (page ?? 1);
            //Lay 5 game moi nhat
            var gamemoi = Laygamemoi(30);
            return View(gamemoi.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Loaisp()
        {
            var loaisp = from lsp in data.tblLoaiSanPhams select lsp;
            return PartialView(loaisp);
        }
        public ActionResult SPTheotheloai(string id)
        {
            var game = from g in data.tblSanPhams where g.MaLoai == id select g;
            return View(game);
        }
        public ActionResult Details(string id)
        {
            var game = from g in data.tblSanPhams
                       where g.MaSP == id
                       select g;
            return View(game.Single());
        }

        public ActionResult Search()
        {
            var model = (from sp in data.tblSanPhams select sp).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(string searchString)
        {
            var model = from sp in data.tblSanPhams select sp;
            model = model.Where(tk => tk.TenSP.Contains(searchString));
            return View(model);
        }
    }
}