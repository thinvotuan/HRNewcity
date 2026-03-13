using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BatDongSan.Helper.Common;
using BatDongSan.Utils.Paging;
using BatDongSan.Helper.Utils;
using System.Data;
using System.Globalization;
using System.Text;
using System.IO;
using BatDongSan.Models.NhanSu;
namespace BatDongSan.Controllers.TinhCong
{
    public class ChamCongController : ApplicationController
    {

        private LinqNhanSuDataContext nhanSuContext = new LinqNhanSuDataContext();
        private IList<BatDongSan.Models.DanhMuc.tbl_DM_PhongBan> phongBans;
        private StringBuilder buildTree;
        private readonly string MCV = "ChamCongAdmin";
        private bool? permission;
        public ActionResult XemTinhHinhRaVao()
        {
            #region Role user
            permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            AdminNhanSu(GetUser().manv);
            thang(DateTime.Now.Month);
            nam(DateTime.Now.Year);
            return View("");
        }
        public ActionResult LoadXemTinhHinhRaVao(string qSearch, int thang, int nam, int _page = 0)
        {
            #region Role user
            permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            //BatDongSan.Models.ChamCong.LinqChamCongServerDataContext contextCC = new BatDongSan.Models.ChamCong.LinqChamCongServerDataContext();
            string maNhanVien = GetUser().manv;
            int page = _page == 0 ? 1 : _page;
            int pIndex = page;
            ////Get ma cham cong
            //var getMaCC = nhanSuContext.tbl_NS_NhanViens.Where(d => d.maNhanVien == maNhanVien).FirstOrDefault();
            //if (getMaCC != null)
            //{
            //    var maChamCong = getMaCC.maChamCong;

            int total = nhanSuContext.sp_NS_XemTinhHinhRaVao(maNhanVien, thang, nam, qSearch).Count();
            PagingLoaderFullController("/ChamCong/XemTinhHinhRaVao/", total, page, "?qsearch=" + qSearch + "&maNhanVien=" + maNhanVien);
            ViewData["lsDanhSach"] = nhanSuContext.sp_NS_XemTinhHinhRaVao(maNhanVien, thang, nam, qSearch).Skip(start).Take(offset).ToList();

            ViewData["qSearch"] = qSearch;
            return PartialView("_LoadXemTinhHinhRaVao");
            //}
            //else {
            //    return View("error");
            //}
        }
        //public ActionResult XemTinhHinhRaVaoCongNhan()
        //{
        //    #region Role user
        //    permission = GetPermission("XemRaVaoToanCTy", BangPhanQuyen.QuyenXem);
        //    if (!permission.HasValue)
        //        return View("LogIn");
        //    if (!permission.Value)
        //        return View("AccessDenied");
        //    #endregion
        //    thang(DateTime.Now.Month);
        //    nam(DateTime.Now.Year);
        //    return View("");
        //}
        //public ActionResult LoadXemTinhHinhRaVaoCongNhan(string qSearch, int thang, int nam, int _page = 0)
        //{
        //    #region Role user
        //    permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
        //    if (!permission.HasValue)
        //        return View("LogIn");
        //    if (!permission.Value)
        //        return View("AccessDenied");
        //    #endregion
        //    //BatDongSan.Models.ChamCong.LinqChamCongServerDataContext contextCC = new BatDongSan.Models.ChamCong.LinqChamCongServerDataContext();
        //    string maNhanVien = GetUser().manv;
        //    int page = _page == 0 ? 1 : _page;
        //    int pIndex = page;
        //    ////Get ma cham cong
        //    //var getMaCC = nhanSuContext.tbl_NS_NhanViens.Where(d => d.maNhanVien == maNhanVien).FirstOrDefault();
        //    //if (getMaCC != null)
        //    //{
        //    //    var maChamCong = getMaCC.maChamCong;

        //    int total = nhanSuContext.sp_NS_XemTinhHinhRaVao(maNhanVien, thang, nam, qSearch).Count();
        //    PagingLoaderFullController("/ChamCong/XemTinhHinhRaVao/", total, page, "?qsearch=" + qSearch + "&maNhanVien=" + maNhanVien);
        //    ViewData["lsDanhSach"] = nhanSuContext.sp_NS_XemTinhHinhRaVao(maNhanVien, thang, nam, qSearch).Skip(start).Take(offset).ToList();

        //    ViewData["qSearch"] = qSearch;
        //    return PartialView("_LoadXemTinhHinhRaVao");
        //    //}
        //    //else {
        //    //    return View("error");
        //    //}
        //}
        public ActionResult XemBangLuong()
        {
            #region Role user
            permission = GetPermission("XemBangLuong", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion

            nam(DateTime.Now.Year);
            return View("");
        }

        public ActionResult ViewChiTietLuong(string thang, string nam)
        {
            #region Role user
            permission = GetPermission("XemBangLuong", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            var dsMauIn = nhanSuContext.GetTable<BatDongSan.Models.HeThong.Sys_PrintTemplate>().Where(d => d.maMauIn == "MIBLNV").FirstOrDefault();
            string noiDung = string.Empty;
            var ds = nhanSuContext.tbl_NS_BangLuongNhanViens
                               .Where(t => t.maNhanVien == GetUser().manv && t.thang.ToString() == thang && nam == t.nam.ToString()).FirstOrDefault();
            ViewData["chiTiet"] = dsMauIn;
            if (dsMauIn != null)
            {
                double tongBaoHiem = (ds.baoHiem ?? 0);
                double tongPhuCapTruyLanh = (ds.congTacPhi ?? 0) + (ds.TTTLBaoHiem ?? 0) + (ds.TTTLThue ?? 0) + (ds.TTTLTamUng ?? 0) + (ds.phuCapKhac ?? 0) + (ds.TTTLLuong ?? 0);
                noiDung = dsMauIn.html.Replace("{$thang}", Convert.ToString(thang))
                    .Replace("{$nam}", Convert.ToString(nam))
                    .Replace("{$hoVaTen}", ds.hoTen)
                    .Replace("{$tongLuong}", String.Format("{0:###,##0}", ds.tongLuong ?? 0))
                    .Replace("{$luongDongBaoHiem}", String.Format("{0:###,##0}", ds.luongDongBaoHiem ?? 0))
                    .Replace("{$khoanBoSungLuong}", String.Format("{0:###,##0}", ds.khoanBoSungLuong ?? 0))
                    .Replace("{$phuCapCongTrinh}", String.Format("{0:###,##0}", ds.phuCapCongTrinh ?? 0))
                    .Replace("{$ngayCongChuan}", Convert.ToString(ds.ngayCongChuan ?? 0))
                    .Replace("{$ngayCongTinhLuong}", Convert.ToString(ds.tongNgayCong ?? 0))
                    .Replace("{$ngayCong}", Convert.ToString((ds.soNgayQuet ?? 0) + (ds.soNgayCongTac ?? 0)))
                    .Replace("{$nghiBu}", Convert.ToString(ds.soNgayNghiBu ?? 0))
                    .Replace("{$nghiPhep}", Convert.ToString(ds.soNgayNghiPhep ?? 0))
                    .Replace("{$nghiLeTet}", Convert.ToString(ds.soNgayNghiLe ?? 0))
                    .Replace("{$luyKeThangTruoc}", Convert.ToString(ds.soNgayPhepLuyKeThangTruoc ?? 0))
                    .Replace("{$nghiKhongLuong}", Convert.ToString(ds.soNgayNghiKhongLuong ?? 0))
                    //Lương Theo Ngày Công
                    .Replace("{$luongTheoNgayCong}", String.Format("{0:###,##0}", ds.luongThang ?? 0))
                    //Phụ Cấp Khác Và Truy Lãnh

                    .Replace("{$tongPhuCapTruyLanh}", String.Format("{0:###,##0}", tongPhuCapTruyLanh))
                    .Replace("{$tienPhuCapCongTacTD}", String.Format("{0:###,##0}", ds.congTacPhi ?? 0))
                    .Replace("{$tienTTTLBaoHiem}", String.Format("{0:###,##0}", ds.TTTLBaoHiem ?? 0))
                    .Replace("{$tienTTTLThue}", String.Format("{0:###,##0}", ds.TTTLThue ?? 0))
                    .Replace("{$tienTTTLTamUng}", String.Format("{0:###,##0}", ds.TTTLTamUng ?? 0))
                    .Replace("{$tienTTTLLuong}", String.Format("{0:###,##0}", ds.TTTLLuong ?? 0))
                    .Replace("{$TienPhuCapKhac}", String.Format("{0:###,##0}", ds.phuCapKhac ?? 0))
                    //Các Khoản Khấu Trừ
                    .Replace("{$tongKhauTru}", String.Format("{0:###,##0}", tongBaoHiem + (ds.thue ?? 0)))
                    .Replace("{$tongBaoHiem}", String.Format("{0:###,##0}", tongBaoHiem))
                    .Replace("{$tienThueTNCC}", String.Format("{0:###,##0}", ds.thue ?? 0))
                    .Replace("{$tienGiamTruGC}", String.Format("{0:###,##0}", (ds.giamTruBanThan ?? 0) + (ds.giamTruNguoiPhuThuoc ?? 0)))
                    .Replace("{$tongThucNhanLuong}", String.Format("{0:###,##0}", ds.thucLanh ?? 0));

            }
            ViewBag.NoiDung = noiDung;
            // return PartialView("_ViewChiTietLuong");
            return PartialView("_ViewChiTietLuongTemplate");
        }
        public ActionResult LoadXemBangLuong(int nam, int _page = 0)
        {
            #region Role user
            permission = GetPermission("XemBangLuong", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            string maNhanVien = GetUser().manv;
            int page = _page == 0 ? 1 : _page;
            int pIndex = page;
            int total = nhanSuContext.sp_NS_BangLuongDanhChoNhanVien(maNhanVien, nam).Count();
            PagingLoaderFullController("/ChamCong/XemBangLuong/", total, page, "?maNhanVien=" + maNhanVien);
            ViewData["lsDanhSach"] = nhanSuContext.sp_NS_BangLuongDanhChoNhanVien(maNhanVien, nam).Skip(start).Take(offset).ToList();

            return PartialView("_LoadXemBangLuong");
        }
        public ActionResult LoadBangLuongThangMuoi()
        {
            #region Role user
            permission = GetPermission("XemBangLuong", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            ViewBag.lstThangMuoi = nhanSuContext.tbl_NS_NhanVien_Thuongs.Where(d => d.maNhanVien == GetUser().manv).ToList();
            return PartialView("_LoadBangLuongThangMuoi");
        }
        public ActionResult XemBangLuongThuongTet()
        {
            #region Role user
            permission = GetPermission("XemBangLuongTT", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion

            nam(DateTime.Now.Year);
            return View("");
        }
        public ActionResult LoadXemBangLuongThuongTet(int nam)
        {
            #region Role user
            permission = GetPermission("XemBangLuongTT", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            string maNhanVien = GetUser().manv;
            string ckDuyet = "khong";
            var checkDuyet = nhanSuContext.tbl_DuyetBangLuongThuongTets.Where(d => d.nam == nam).FirstOrDefault();
            if (checkDuyet != null)
            {
                ckDuyet = "duyet";
            }
            ViewBag.ckDuyet = ckDuyet;
            ViewData["lsDanhSach"] = nhanSuContext.sp_NS_BangLuongThuongTet(nam, "", "", "").Where(d => d.maNhanVien == maNhanVien).ToList();

            return PartialView("_LoadXemBangLuongThuongTet");
        }
        public ActionResult XemBangLuongNam2017()
        {
            #region Role user
            permission = GetPermission("XemBangLuongNam2017", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion


            return View("");
        }
        public ActionResult LoadXemBangLuong2017()
        {
            #region Role user
            permission = GetPermission("XemBangLuongNam2017", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            string maNhanVien = GetUser().manv;
            ViewBag.lsDanhSach = nhanSuContext.tbl_NS_DieuChinhLuongs.Where(d => d.maNhanVien == maNhanVien && d.nam == 2017).ToList();

            return PartialView("LoadXemBangLuong2017");
        }
        public ActionResult LoadBangLuongThangMuoiMot()
        {
            #region Role user
            permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            //var lstKPI = nhanSuContext.tbl_NS_NhanVien_KPIs.ToList();
            //foreach (var item in lstKPI) {
            //    var nhanVienThuong = nhanSuContext.tbl_NS_NhanVien_Thuongs.Where(d => d.maNhanVien == item.maNhanVien).FirstOrDefault();
            //    if (nhanVienThuong != null)
            //    {
            //        nhanVienThuong.diemKPI = item.diemKPI;
            //        nhanSuContext.SubmitChanges();
            //    }
            //}
            ViewBag.lstThangMuoi = nhanSuContext.tbl_NS_NhanVien_Thuong11s.OrderBy(d => d.maNhanVien).Where(d => d.maNhanVien == GetUser().manv).ToList();
            return PartialView("_LoadBangLuongThangMuoiMot");
        }
        public ActionResult XemBangChamCongChiTiet()
        {
            #region Role user
            permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            buildTree = new StringBuilder();
            phongBans = nhanSuContext.GetTable<BatDongSan.Models.DanhMuc.tbl_DM_PhongBan>().ToList();
            buildTree = TreePhongBanAjax.BuildTreeDepartment(phongBans);
            ViewBag.PhongBans = buildTree.ToString();
            thang(DateTime.Now.Month);
            nam(DateTime.Now.Year);
            return View("");
        }
        public ActionResult LoadBangChamCongChiTiet(string qSearch, int thang, int nam, int _page = 0)
        {
            #region Role user
            permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            string maNhanVien = GetUser().manv;
            int page = _page == 0 ? 1 : _page;
            int pIndex = page;
            int total = nhanSuContext.sp_NS_BangChamCongChiTiet(thang, nam, qSearch, maNhanVien).Count();
            PagingLoaderFullController("/ChamCong/LoadBangChamCongChiTiet/", total, page, "?qsearch=" + qSearch);
            ViewData["lsDanhSach"] = nhanSuContext.sp_NS_BangChamCongChiTiet(thang, nam, qSearch, maNhanVien).Skip(start).Take(offset).ToList();

            ViewData["qSearch"] = qSearch;
            return PartialView("_LoadBangChamCongChiTiet");
        }


        public ActionResult XemTinhHinhRaVaoCongNhan()
        {
            #region Role user
            permission = GetPermission("ChamCongAdminCN", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            return View("");
        }
        public ActionResult LoadXemTinhHinhRaVaoCongNhan(string qSearch, string tuNgay, string denNgay, int _page = 0)
        {
            #region Role user
            permission = GetPermission("ChamCongAdminCN", BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            DateTime? fromDate = null;
            DateTime? toDate = null;
            if (!String.IsNullOrEmpty(tuNgay))
                fromDate = DateTime.ParseExact(tuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (!String.IsNullOrEmpty(denNgay))
                toDate = DateTime.ParseExact(denNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            string maNhanVien = GetUser().manv;
            int page = _page == 0 ? 1 : _page;
            int pIndex = page;

            int total = nhanSuContext.sp_NS_XemTinhHinhRaVaoCongNhan(fromDate, toDate, qSearch).Count();
            PagingLoaderFullController("/ChamCong/XemTinhHinhRaVaoCongNhan/", total, page, "?qsearch=" + qSearch);
            ViewData["lsDanhSach"] = nhanSuContext.sp_NS_XemTinhHinhRaVaoCongNhan(fromDate, toDate, qSearch).Skip(start).Take(offset).ToList();

            ViewData["qSearch"] = qSearch;
            ViewBag.tuNgay = tuNgay;
            ViewBag.denNgay = tuNgay;
            return PartialView("_LoadXemTinhHinhRaVaoCongNhan");

        }


        public ActionResult LoadBangChamCongTongHop(string qSearch, int nam, int _page = 0)
        {
            #region Role user
            permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion

            int page = _page == 0 ? 1 : _page;
            int pIndex = page;
            string maNhanVien = GetUser().manv;
            int total = nhanSuContext.sp_NS_BangTongHopCongThang(null, nam, qSearch, maNhanVien, 0, null, null).Count();
            PagingLoaderFullController("/BangChamCongTongHop/LoadBangChamCongTongHop/", total, page, "?qsearch=" + qSearch);
            ViewData["lsDanhSach"] = nhanSuContext.sp_NS_BangTongHopCongThang(null, nam, qSearch, maNhanVien, 0, null, null).Skip(start).Take(offset).ToList();

            ViewData["qSearch"] = qSearch;
            return PartialView("_LoadBangChamCongTongHop");
        }
        private void thang(int value)
        {
            Dictionary<int, string> dics = new Dictionary<int, string>();
            for (int i = 1; i < 13; i++)
            {
                dics[i] = i.ToString();
            }
            ViewData["thang"] = new SelectList(dics, "Key", "Value", value);
            ViewData["thangtc"] = new SelectList(dics, "Key", "Value", value);
        }
        private void nam(int value)
        {
            Dictionary<int, string> dics = new Dictionary<int, string>();
            for (int i = (DateTime.Now.Year - 5); i < (DateTime.Now.Year + 5); i++)
            {
                dics[i] = i.ToString();
            }
            ViewData["nam"] = new SelectList(dics, "Key", "Value", value);
            ViewData["namtc"] = new SelectList(dics, "Key", "Value", value);
        }

        #region Import file excel từ Zoom vào dữ liệu chấm công

        public FileResult DownloadImportFile()
        {
            string savedFileName = Path.Combine("/UploadFiles/Template/", "ImportDuLieuChamCongZoom.xlsx");
            return File(savedFileName, "multipart/form-data", "ImportDuLieuChamCongZoom.xlsx");
        }

        public ActionResult ImportExcelData(string excelPath)
        {
            try
            {
                string[] supportedFiles = { ".xlsx", ".xls" };
                HttpPostedFileBase File;
                File = Request.Files[0];
                if (File.ContentLength > 0)
                {
                    string extension = Path.GetExtension(File.FileName);
                    bool exist = Array.Exists(supportedFiles, element => element == extension);
                    if (exist == false)
                    {
                        return Json(new { success = false });
                    }
                    else
                    {
                        var date = DateTime.Now.ToString("yyyyMMdd-HHMMss");
                        string savedLocation = "/UploadFiles/NhanVien/";
                        Directory.CreateDirectory(savedLocation);
                        var filePath = Server.MapPath(savedLocation);
                        string fileName = date.ToString() + File.FileName;
                        string savedFileName = Path.Combine(filePath, fileName);
                        File.SaveAs(savedFileName);

                        ExcelDataProcessing excelDataProcessor = new ExcelDataProcessing(savedFileName);
                        DataTable dt = excelDataProcessor.GetDataTableWorkSheet("Sheet1");
                        List<tbl_NS_ChamCong> chamCong = new List<tbl_NS_ChamCong>();
                        tbl_NS_ChamCong cc;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (!String.IsNullOrEmpty(row["Mã nhân viên"].ToString()) && !String.IsNullOrEmpty(row["Email"].ToString()))
                            {

                                CultureInfo culture = new CultureInfo("en-US");
                                //Convert.ToDateTime(desiredValue);

                                string maChamCong = nhanSuContext.tbl_NS_NhanViens.Where(d => d.email.Trim().ToLower() == row["Email"].ToString().Trim().ToLower() && d.maNhanVien.Trim().ToLower() == row["Mã nhân viên"].ToString().Trim().ToLower() && ((d.trangThai ?? 0) == 0)).Select(d => d.maChamCong).FirstOrDefault() ?? string.Empty;
                                if (!string.IsNullOrEmpty(maChamCong))
                                {
                                    if (!String.IsNullOrEmpty(row["Thời gian chấm công 1"].ToString()))
                                    {
                                        cc = new tbl_NS_ChamCong();
                                        cc.maChamCong = maChamCong;
                                        cc.maMayChamCong = row["Mã máy chấm công"].ToString().Trim().ToUpper();
                                        string thoiGianChamCong1 = row["Thời gian chấm công 1"].ToString();
                                        try
                                        {
                                            System.Globalization.CultureInfo cti = new System.Globalization.CultureInfo("vi-VN");
                                            DateTime date1 = DateTime.Parse(thoiGianChamCong1, cti, DateTimeStyles.None);
                                            cc.checkTime = date1;

                                        }
                                        catch
                                        {
                                            cc.checkTime = ConvertDatetimeApp(thoiGianChamCong1);
                                        }
                                        chamCong.Add(cc);


                                    }
                                    if (!String.IsNullOrEmpty(row["Thời gian chấm công 2"].ToString()))
                                    {
                                        cc = new tbl_NS_ChamCong();
                                        cc.maChamCong = maChamCong;
                                        cc.maMayChamCong = row["Mã máy chấm công"].ToString().Trim().ToUpper();
                                        string thoiGianChamCong2 = row["Thời gian chấm công 2"].ToString();
                                        try
                                        {
                                            System.Globalization.CultureInfo cti = new System.Globalization.CultureInfo("vi-VN");
                                            DateTime date1 = DateTime.Parse(thoiGianChamCong2, cti, DateTimeStyles.None);
                                            cc.checkTime = date1;

                                        }
                                        catch
                                        {
                                            cc.checkTime = ConvertDatetimeApp(thoiGianChamCong2);
                                        }
                                        chamCong.Add(cc);
                                    }
                                }
                            }

                        }
                        nhanSuContext.tbl_NS_ChamCongs.InsertAllOnSubmit(chamCong);
                        nhanSuContext.SubmitChanges();
                        System.IO.File.Delete(Server.MapPath("/UploadFiles/NhanVien/" + fileName));
                    }
                }
                SaveActiveHistory("Import danh sách nhân viên");
                return Json(new { success = true });
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region Hàm convert datetime
        public DateTime ConvertDatetimeApp(string giaTri)
        {
            string[] formats = {
                                   
                         //ngày tháng năm
                         "d/M/yyyy h:mm:ss tt", "d/M/yyyy h:mm tt", 
                         "dd/MM/yyyy hh:mm:ss","dd/MM/yyyy hh:mm:ss tt",
                         "d/M/yyyy h:mm:ss", "dd/MM/yyyy h:mm:ss tt","dd/MM/yyyy h:mm:ss",
                         "d/M/yyyy hh:mm tt", "d/M/yyyy hh tt", 
                         "d/M/yyyy h:mm", "d/M/yyyy h:mm", 
                         "dd/MM/yyyy hh:mm", "dd/M/yyyy hh:mm",

                         "d-M-yyyy h:mm:ss tt", "d-M-yyyy h:mm tt", 
                         "dd-MM-yyyy hh:mm:ss","dd-MM-yyyy hh:mm:ss tt",
                         "d-M-yyyy h:mm:ss", "dd-MM/yyyy h:mm:ss tt","dd-MM-yyyy h:mm:ss",
                         "d-M-yyyy hh:mm tt", "d-M-yyyy hh tt", 
                         "d-M-yyyy h:mm", "d-M-yyyy h:mm", 
                         "dd-MM-yyyy hh:mm", "dd-MM-yyyy hh:mm",
                                   //tháng ngày năm
                         "M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt", 
                         "MM/dd/yyyy hh:mm:ss","MM/dd/yyyy hh:mm:ss tt",
                         "M/d/yyyy h:mm:ss", "MM/dd/yyyy h:mm:ss tt","MM/dd/yyyy h:mm:ss",
                         "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", 
                         "M/d/yyyy h:mm", "M/d/yyyy h:mm", 
                         "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm",

                         "M-d-yyyy h:mm:ss tt", "M-d-yyyy h:mm tt", 
                         "MM-dd-yyyy hh:mm:ss","MM-dd-yyyy hh:mm:ss tt",
                         "M-d-yyyy h:mm:ss", "MM-dd/yyyy h:mm:ss tt","MM-dd-yyyy h:mm:ss",
                         "M-d-yyyy hh:mm tt", "M-d-yyyy hh tt", 
                         "M-d-yyyy h:mm", "M-d-yyyy h:mm", 
                         "MM-dd-yyyy hh:mm", "M-dd-yyyy hh:mm"
                          };
            return DateTime.ParseExact(giaTri, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
        }
        #endregion
    }
}
