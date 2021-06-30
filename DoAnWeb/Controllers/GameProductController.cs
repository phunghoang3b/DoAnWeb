﻿using DoAnWeb.Models;
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
        public ActionResult Index()
        {
            //Lay 5 game moi nhat
            var gamemoi = Laygamemoi(5);
            return View(gamemoi);
        }
        public ActionResult Loaisp()
        {
            var loaisp = from lsp in data.tblLoaiSanPhams select lsp;
            return PartialView(loaisp);
        }
    }
}