using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BatDongSan.Helper.Utils;
using BatDongSan.Models.NhanSu;
using BatDongSan.Models.PhieuDeNghi;
using BatDongSan.Models.HeThong;
using BatDongSan.Models.DanhMuc;
using System.Text;
using BatDongSan.Utils.Paging;
using BatDongSan.Helper.Common;
using System.Globalization;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using Worldsoft.Mvc.Web.Util;

namespace BatDongSan.Controllers.PhieuDeNghi
{
    public class PhieuKeKhaiYTeController : ApplicationController
    {
        LinqPhieuDeNghiDataContext lqPhieuDN = new LinqPhieuDeNghiDataContext();
        LinqDanhMucDataContext linqDM = new LinqDanhMucDataContext();
        public bool? permission;
        public const string taskIDSystem = "KhaiBaoYTe";//REQUESTLEAVE
        private PhieuKeKhaiYTeModel keKhai;
        private tbl_NS_KhaiBaoYTe khaiBao;
        //
        // GET: /PhieuKeKhaiYTe/

        #region Danh sách phiếu kê khai y tết

        public ActionResult Index()
        {

            try
            {
                #region Role user
                permission = GetPermission(taskIDSystem, BangPhanQuyen.QuyenXem);
                if (!permission.HasValue)
                    return View("LogIn");
                if (!permission.Value)
                    return View("AccessDenied");
                #endregion

                BuitlTinhThanh(string.Empty);
                BindDataQuanHuyen(string.Empty, null);
                return View("Index");
            }
            catch (Exception ex)
            {
                return View("error");
            }

        }


        public ActionResult ViewIndex(string searchString, string tuNgay, string denNgay, string maTinhThanh, int? idQuanHuyen, int _page = 0)
        {
            try
            {

                #region Role user
                permission = GetPermission(taskIDSystem, BangPhanQuyen.QuyenXem);
                if (!permission.HasValue)
                    return PartialView("LogIn");
                if (!permission.Value)
                    return PartialView("AccessDenied");
                #endregion
                Administrator(GetUser().manv);
                DateTime? fromDate = null;
                DateTime? toDate = null;
                if (!String.IsNullOrEmpty(tuNgay))
                {
                    fromDate = DateTime.ParseExact(tuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (!String.IsNullOrEmpty(denNgay))
                {
                    toDate = DateTime.ParseExact(denNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                var datas = lqPhieuDN.NS_KhaiBaoYTe_Index_Group(fromDate, toDate, GetUser().manv, searchString, maTinhThanh, idQuanHuyen).ToList();
                ViewData["lsDanhSach"] = datas;
                return PartialView("ViewIndex");
            }
            catch (Exception ex)
            {

                return PartialView("error");
            }

        }
        #endregion

        #region Thêm, xóa, sửa phiếu kê khai y tế
        public ActionResult Create()
        {

            try
            {
                #region Role user
                permission = GetPermission(taskIDSystem, BangPhanQuyen.QuyenThem);
                if (!permission.HasValue)
                    return PartialView("LogIn");
                if (!permission.Value)
                    return PartialView("AccessDenied");
                #endregion
                ThongTinPhieuKeKhai(string.Empty);
                return View(keKhai);
            }
            catch (Exception ex)
            {
                return View("error");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection coll)
        {
            try
            {
                #region Role user
                permission = GetPermission(taskIDSystem, BangPhanQuyen.QuyenThem);
                if (!permission.HasValue)
                    return PartialView("LogIn");
                if (!permission.Value)
                    return PartialView("AccessDenied");
                #endregion
                BindDataToSave(coll, string.Empty);
                lqPhieuDN.tbl_NS_KhaiBaoYTes.InsertOnSubmit(khaiBao);
                lqPhieuDN.SubmitChanges();
                return RedirectToAction("Edit", new { id = khaiBao.MaPhieu });
            }
            catch (Exception ex)
            {
                return View("error");
            }
        }

        public ActionResult Edit(string id)
        {

            try
            {
                #region Role user
                permission = GetPermission(taskIDSystem, BangPhanQuyen.QuyenSua);
                if (!permission.HasValue)
                    return PartialView("LogIn");
                if (!permission.Value)
                    return PartialView("AccessDenied");
                #endregion
                ThongTinPhieuKeKhai(id);
                return View(keKhai);
            }
            catch (Exception ex)
            {
                return View("error");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection coll)
        {
            try
            {
                #region Role user
                permission = GetPermission(taskIDSystem, BangPhanQuyen.QuyenSua);
                if (!permission.HasValue)
                    return PartialView("LogIn");
                if (!permission.Value)
                    return PartialView("AccessDenied");
                #endregion
                BindDataToSave(coll, coll.Get("MaPhieu"));
                lqPhieuDN.SubmitChanges();
                return RedirectToAction("Edit", new { id = khaiBao.MaPhieu });
            }
            catch (Exception ex)
            {
                return View("error");
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {

            try
            {
                #region Role user
                permission = GetPermission(taskIDSystem, BangPhanQuyen.QuyenXoa);
                if (!permission.HasValue)
                    return View("LogIn");
                if (!permission.Value)
                    return View("AccessDenied");
                #endregion

                var delPhieu = lqPhieuDN.tbl_NS_KhaiBaoYTes.Where(d => d.MaPhieu == id).FirstOrDefault();
                lqPhieuDN.tbl_NS_KhaiBaoYTes.DeleteOnSubmit(delPhieu);
                lqPhieuDN.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //write log
                Log4Net.WriteLog(log4net.Core.Level.Error, ex.Message);

                return View("error");
            }

        }
        #endregion

        #region Thông tin phiếu kê khai y tế

        public void BindDataToSave(FormCollection col, string maPhieu)
        {
            if (string.IsNullOrEmpty(maPhieu))
            {
                khaiBao = new tbl_NS_KhaiBaoYTe();
                khaiBao.MaNhanVienLapPhieu = GetUser().manv;
                khaiBao.NgayLap = DateTime.Now;
                khaiBao.MaPhieu = GenerateUtil.CheckLetter("PKKYT", GetMax());
            }
            else
            {
                khaiBao = lqPhieuDN.tbl_NS_KhaiBaoYTes.Where(d => d.MaPhieu == maPhieu).FirstOrDefault();
            }
            khaiBao.MaNhanVien = col.Get("MaNhanVienKk");
            khaiBao.GioiTinh = col.Get("GioiTinh") == "True" ? true : false;
            khaiBao.DienThoai = col.Get("DienThoai");
            khaiBao.DiaChiHienNay = col.Get("DiaChiHienNay");
            khaiBao.MaTinhThanh = col.Get("MaTinhThanh");
            khaiBao.IDQuanHuyen = Convert.ToInt32(col.Get("MaQuanHuyen"));
            khaiBao.TenPhuongXa = col.Get("TenPhuongXa");
            khaiBao.TrangThaiLamViecTuXa = col.Get("TrangThaiLamViecTuXa") == "True" ? true : false;
            khaiBao.TrangThaiTiepXuc = col.Get("TrangThaiTiepXuc") == "True" ? true : false;
            if (khaiBao.TrangThaiTiepXuc == true)
            {
                khaiBao.NgayTiepXuc = String.IsNullOrEmpty(col.Get("NgayTiepXuc")) ? (DateTime?)null : DateTime.ParseExact(col.Get("NgayTiepXuc"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            khaiBao.TrangThaiPhongToaTaiKhuVucMinhSong = col.Get("TrangThaiPhongToaTaiKhuVucMinhSong") == "True" ? true : false;
            if (khaiBao.TrangThaiPhongToaTaiKhuVucMinhSong == true)
            {
                khaiBao.TuNgayPhongToa = String.IsNullOrEmpty(col.Get("TuNgayPhongToa")) ? (DateTime?)null : DateTime.ParseExact(col.Get("TuNgayPhongToa"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                khaiBao.DenNgayPhongToa = String.IsNullOrEmpty(col.Get("DenNgayPhongToa")) ? (DateTime?)null : DateTime.ParseExact(col.Get("DenNgayPhongToa"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            khaiBao.TrangThaiPhongToaGan = col.Get("TrangThaiPhongToaGan") == "True" ? true : false;
            if (khaiBao.TrangThaiPhongToaGan == true)
            {
                khaiBao.TuNgayPhongToaGan = String.IsNullOrEmpty(col.Get("TuNgayPhongToaGan")) ? (DateTime?)null : DateTime.ParseExact(col.Get("TuNgayPhongToaGan"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                khaiBao.DenNgayPhongToaGan = String.IsNullOrEmpty(col.Get("DenNgayPhongToaGan")) ? (DateTime?)null : DateTime.ParseExact(col.Get("DenNgayPhongToaGan"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            khaiBao.GhiChuPhongToaGan = col.Get("GhiChuPhongToaGan");
            khaiBao.TrangThaiLayMauXetNghiem = col.Get("TrangThaiLayMauXetNghiem") == "True" ? true : false;
            if (khaiBao.TrangThaiLayMauXetNghiem == true)
            {
                khaiBao.SoLanXetNghiem = string.IsNullOrEmpty(col.Get("SoLanXetNghiem")) ? 0 : Convert.ToInt32(col.Get("SoLanXetNghiem"));
                khaiBao.NgayLayMau = String.IsNullOrEmpty(col.Get("NgayLayMau")) ? (DateTime?)null : DateTime.ParseExact(col.Get("NgayLayMau"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            khaiBao.TrangThaiHo = col.Get("TrangThaiHo") == "True" ? true : false;
            khaiBao.TrangThaiSotTren37 = col.Get("TrangThaiSotTren37") == "True" ? true : false;
            khaiBao.TrangThaiKhoTho = col.Get("TrangThaiKhoTho") == "True" ? true : false;
            khaiBao.TrangThaiDauHong = col.Get("TrangThaiDauHong") == "True" ? true : false;
            khaiBao.TrangThaiMatViGiac = col.Get("TrangThaiMatViGiac") == "True" ? true : false;
            khaiBao.TrangThaiMatKhuuGiac = col.Get("TrangThaiMatKhuuGiac") == "True" ? true : false;
            khaiBao.TrieuChungKhac = col.Get("TrieuChungKhac") == "True" ? true : false;
        }

        public void ThongTinPhieuKeKhai(string maPhieu)
        {
            if (string.IsNullOrEmpty(maPhieu))
            {
                keKhai = new PhieuKeKhaiYTeModel();
                keKhai.MaNhanVienLapPhieu = GetUser().manv;
                keKhai.TenNhanVienLapPhieu = HoVaTen(keKhai.MaNhanVienLapPhieu);
                keKhai.MaNhanVienKk = keKhai.MaNhanVienLapPhieu;
                keKhai.TenNhanVienKK = keKhai.TenNhanVienLapPhieu;
                keKhai.NgayLap = DateTime.Now;
                keKhai.MaPhieu = GenerateUtil.CheckLetter("PKKYT", GetMax());
            }
            else
            {
                keKhai = (from kk in lqPhieuDN.tbl_NS_KhaiBaoYTes
                          join nv in lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.tbl_NS_NhanVien>() on kk.MaNhanVienLapPhieu equals nv.maNhanVien into dtDong
                          from nv2 in dtDong.DefaultIfEmpty()

                          join nvkk in lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.tbl_NS_NhanVien>() on kk.MaNhanVien equals nvkk.maNhanVien into dtDong2
                          from nvkk2 in dtDong2.DefaultIfEmpty()

                          join tt in lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.Sys_TinhThanh>() on kk.MaTinhThanh equals tt.maTinhThanh into dtDong3
                          from tt2 in dtDong3.DefaultIfEmpty()

                          join qh in lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.Sys_QuanHuyen>() on kk.IDQuanHuyen equals qh.id into dtDong4
                          from qh2 in dtDong4.DefaultIfEmpty()
                          where kk.MaPhieu == maPhieu
                          select new PhieuKeKhaiYTeModel
                          {
                              MaNhanVienLapPhieu = kk.MaNhanVienLapPhieu,
                              TenNhanVienLapPhieu = nv2.ho + " " + nv2.ten,
                              NgayLap = kk.NgayLap,
                              MaNhanVienKk = kk.MaNhanVien,
                              TenNhanVienKK = nvkk2.ho + " " + nvkk2.ten,
                              MaPhieu = kk.MaPhieu,
                              TrangThaiLamViecTuXa = kk.TrangThaiLamViecTuXa ?? false,
                              TrangThaiTiepXuc = kk.TrangThaiTiepXuc,
                              TrangThaiPhongToaTaiKhuVucMinhSong = kk.TrangThaiPhongToaTaiKhuVucMinhSong,
                              TuNgayPhongToa = kk.TuNgayPhongToa,
                              DenNgayPhongToa = kk.DenNgayPhongToa,
                              TrangThaiPhongToaGan = kk.TrangThaiPhongToaGan ?? false,
                              GhiChuPhongToaGan = kk.GhiChuPhongToaGan,
                              TrangThaiLayMauXetNghiem = kk.TrangThaiLayMauXetNghiem ?? false,
                              SoLanXetNghiem = kk.SoLanXetNghiem ?? 0,
                              NgayLayMau = kk.NgayLayMau,
                              TrangThaiHo = kk.TrangThaiHo ?? false,
                              TrangThaiSotTren37 = kk.TrangThaiSotTren37 ?? false,
                              TrangThaiKhoTho = kk.TrangThaiKhoTho ?? false,
                              TrangThaiDauHong = kk.TrangThaiDauHong ?? false,
                              TrangThaiMatViGiac = kk.TrangThaiMatViGiac ?? false,
                              TrangThaiMatKhuuGiac = kk.TrangThaiMatKhuuGiac ?? false,
                              TrieuChungKhac = kk.TrieuChungKhac ?? false,
                              GioiTinh = kk.GioiTinh,
                              DienThoai = kk.DienThoai,
                              DiaChiHienNay = kk.DiaChiHienNay,
                              MaTinhThanh = kk.MaTinhThanh,
                              TenTinhThanh = tt2.tenTinhThanh,
                              MaQuanHuyen = kk.IDQuanHuyen ?? 0,
                              TenQuanHuyen = qh2.tenQuanHuyen,
                              TrangThaiNguoiKhai = kk.TrangThaiNguoiKhai ?? false,
                              XacNhan = kk.XacNhan ?? false,
                              TenPhuongXa = kk.TenPhuongXa ?? string.Empty,
                              NgayTiepXuc = kk.NgayTiepXuc,
                              TuNgayPhongToaGan = kk.TuNgayPhongToaGan,
                              DenNgayPhongToaGan = kk.DenNgayPhongToaGan,
                          }).FirstOrDefault();
            }
            BuitlTinhThanh(keKhai.MaTinhThanh);
            BindDataQuanHuyen(keKhai.MaTinhThanh, keKhai.MaQuanHuyen);
        }

        public string GetMax()
        {
            return lqPhieuDN.tbl_NS_KhaiBaoYTes.OrderByDescending(d => d.NgayLap).Select(d => d.MaPhieu).FirstOrDefault();
        }
        #endregion

        #region Binddata
        private void BindDataLeave_Index(string leaveCode)
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("", "[Chọn]");

            foreach (var item in linqDM.tbl_DM_LoaiNghiPheps.ToList())
            {
                dict.Add(item.maLoaiNghiPhep, item.tenLoaiNghiPhep);
            }
            ViewBag.leaveTypes = new SelectList(dict, "Key", "Value", leaveCode);
        }

        public void BuitlTinhThanh(string select)
        {
            IList<BatDongSan.Models.NhanSu.Sys_TinhThanh> tinhThanh = lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.Sys_TinhThanh>().ToList();
            tinhThanh.Insert(0, new BatDongSan.Models.NhanSu.Sys_TinhThanh() { maTinhThanh = "", tenTinhThanh = "[Vui lòng chọn tỉnh/thành phố]" });
            ViewBag.TinhThanh = new SelectList(tinhThanh, "maTinhThanh", "tenTinhThanh", select);
        }

        public void BindDataQuanHuyen(string maTinhThanh, int? idQuanHuyen)
        {
            IList<BatDongSan.Models.NhanSu.Sys_QuanHuyen> quanHuyens = lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.Sys_QuanHuyen>().Where(d => d.maTinhThanh == maTinhThanh).ToList();
            quanHuyens.Insert(0, new BatDongSan.Models.NhanSu.Sys_QuanHuyen() { id = 0, tenQuanHuyen = "[Vui lòng chọn quận/huyện]" });
            ViewBag.QuanHuyen = new SelectList(quanHuyens, "id", "tenQuanHuyen", idQuanHuyen);
        }

        public JsonResult GetQuanHuyen(string id)
        {
            try
            {
                var quanHuyens = lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.Sys_QuanHuyen>().Where(s => s.maTinhThanh == id)
                                        .Select(s => new { s.id, s.tenQuanHuyen }).ToList();
                return Json(quanHuyens);
            }
            catch
            {
                return Json(string.Empty);
            }
        }
        #endregion

        #region Load danh sách nhân viên phòng ban
        /// <summary>
        /// Danh sách nhân viên theo phòng ban
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhSachNVPB()
        {
            try
            {

                StringBuilder buildTree = new StringBuilder();
                var phongBans = linqDM.tbl_DM_PhongBans.ToList();
                buildTree = TreePhongBans.BuildTreeDepartment(phongBans);
                ViewBag.NVPB = buildTree.ToString();
                return View();
            }
            catch (Exception ex)
            {
                //write log
                Log4Net.WriteLog(log4net.Core.Level.Error, ex.Message);

                return View("error");
            }

        }

        public ActionResult LoadNhanVien(int? page, string searchString, string maPhongBan)
        {
            try
            {



                IList<sp_PB_DanhSachNhanVienResult> phongBan1s;
                phongBan1s = linqDM.sp_PB_DanhSachNhanVien(searchString, maPhongBan).ToList();
                ViewBag.isGet = "True";
                int currentPageIndex = page.HasValue ? page.Value : 1;
                ViewBag.Count = phongBan1s.Count();
                ViewBag.Search = searchString;
                ViewBag.MaPhongBan = maPhongBan;
                return PartialView("_LoadNhanVien", phongBan1s.ToPagedList(currentPageIndex, 10));
            }
            catch (Exception ex)
            {
                //write log
                Log4Net.WriteLog(log4net.Core.Level.Error, ex.Message);

                return View("error");
            }

        }
        #endregion


        #region Xác nhận hiếu kê khai y tế
        public JsonResult XacNhanKhaiBaoYT(string id)
        {
            try
            {
                var capNhat = lqPhieuDN.tbl_NS_KhaiBaoYTes.Where(d => d.MaPhieu == id).FirstOrDefault();
                if (capNhat != null && ((capNhat.XacNhan ?? false) == false))
                {
                    capNhat.XacNhan = true;
                    lqPhieuDN.SubmitChanges();
                }
                return Json(string.Empty);
            }
            catch
            {
                return Json("Error");
            }
        }
        #endregion

        #region Load lại phiếu cũ
        public ActionResult LoadLaiPhieuCu(string id)
        {
            try
            {
                keKhai = (from kk in lqPhieuDN.tbl_NS_KhaiBaoYTes
                          join nv in lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.tbl_NS_NhanVien>() on kk.MaNhanVienLapPhieu equals nv.maNhanVien into dtDong
                          from nv2 in dtDong.DefaultIfEmpty()

                          join nvkk in lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.tbl_NS_NhanVien>() on kk.MaNhanVien equals nvkk.maNhanVien into dtDong2
                          from nvkk2 in dtDong2.DefaultIfEmpty()

                          join tt in lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.Sys_TinhThanh>() on kk.MaTinhThanh equals tt.maTinhThanh into dtDong3
                          from tt2 in dtDong3.DefaultIfEmpty()

                          join qh in lqPhieuDN.GetTable<BatDongSan.Models.NhanSu.Sys_QuanHuyen>() on kk.IDQuanHuyen equals qh.id into dtDong4
                          from qh2 in dtDong4.DefaultIfEmpty()
                          where kk.MaNhanVien == id
                          select new PhieuKeKhaiYTeModel
                          {
                              MaNhanVienLapPhieu = kk.MaNhanVienLapPhieu,
                              TenNhanVienLapPhieu = nv2.ho + " " + nv2.ten,
                              NgayLap = kk.NgayLap,
                              MaNhanVienKk = kk.MaNhanVien,
                              TenNhanVienKK = nvkk2.ho + " " + nvkk2.ten,
                              MaPhieu = kk.MaPhieu,
                              TrangThaiLamViecTuXa = kk.TrangThaiLamViecTuXa ?? false,
                              TrangThaiTiepXuc = kk.TrangThaiTiepXuc,
                              TrangThaiPhongToaTaiKhuVucMinhSong = kk.TrangThaiPhongToaTaiKhuVucMinhSong,
                              TuNgayPhongToa = kk.TuNgayPhongToa,
                              DenNgayPhongToa = kk.DenNgayPhongToa,
                              TrangThaiPhongToaGan = kk.TrangThaiPhongToaGan ?? false,
                              GhiChuPhongToaGan = kk.GhiChuPhongToaGan,
                              TrangThaiLayMauXetNghiem = kk.TrangThaiLayMauXetNghiem ?? false,
                              SoLanXetNghiem = kk.SoLanXetNghiem ?? 0,
                              NgayLayMau = kk.NgayLayMau,
                              TrangThaiHo = kk.TrangThaiHo ?? false,
                              TrangThaiSotTren37 = kk.TrangThaiSotTren37 ?? false,
                              TrangThaiKhoTho = kk.TrangThaiKhoTho ?? false,
                              TrangThaiDauHong = kk.TrangThaiDauHong ?? false,
                              TrangThaiMatViGiac = kk.TrangThaiMatViGiac ?? false,
                              TrangThaiMatKhuuGiac = kk.TrangThaiMatKhuuGiac ?? false,
                              TrieuChungKhac = kk.TrieuChungKhac ?? false,
                              GioiTinh = kk.GioiTinh,
                              DienThoai = kk.DienThoai,
                              DiaChiHienNay = kk.DiaChiHienNay,
                              MaTinhThanh = kk.MaTinhThanh,
                              TenTinhThanh = tt2.tenTinhThanh,
                              MaQuanHuyen = kk.IDQuanHuyen ?? 0,
                              TenQuanHuyen = qh2.tenQuanHuyen,
                              TrangThaiNguoiKhai = kk.TrangThaiNguoiKhai ?? false,
                              XacNhan = kk.XacNhan ?? false,
                              TenPhuongXa = kk.TenPhuongXa ?? string.Empty,
                              NgayTiepXuc = kk.NgayTiepXuc,
                              TuNgayPhongToaGan = kk.TuNgayPhongToaGan,
                              DenNgayPhongToaGan = kk.DenNgayPhongToaGan,
                          }).OrderByDescending(d => d.MaPhieu).FirstOrDefault();
                return PartialView("LoadPhieuCu", keKhai);
            }
            catch
            {
                return Json(string.Empty);
            }
        }
        #endregion

        #region Xuất excel
        public void XuatFileKeKhaiYTe(string searchString, string tuNgay, string denNgay, string maTinhThanh, int? idQuanHuyen)
        {
            var filename = "";
            var virtualPath = HttpRuntime.AppDomainAppVirtualPath;
            var fileStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(virtualPath + @"\Content\Report\ReportTemplate.xls"), FileMode.Open, FileAccess.Read);

            var workbook = new HSSFWorkbook(fileStream, true);
            filename += "DanhSachKeKhaiYTe.xls";

            DateTime? fromDate = null;
            DateTime? toDate = null;
            if (!String.IsNullOrEmpty(tuNgay))
            {
                fromDate = DateTime.ParseExact(tuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(denNgay))
            {
                toDate = DateTime.ParseExact(denNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            var sheet = workbook.GetSheet("danhsachnhanvien");

            /*style title start*/
            //tạo font cho các title
            //font tiêu đề 
            HSSFFont hFontTieuDe = (HSSFFont)workbook.CreateFont();
            hFontTieuDe.FontHeightInPoints = 18;
            hFontTieuDe.Boldweight = 100 * 10;
            hFontTieuDe.FontName = "Times New Roman";
            hFontTieuDe.Color = HSSFColor.BLUE.index;

            //font tiêu đề 
            HSSFFont hFontTongGiaTriHT = (HSSFFont)workbook.CreateFont();
            hFontTongGiaTriHT.FontHeightInPoints = 11;
            hFontTongGiaTriHT.Boldweight = (short)FontBoldWeight.BOLD;
            hFontTongGiaTriHT.FontName = "Times New Roman";
            hFontTongGiaTriHT.Color = HSSFColor.BLACK.index;

            //font thông tin bảng tính
            HSSFFont hFontTT = (HSSFFont)workbook.CreateFont();
            hFontTT.IsItalic = true;
            hFontTT.Boldweight = (short)FontBoldWeight.BOLD;
            hFontTT.Color = HSSFColor.BLACK.index;
            hFontTT.FontName = "Times New Roman";
            hFontTieuDe.FontHeightInPoints = 11;

            //font chứ hoa đậm
            HSSFFont hFontNommalUpper = (HSSFFont)workbook.CreateFont();
            hFontNommalUpper.Boldweight = (short)FontBoldWeight.BOLD;
            hFontNommalUpper.Color = HSSFColor.BLACK.index;
            hFontNommalUpper.FontName = "Times New Roman";

            //font chữ bình thường
            HSSFFont hFontNommal = (HSSFFont)workbook.CreateFont();
            hFontNommal.Color = HSSFColor.BLACK.index;
            hFontNommal.FontName = "Times New Roman";

            //font chữ bình thường đậm
            HSSFFont hFontNommalBold = (HSSFFont)workbook.CreateFont();
            hFontNommalBold.Color = HSSFColor.BLACK.index;
            hFontNommalBold.Boldweight = (short)FontBoldWeight.BOLD;
            hFontNommalBold.FontName = "Times New Roman";

            //tạo font cho các title end

            //Set style
            var styleTitle = workbook.CreateCellStyle();
            styleTitle.SetFont(hFontTieuDe);
            styleTitle.Alignment = HorizontalAlignment.LEFT;

            //style infomation
            var styleInfomation = workbook.CreateCellStyle();
            styleInfomation.SetFont(hFontTT);
            styleInfomation.Alignment = HorizontalAlignment.LEFT;

            //style header
            var styleheadedColumnTable = workbook.CreateCellStyle();
            styleheadedColumnTable.SetFont(hFontNommalUpper);
            styleheadedColumnTable.WrapText = true;
            styleheadedColumnTable.BorderBottom = CellBorderType.THIN;
            styleheadedColumnTable.BorderLeft = CellBorderType.THIN;
            styleheadedColumnTable.BorderRight = CellBorderType.THIN;
            styleheadedColumnTable.BorderTop = CellBorderType.THIN;
            styleheadedColumnTable.VerticalAlignment = VerticalAlignment.CENTER;
            styleheadedColumnTable.Alignment = HorizontalAlignment.CENTER;

            var styleHeading1 = workbook.CreateCellStyle();
            styleHeading1.SetFont(hFontNommalBold);
            styleHeading1.WrapText = true;
            styleHeading1.BorderBottom = CellBorderType.THIN;
            styleHeading1.BorderLeft = CellBorderType.THIN;
            styleHeading1.BorderRight = CellBorderType.THIN;
            styleHeading1.BorderTop = CellBorderType.THIN;
            styleHeading1.VerticalAlignment = VerticalAlignment.CENTER;
            styleHeading1.Alignment = HorizontalAlignment.LEFT;

            var hStyleConLeft = (HSSFCellStyle)workbook.CreateCellStyle();
            hStyleConLeft.SetFont(hFontNommal);
            hStyleConLeft.VerticalAlignment = VerticalAlignment.TOP;
            hStyleConLeft.Alignment = HorizontalAlignment.LEFT;
            hStyleConLeft.WrapText = true;
            hStyleConLeft.BorderBottom = CellBorderType.THIN;
            hStyleConLeft.BorderLeft = CellBorderType.THIN;
            hStyleConLeft.BorderRight = CellBorderType.THIN;
            hStyleConLeft.BorderTop = CellBorderType.THIN;

            var hStyleConRight = (HSSFCellStyle)workbook.CreateCellStyle();
            hStyleConRight.SetFont(hFontNommal);
            hStyleConRight.VerticalAlignment = VerticalAlignment.TOP;
            hStyleConRight.Alignment = HorizontalAlignment.RIGHT;
            hStyleConRight.BorderBottom = CellBorderType.THIN;
            hStyleConRight.BorderLeft = CellBorderType.THIN;
            hStyleConRight.BorderRight = CellBorderType.THIN;
            hStyleConRight.BorderTop = CellBorderType.THIN;


            var hStyleConCenter = (HSSFCellStyle)workbook.CreateCellStyle();
            hStyleConCenter.SetFont(hFontNommal);
            hStyleConCenter.VerticalAlignment = VerticalAlignment.TOP;
            hStyleConCenter.Alignment = HorizontalAlignment.CENTER;
            hStyleConCenter.BorderBottom = CellBorderType.THIN;
            hStyleConCenter.BorderLeft = CellBorderType.THIN;
            hStyleConCenter.BorderRight = CellBorderType.THIN;
            hStyleConCenter.BorderTop = CellBorderType.THIN;
            //set style end


            Row rowC = null;
            //Khai báo row đầu tiên
            int firstRowNumber = 1;

            string rowtitle = "DANH SÁCH NHÂN VIÊN KÊ KHAI Y TẾ";
            var titleCell = HSSFCellUtil.CreateCell(sheet.CreateRow(firstRowNumber), 6, rowtitle.ToUpper());
            titleCell.CellStyle = styleTitle;

            ++firstRowNumber;

            var list1 = new List<string>();
            list1.Add("STT");
            list1.Add("Mã nhân viên");
            list1.Add("Tên nhân viên");
            list1.Add("Nhân viên kê khai dùm");
            list1.Add("Ngày lập");
            list1.Add("Số nhà, Đường");
            list1.Add("Phường/ Xã");
            list1.Add("Tỉnh/ Thành phố");
            list1.Add("Quận/ Huyện");
            list1.Add("Làm việc từ xa");
            list1.Add("Tiếp xúc với người xác định hoặc bị nghi nhiễm");
            list1.Add("Ngày tiếp xúc");
            list1.Add("Nơi mình sống bị phong tỏa cách ly");
            list1.Add("Từ ngày phong tỏa");
            list1.Add("Đến ngày phong tỏa");
            list1.Add("Gần Nơi mình sống bị phong tỏa cách ly");
            list1.Add("Ghi chú");
            list1.Add("Từ ngày phong tỏa");
            list1.Add("Đến ngày phong tỏa");
            list1.Add("14 ngày qua có lấy mẫu xét nghiệm");
            list1.Add("Số lần xét nghiệm");
            list1.Add("Ngày lấy mẫu");
            list1.Add("Ho");
            list1.Add("Sốt trên 37 độ");
            list1.Add("Khó thở");
            list1.Add("Đau họng");
            list1.Add("Mất vị giác");
            list1.Add("Mất khứu giác");
            list1.Add("Triệu chứng khác");
            //Start row 13
            var headerRow = sheet.CreateRow(2);
            ReportHelperExcel.CreateHeaderRow(headerRow, 0, styleheadedColumnTable, list1);


            var idRowStart = 3;
            var datas = lqPhieuDN.NS_KhaiBaoYTe_XuatFileExcel(fromDate, toDate, GetUser().manv, searchString, maTinhThanh, idQuanHuyen).ToList();
            //#region
            if (datas != null && datas.Count > 0)
            {
                var stt = 0;
                int dem = 0;
                foreach (var item in datas)
                {
                    dem = 0;

                    idRowStart++;
                    rowC = sheet.CreateRow(idRowStart);
                    ReportHelperExcel.SetAlignment(rowC, dem++, (++stt).ToString(), hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.MaNhanVien, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, (item.tenNhanVien), hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, (item.TenNhanVienKeKhaiDum), hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, String.Format("{0:dd/MM/yyyy}", item.NgayLap), hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.DiaChiHienNay, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TenPhuongXa, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TenTinhThanh, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TenQuanHuyen, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiLamViecTuXa == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiTiepXuc == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, String.Format("{0:dd/MM/yyyy}", item.NgayTiepXuc), hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiPhongToaTaiKhuVucMinhSong == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, String.Format("{0:dd/MM/yyyy}", item.TuNgayPhongToa), hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, String.Format("{0:dd/MM/yyyy}", item.DenNgayPhongToa), hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiPhongToaGan == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.GhiChuPhongToaGan, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, String.Format("{0:dd/MM/yyyy}", item.TuNgayPhongToaGan), hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, String.Format("{0:dd/MM/yyyy}", item.DenNgayPhongToaGan), hStyleConCenter);

                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiLayMauXetNghiem == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.SoLanXetNghiem.ToString(), hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, String.Format("{0:dd/MM/yyyy}", item.NgayLayMau), hStyleConCenter);

                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiHo == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiSotTren37 == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiKhoTho == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiDauHong == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiMatViGiac == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrangThaiMatKhuuGiac == true ? "1" : "0", hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item.TrieuChungKhac == true ? "1" : "0", hStyleConCenter);
                }

                sheet.SetColumnWidth(0, 5 * 250);
                sheet.SetColumnWidth(1, 15 * 270);
                sheet.SetColumnWidth(2, 20 * 250);
                sheet.SetColumnWidth(3, 25 * 210);
                sheet.SetColumnWidth(4, 30 * 210);
                sheet.SetColumnWidth(5, 30 * 210);
                sheet.SetColumnWidth(6, 30 * 210);
                sheet.SetColumnWidth(7, 30 * 210);
                sheet.SetColumnWidth(8, 30 * 210);
                sheet.SetColumnWidth(9, 30 * 210);
                sheet.SetColumnWidth(10, 20 * 210);
                sheet.SetColumnWidth(11, 15 * 210);
                sheet.SetColumnWidth(12, 15 * 210);
                sheet.SetColumnWidth(13, 15 * 210);
                sheet.SetColumnWidth(14, 15 * 210);
                sheet.SetColumnWidth(15, 30 * 210);
                sheet.SetColumnWidth(16, 15 * 210);
                sheet.SetColumnWidth(17, 15 * 210);
                sheet.SetColumnWidth(18, 15 * 210);
                sheet.SetColumnWidth(19, 15 * 210);
                sheet.SetColumnWidth(20, 15 * 210);
                sheet.SetColumnWidth(21, 15 * 210);
                sheet.SetColumnWidth(22, 15 * 210);
                sheet.SetColumnWidth(23, 15 * 210);
                sheet.SetColumnWidth(24, 15 * 210);
                sheet.SetColumnWidth(25, 15 * 210);
                sheet.SetColumnWidth(26, 15 * 210);
                sheet.SetColumnWidth(27, 15 * 210);
                sheet.SetColumnWidth(28, 15 * 210);
            }
            else
            {

                sheet.SetColumnWidth(0, 5 * 250);
                sheet.SetColumnWidth(1, 15 * 270);
                sheet.SetColumnWidth(2, 20 * 250);
                sheet.SetColumnWidth(3, 25 * 210);
                sheet.SetColumnWidth(4, 30 * 210);
                sheet.SetColumnWidth(5, 30 * 210);
                sheet.SetColumnWidth(6, 30 * 210);
                sheet.SetColumnWidth(7, 30 * 210);
                sheet.SetColumnWidth(8, 30 * 210);
                sheet.SetColumnWidth(9, 30 * 210);
                sheet.SetColumnWidth(10, 20 * 210);
                sheet.SetColumnWidth(11, 15 * 210);
                sheet.SetColumnWidth(12, 15 * 210);
                sheet.SetColumnWidth(13, 15 * 210);
                sheet.SetColumnWidth(14, 15 * 210);
                sheet.SetColumnWidth(15, 30 * 210);
                sheet.SetColumnWidth(16, 15 * 210);
                sheet.SetColumnWidth(17, 15 * 210);
                sheet.SetColumnWidth(18, 15 * 210);
                sheet.SetColumnWidth(19, 15 * 210);
                sheet.SetColumnWidth(20, 15 * 210);
                sheet.SetColumnWidth(21, 15 * 210);
                sheet.SetColumnWidth(22, 15 * 210);
                sheet.SetColumnWidth(23, 15 * 210);
                sheet.SetColumnWidth(24, 15 * 210);
                sheet.SetColumnWidth(25, 15 * 210);
                sheet.SetColumnWidth(26, 15 * 210);
                sheet.SetColumnWidth(27, 15 * 210);
                sheet.SetColumnWidth(28, 15 * 210);

            }

            var stream = new MemoryStream();
            workbook.Write(stream);

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            Response.Clear();

            Response.BinaryWrite(stream.GetBuffer());
            Response.End();

        }
        #endregion
    }
}
