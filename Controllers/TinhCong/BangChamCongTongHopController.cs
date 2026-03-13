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
using Worldsoft.Mvc.Web.Util;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using BatDongSan.Models.DBChamCong;
namespace BatDongSan.Controllers.TinhCong
{
    public class BangChamCongTongHopController : ApplicationController
    {

        private LinqNhanSuDataContext nhanSuContext = new LinqNhanSuDataContext();
        BatDongSan.Models.HeThong.LinqHeThongDataContext lqHeThong = new BatDongSan.Models.HeThong.LinqHeThongDataContext();
        private IList<BatDongSan.Models.DanhMuc.tbl_DM_PhongBan> phongBans;
        private StringBuilder buildTree;
        private readonly string MCV = "ChamCongAM";
        private bool? permission;
        public ActionResult Index()
        {
            #region Role user
            permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion
            thang(DateTime.Now.Month);
            nam(DateTime.Now.Year);
            return View("");

        }
        public ActionResult XemBangChamCongChiTiet()
        {
           // ImportDuLieuVanTay(null, null, null, null, null, null, null);
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
            ViewBag.QuyenSuaBaoHiem = QuyenSuaBaoHiem(GetUser().manv);
            ViewBag.MaNhanVien = GetUser().manv;
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

            int page = _page == 0 ? 1 : _page;
            int pIndex = page;
            int total = nhanSuContext.sp_NS_BangChamCongChiTiet(thang, nam, qSearch, "").Count();
            PagingLoaderFullController("/BangChamCongTongHop/LoadBangChamCongChiTiet/", total, page, "?qsearch=" + qSearch);
            ViewData["lsDanhSach"] = nhanSuContext.sp_NS_BangChamCongChiTiet(thang, nam, qSearch, "").Skip(start).Take(1000).ToList();

            ViewData["qSearch"] = qSearch;
            ViewBag.MaNhanVien = GetUser().manv;
            return PartialView("_LoadBangChamCongChiTiet");
        }


        #region Xuat File XuatFileBangTongHopCongThang
        public void XuatFileBangTongHopCongThang(string qSearch, string maPhongBan, int thang, int nam, int _page = 0)
        {
            try
            {
                var filename = "";
                var virtualPath = HttpRuntime.AppDomainAppVirtualPath;

                var fileStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(virtualPath + @"\Content\Report\ReportTemplateTHCT.xls"), FileMode.Open, FileAccess.Read);

                var workbook = new HSSFWorkbook(fileStream, true);

                filename += "BangTongHopCongThang_" + thang + "_" + nam + ".xls";

                #region format style excel cell
                /*style title start*/
                //tạo font cho các title
                //font tiêu đề 
                HSSFFont hFontTieuDe = (HSSFFont)workbook.CreateFont();
                hFontTieuDe.FontHeightInPoints = 11;
                hFontTieuDe.Boldweight = 100 * 10;
                hFontTieuDe.FontName = "Times New Roman";
                //hFontTieuDe.Color = HSSFColor.BLUE.index;

                //font tiêu đề 
                HSSFFont hFontTieuDeUnderline = (HSSFFont)workbook.CreateFont();
                hFontTieuDeUnderline.FontHeightInPoints = 11;
                hFontTieuDeUnderline.Boldweight = 100 * 10;
                hFontTieuDeUnderline.FontName = "Times New Roman";
                hFontTieuDeUnderline.Underline = 1;
                //hFontTieuDe.Color = HSSFColor.BLUE.index;


                HSSFFont hFontTieuDeItalic = (HSSFFont)workbook.CreateFont();
                hFontTieuDeItalic.FontHeightInPoints = 11;
                //hFontTieuDeItalic.Boldweight = 100 * 10;
                hFontTieuDeItalic.FontName = "Times New Roman";
                hFontTieuDeItalic.IsItalic = true;
                //hFontTieuDe.Color = HSSFColor.BLUE.index;


                HSSFFont hFontTieuDeLarge = (HSSFFont)workbook.CreateFont();
                hFontTieuDeLarge.FontHeightInPoints = 16;
                hFontTieuDeLarge.Boldweight = 100 * 10;
                hFontTieuDeLarge.FontName = "Times New Roman";
                //hFontTieuDeLarge.Color = HSSFColor.BLUE.index;

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

                //Set styleUnderline
                var styleTitleUnderline = workbook.CreateCellStyle();
                styleTitleUnderline.SetFont(hFontTieuDeUnderline);
                styleTitleUnderline.Alignment = HorizontalAlignment.LEFT;

                //Set style In nghiêng
                var styleTitleItalic = workbook.CreateCellStyle();
                styleTitleItalic.SetFont(hFontTieuDeItalic);
                styleTitleItalic.Alignment = HorizontalAlignment.LEFT;

                //Set style Large font
                var styleTitleLarge = workbook.CreateCellStyle();
                styleTitleLarge.SetFont(hFontTieuDeLarge);
                styleTitleLarge.Alignment = HorizontalAlignment.LEFT;

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

                //style sum cell
                var styleCellSumary = workbook.CreateCellStyle();
                styleCellSumary.SetFont(hFontNommalUpper);
                styleCellSumary.WrapText = true;
                styleCellSumary.BorderBottom = CellBorderType.THIN;
                styleCellSumary.BorderLeft = CellBorderType.THIN;
                styleCellSumary.BorderRight = CellBorderType.THIN;
                styleCellSumary.BorderTop = CellBorderType.THIN;
                styleCellSumary.VerticalAlignment = VerticalAlignment.CENTER;
                styleCellSumary.Alignment = HorizontalAlignment.RIGHT;

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
                #endregion

                //Khai báo row
                Row rowC = null;



                var sheet = workbook.CreateSheet("BangTongHopCongThang");

                //Khai báo row đầu tiên
                int firstRowNumber = 3;

                string cellTenCty = "TỔNG CÔNG TY XDCTGT 6 - CÔNG TY CỔ PHẦN";
                var titleCellCty = HSSFCellUtil.CreateCell(sheet.CreateRow(0), 0, cellTenCty.ToUpper());
                titleCellCty.CellStyle = styleTitle;

                string cellTenCacBanDH = "CÁC BAN ĐIỀU HÀNH DỰ ÁN";
                var titleCellTenCacBanDH = HSSFCellUtil.CreateCell(sheet.CreateRow(1), 1, cellTenCacBanDH.ToUpper());
                titleCellTenCacBanDH.CellStyle = styleTitle;

                string cellTitleMain = "BẢNG TỔNG HỢP CÔNG THÁNG " + thang + "/" + nam;
                var titleCellTitleMain = HSSFCellUtil.CreateCell(sheet.CreateRow(2), 5, cellTitleMain.ToUpper());
                titleCellTitleMain.CellStyle = styleTitle;

                firstRowNumber++;

                var list1 = new List<string>();
                list1.Add("STT");
                list1.Add("Họ tên");
                list1.Add("Mã nhân viên");
                list1.Add("Mã chấm công");
                list1.Add("Phòng ban");
                list1.Add("Chức vụ");
                list1.Add("Công chuẩn");
                list1.Add("Số ngày quét");
                list1.Add("Công tác");
                list1.Add("Nghỉ phép có lương");
                list1.Add("Nghỉ phép không lương");
                list1.Add("Nghỉ lễ");
                list1.Add("Lũy kế tháng trước");
                list1.Add("Tổng cộng");

                if (GetUser().manv == "BGD001" || GetUser().manv == "NV-1706004")
                {
                    list1.Add("Tổng tiềm cơm");
                }

                var idRowStart = firstRowNumber; // bat dau o dong thu 4
                var headerRow = sheet.CreateRow(idRowStart);
                int rowend = idRowStart;
                ReportHelperExcel.CreateHeaderRow(headerRow, 0, styleheadedColumnTable, list1);
                idRowStart++;
                sheet.SetColumnWidth(0, 5 * 210);
                sheet.SetColumnWidth(1, 30 * 210);
                sheet.SetColumnWidth(2, 15 * 210);
                sheet.SetColumnWidth(3, 15 * 210);
                sheet.SetColumnWidth(4, 30 * 210);
                sheet.SetColumnWidth(5, 15 * 210);
                sheet.SetColumnWidth(6, 15 * 210);
                sheet.SetColumnWidth(7, 15 * 210);
                sheet.SetColumnWidth(8, 15 * 210);
                sheet.SetColumnWidth(9, 15 * 210);
                sheet.SetColumnWidth(10, 15 * 210);
                sheet.SetColumnWidth(11, 15 * 210);
                if (GetUser().manv == "BGD001" || GetUser().manv == "NV-1706004")
                {
                    sheet.SetColumnWidth(16, 30 * 210);
                }

                var data = nhanSuContext.sp_NS_BangTongHopCongThang(thang, nam, qSearch, "", 0, maPhongBan, null).ToList();
                var stt = 0;
                int dem = 0;

                foreach (var item1 in data)
                {
                    dem = 0;

                    stt++;
                    idRowStart++;

                    rowC = sheet.CreateRow(idRowStart);
                    ReportHelperExcel.SetAlignment(rowC, dem++, stt.ToString(), hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.hoTen, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.maNhanVien, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.maChamCong, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.phongBan, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.chucVu, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.congChuan, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.ngayQuet, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.congTac, hStyleConRight);

                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.nghiPhep, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.nghiPhepKhongLuong, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.nghiLe, hStyleConRight);

                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.luyKeThangTruoc, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.tongCong, hStyleConRight);
                    if (GetUser().manv == "BGD001" || GetUser().manv == "NV-1706004")
                    {
                        double tongTienComTrua = (double?)nhanSuContext.tbl_NS_BangChiTietChamCongs.Where(d => d.maNhanVien == item1.maNhanVien && d.thang == item1.thang && d.nam == item1.nam).Sum(d => d.tienComTrua) ?? 0;
                        string kiemTraCT = System.Configuration.ConfigurationManager.AppSettings["FlagCongTac"].ToString();
                        if (item1.thang >= 3 && kiemTraCT == "1")
                        {
                            tongTienComTrua = tongTienComTrua - ((item1.congTac ?? 0) * 40000);
                        }
                        ReportHelperExcel.SetAlignment(rowC, dem++, tongTienComTrua, hStyleConRight);
                    }
                }


                idRowStart = idRowStart + 2;
                var date = DateTime.Now.Day;
                string cellFooterNgayLap = "Tp.Hồ Chí Minh, ngày " + date + " tháng " + thang + " năm " + nam;
                var titleCellFooterNgayLap = HSSFCellUtil.CreateCell(sheet.CreateRow(idRowStart), 8, cellFooterNgayLap);
                titleCellFooterNgayLap.CellStyle = styleTitleItalic;

                //idRowStart = idRowStart + 2;
                //string cellFooterPTC = "PHÒNG TỔ CHỨC CB-LĐ";
                //var titleCellFooterPTC = HSSFCellUtil.CreateCell(sheet.CreateRow(idRowStart), 1, cellFooterPTC);
                //titleCellFooterPTC.CellStyle = styleTitle;

                //string cellFooterKT = "PHÒNG TÀI CHÍNH KẾ TOÁN";
                //var titleCellFooterKT = HSSFCellUtil.CreateCell(sheet.GetRow(idRowStart), 7, cellFooterKT);
                //titleCellFooterKT.CellStyle = styleTitle;

                //string cellFooterTGD = "TỔNG GIÁM ĐỐC";
                //var titleCellFooterTGD = HSSFCellUtil.CreateCell(sheet.GetRow(idRowStart), 14, cellFooterTGD);
                //titleCellFooterTGD.CellStyle = styleTitle;


                var stream = new MemoryStream();
                workbook.Write(stream);

                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.Clear();

                Response.BinaryWrite(stream.GetBuffer());
                Response.End();
            }
            catch
            {

            }
        }
        #endregion

        public ActionResult XemBangChamCongTongHop()
        {

            #region Role user
            permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
            if (!permission.HasValue)
                return View("LogIn");
            if (!permission.Value)
                return View("AccessDenied");
            #endregion


            thang(DateTime.Now.Month);
            nam(DateTime.Now.Year);
            return View("");
        }
        public ActionResult LoadBangChamCongTongHop(string qSearch, bool? tyLeBH, string maPhongBan, int thang, int nam, int _page = 0)
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
            int total = nhanSuContext.sp_NS_BangTongHopCongThang(thang, nam, qSearch, "", 0, maPhongBan, tyLeBH).Count();
            PagingLoaderFullController("/BangChamCongTongHop/LoadBangChamCongTongHop/", total, page, "?qsearch=" + qSearch + "&tyLeBH=" + tyLeBH);
            ViewData["lsDanhSach"] = nhanSuContext.sp_NS_BangTongHopCongThang(thang, nam, qSearch, "", 0, maPhongBan, tyLeBH).Skip(start).Take(1000).ToList();

            ViewData["qSearch"] = qSearch;
            ViewBag.QuyenSuaBaoHiem = QuyenSuaBaoHiem(GetUser().manv);
            ViewBag.tyLeBaoHiemBeHon14 = nhanSuContext.tbl_NS_TyLeDongBHs.Select(d => d.baoHiemYTeDoanhNghiep + d.baoHiemYTeNhanVien).FirstOrDefault() ?? 0;
            ViewBag.MaNhanVien = GetUser().manv;
            return PartialView("_LoadBangChamCongTongHop");
        }
        public ActionResult UpdateBangChamCongTH(int thang, int nam)
        {


            try
            {
                #region Role user
                permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
                if (!permission.HasValue)
                    return View("LogIn");
                if (!permission.Value)
                    return View("AccessDenied");
                #endregion

                var list = nhanSuContext.sp_NS_BangTongHopCongThang_TinhToan(thang, nam);


                var result = new { kq = true };
                SaveActiveHistory("Tính công tháng: " + thang + " năm: " + nam);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return View();
            }

        }
        public ActionResult UpdateBangChamCongChiTiet(int thang, int nam)
        {


            try
            {
                #region Role user
                permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
                if (!permission.HasValue)
                    return View("LogIn");
                if (!permission.Value)
                    return View("AccessDenied");
                #endregion
                SaveActiveHistory("Tính công chi tiết: " + thang + " năm: " + nam);
                var list = nhanSuContext.sp_Ns_CapNhatBangCongChiTiet(thang, nam);


                var result = new { kq = true };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return View();
            }

        }
        private void thang(int value)
        {
            Dictionary<int, string> dics = new Dictionary<int, string>();
            for (int i = 0; i < 13; i++)
            {
                dics[i] = i.ToString();
            }
            ViewData["thang"] = new SelectList(dics, "Key", "Value", value);
            ViewData["thangtc"] = new SelectList(dics, "Key", "Value", value);
        }
        private void nam(int value)
        {
            Dictionary<int, string> dics = new Dictionary<int, string>();
            for (int i = 2015; i < 2031; i++)
            {
                dics[i] = i.ToString();
            }
            ViewData["nam"] = new SelectList(dics, "Key", "Value", value);
            ViewData["namtc"] = new SelectList(dics, "Key", "Value", value);
        }

        #region Quyền sửa bảo hiểm
        public string QuyenSuaBaoHiem(string maNhanVien)
        {
            try
            {
                int countNhanSu = lqHeThong.sp_Sys_User_Index("SuaBaoHiem", null, null, null, null, null).Where(d => d.manv == maNhanVien).Count();
                if (countNhanSu > 0)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            catch
            {
                return "true";
            }
        }

        public ActionResult ChuyenTyleBaoHiem(string maNhanVien, int thang, int nam, decimal tyLeBaoHiem)
        {
            try
            {
                #region Role user
                permission = GetPermission(MCV, BangPhanQuyen.QuyenXem);
                if (!permission.HasValue)
                    return Json("LogIn");
                if (!permission.Value)
                    return Json("AccessDenied");
                #endregion

                //nhanSuContext
                tbl_NS_BangTongHopCongThang_TyLeDongBaoHiem_ChinhSuaLai data = new tbl_NS_BangTongHopCongThang_TyLeDongBaoHiem_ChinhSuaLai();

                data.maNhanVien = maNhanVien;
                data.createDay = DateTime.Now;
                data.createUser = GetUser().manv;
                data.nam = nam;
                data.thang = thang;

                data.tyLeDongBaoHiem = tyLeBaoHiem;

                //xóa rồi add lại
                nhanSuContext.tbl_NS_BangTongHopCongThang_TyLeDongBaoHiem_ChinhSuaLais.DeleteAllOnSubmit(
                    nhanSuContext.tbl_NS_BangTongHopCongThang_TyLeDongBaoHiem_ChinhSuaLais.Where(d => d.maNhanVien == maNhanVien && d.thang == thang && d.nam == nam));

                nhanSuContext.tbl_NS_BangTongHopCongThang_TyLeDongBaoHiem_ChinhSuaLais.InsertOnSubmit(data);

                nhanSuContext.SubmitChanges();

                return Json(new { messageError = string.Empty });
            }
            catch (Exception ex)
            {
                //write log
                Log4Net.WriteLog(log4net.Core.Level.Error, ex.Message);

                return Json(new { messageError = "Có lỗi xảy ra, vui lòng liên hệ bộ phận IT." });
            }
        }

        #endregion

        #region Import dữ liệu vân tay còn thiếu về hệ thống nhân sự
        public void ImportDuLieuVanTay(int? thang, int? nam, string maCongTrinh, int? TuNgay, int? DenNgay, string maMayChamCong, string qSearch)
        {


            try
            {
                TuNgay = 01;
                DenNgay = 31;
                thang = 10;
                nam = DateTime.Now.Year;

                DBChamCongDataContext linqChamCong = new DBChamCongDataContext();
                LinqNhanSuDataContext linqNhanSu = new LinqNhanSuDataContext();
                var lstVanTay_MayChamCong = linqChamCong.sp_LayChamCongConThieuVeERP(maCongTrinh, "OGT7070057071100035", qSearch, TuNgay, DenNgay, thang, nam).ToList();
                int soLuotImport = 0;

                foreach (var item in lstVanTay_MayChamCong)
                {
                    var checkTblChamCong_NhanSu = linqNhanSu.tbl_NS_ChamCongs.Where(d => d.idQuet == item.idQuet && d.checkTime.Value.Year == nam && d.checkTime.Value.Month == thang).FirstOrDefault();

                    if (checkTblChamCong_NhanSu == null)
                    {
                        tbl_NS_ChamCong tblChamCong = new tbl_NS_ChamCong();

                        tblChamCong.checkTime = item.checktime;

                        tblChamCong.maMayChamCong = item.SN;
                        tblChamCong.idQuet = item.idQuet;
                        tblChamCong.maChamCong = item.badgenumber;
                        linqNhanSu.tbl_NS_ChamCongs.InsertOnSubmit(tblChamCong);
                        linqNhanSu.SubmitChanges();
                        soLuotImport++;
                    }
                }

               //// SaveActiveHistory("Import dữ liệu vân tay: " + thang + " năm: " + nam + ". Import được: " + soLuotImport);
                var result = new { kq = true, soLuotImport = soLuotImport };

              //  return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                SaveActiveHistory("Import dữ liệu vân tay: " + thang + " năm: " + nam + " thật bại.");
                //write log
                Log4Net.WriteLog(log4net.Core.Level.Error, ex.Message);

                var result = new { kq = false };
               // return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Điều chỉnh công
        public JsonResult DieuChinhCong(string maNhanVienDieuChinh, int thangDieuChinh, int namDieuChinh, float? congDieuChinh)
        {
            try
            {
                var capNhatCongDC = nhanSuContext.tbl_NS_BangTongHopCongThangs.Where(d => d.maNhanVien == maNhanVienDieuChinh && d.thang == thangDieuChinh && d.nam == namDieuChinh).FirstOrDefault();
                capNhatCongDC.tongCong = Math.Round(congDieuChinh ?? 0, 2, MidpointRounding.ToEven);
                nhanSuContext.SubmitChanges();
                return Json(string.Empty);
            }
            catch
            {
                return Json("Error");
            }
        }
        #endregion

        #region Import file tổng hợp công

        public void XuatFileBangTongHopCongThangMau(string qSearch, string maPhongBan, int thang, int nam, int _page = 0)
        {
            try
            {
                var filename = "";
                var virtualPath = HttpRuntime.AppDomainAppVirtualPath;

                var fileStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(virtualPath + @"\Content\Report\ReportTemplateTHCT.xls"), FileMode.Open, FileAccess.Read);

                var workbook = new HSSFWorkbook(fileStream, true);

                filename += "BangTongHopCongThang_" + thang + "_" + nam + ".xls";

                #region format style excel cell
                /*style title start*/
                //tạo font cho các title
                //font tiêu đề 
                HSSFFont hFontTieuDe = (HSSFFont)workbook.CreateFont();
                hFontTieuDe.FontHeightInPoints = 11;
                hFontTieuDe.Boldweight = 100 * 10;
                hFontTieuDe.FontName = "Times New Roman";
                //hFontTieuDe.Color = HSSFColor.BLUE.index;

                //font tiêu đề 
                HSSFFont hFontTieuDeUnderline = (HSSFFont)workbook.CreateFont();
                hFontTieuDeUnderline.FontHeightInPoints = 11;
                hFontTieuDeUnderline.Boldweight = 100 * 10;
                hFontTieuDeUnderline.FontName = "Times New Roman";
                hFontTieuDeUnderline.Underline = 1;
                //hFontTieuDe.Color = HSSFColor.BLUE.index;


                HSSFFont hFontTieuDeItalic = (HSSFFont)workbook.CreateFont();
                hFontTieuDeItalic.FontHeightInPoints = 11;
                //hFontTieuDeItalic.Boldweight = 100 * 10;
                hFontTieuDeItalic.FontName = "Times New Roman";
                hFontTieuDeItalic.IsItalic = true;
                //hFontTieuDe.Color = HSSFColor.BLUE.index;


                HSSFFont hFontTieuDeLarge = (HSSFFont)workbook.CreateFont();
                hFontTieuDeLarge.FontHeightInPoints = 16;
                hFontTieuDeLarge.Boldweight = 100 * 10;
                hFontTieuDeLarge.FontName = "Times New Roman";
                //hFontTieuDeLarge.Color = HSSFColor.BLUE.index;

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

                //Set styleUnderline
                var styleTitleUnderline = workbook.CreateCellStyle();
                styleTitleUnderline.SetFont(hFontTieuDeUnderline);
                styleTitleUnderline.Alignment = HorizontalAlignment.LEFT;

                //Set style In nghiêng
                var styleTitleItalic = workbook.CreateCellStyle();
                styleTitleItalic.SetFont(hFontTieuDeItalic);
                styleTitleItalic.Alignment = HorizontalAlignment.LEFT;

                //Set style Large font
                var styleTitleLarge = workbook.CreateCellStyle();
                styleTitleLarge.SetFont(hFontTieuDeLarge);
                styleTitleLarge.Alignment = HorizontalAlignment.LEFT;

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

                //style sum cell
                var styleCellSumary = workbook.CreateCellStyle();
                styleCellSumary.SetFont(hFontNommalUpper);
                styleCellSumary.WrapText = true;
                styleCellSumary.BorderBottom = CellBorderType.THIN;
                styleCellSumary.BorderLeft = CellBorderType.THIN;
                styleCellSumary.BorderRight = CellBorderType.THIN;
                styleCellSumary.BorderTop = CellBorderType.THIN;
                styleCellSumary.VerticalAlignment = VerticalAlignment.CENTER;
                styleCellSumary.Alignment = HorizontalAlignment.RIGHT;

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
                #endregion

                //Khai báo row
                Row rowC = null;

                var sheet = workbook.CreateSheet("BangTongHopCongThang");

                //Khai báo row đầu tiên
                int firstRowNumber = 0;

                var list1 = new List<string>();
                list1.Add("STT");
                list1.Add("Họ tên");
                list1.Add("Mã nhân viên");
                list1.Add("Mã chấm công");
                list1.Add("Phòng ban");
                list1.Add("Chức vụ");
                list1.Add("Công chuẩn");
                list1.Add("Số ngày quét");
                list1.Add("Công tác");
                list1.Add("Nghỉ phép có lương");
                list1.Add("Nghỉ phép không lương");
                list1.Add("Nghỉ lễ");
                list1.Add("Lũy kế tháng trước");
                list1.Add("Tổng cộng");
                list1.Add("Ghi chú");


                var idRowStart = firstRowNumber; // bat dau o dong thu 4
                var headerRow = sheet.CreateRow(idRowStart);
                int rowend = idRowStart;
                ReportHelperExcel.CreateHeaderRow(headerRow, 0, styleheadedColumnTable, list1);
                idRowStart++;
                sheet.SetColumnWidth(0, 5 * 210);
                sheet.SetColumnWidth(1, 30 * 210);
                sheet.SetColumnWidth(2, 15 * 210);
                sheet.SetColumnWidth(3, 15 * 210);
                sheet.SetColumnWidth(4, 30 * 210);
                sheet.SetColumnWidth(5, 15 * 210);
                sheet.SetColumnWidth(6, 15 * 210);
                sheet.SetColumnWidth(7, 15 * 210);
                sheet.SetColumnWidth(8, 15 * 210);
                sheet.SetColumnWidth(9, 15 * 210);
                sheet.SetColumnWidth(10, 15 * 210);
                sheet.SetColumnWidth(11, 15 * 210);
                sheet.SetColumnWidth(12, 15 * 400);
                sheet.SetColumnWidth(13, 15 * 300);
                sheet.SetColumnWidth(14, 15 * 300);

                var data = nhanSuContext.sp_NS_BangTongHopCongThang(thang, nam, qSearch, "", 0, maPhongBan, null).ToList();
                var stt = 0;
                int dem = 0;

                foreach (var item1 in data)
                {
                    dem = 0;

                    stt++;
                    idRowStart++;

                    rowC = sheet.CreateRow(idRowStart);
                    ReportHelperExcel.SetAlignment(rowC, dem++, stt.ToString(), hStyleConCenter);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.hoTen, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.maNhanVien, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.maChamCong, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.phongBan, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.chucVu, hStyleConLeft);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.congChuan, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.ngayQuet, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.congTac, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, (item1.nghiPhep ?? 0) + (item1.nghiBu ?? 0), hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.nghiPhepKhongLuong, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.nghiLe, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.luyKeThangTruoc, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.tongCong, hStyleConRight);
                    ReportHelperExcel.SetAlignment(rowC, dem++, item1.ghiChu, hStyleConRight);

                }

                var stream = new MemoryStream();
                workbook.Write(stream);

                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.Clear();

                Response.BinaryWrite(stream.GetBuffer());
                Response.End();
            }
            catch
            {

            }
        }

        public ActionResult ImportFileTC(int? thang, int? nam)
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
                        DataTable dt = excelDataProcessor.GetDataTableWorkSheet("BangTongHopCongThang");
                        List<tbl_NS_BangTongHopCongThang> listTHC = new List<tbl_NS_BangTongHopCongThang>();
                        tbl_NS_BangTongHopCongThang congTH;
                        foreach (DataRow row in dt.Rows)
                        {

                            if (!String.IsNullOrEmpty(row["Mã nhân viên"].ToString()))
                            {
                                var tongHopCong = nhanSuContext.tbl_NS_BangTongHopCongThangs.Where(d => d.thang == thang && d.nam == nam && d.maNhanVien.Trim().ToLower() == row["Mã nhân viên"].ToString().Trim().ToLower()).FirstOrDefault();
                                if (tongHopCong != null)
                                {
                                    tongHopCong.congChuan = string.IsNullOrEmpty(row["Công chuẩn"].ToString()) ? 0 : Convert.ToDouble(row["Công chuẩn"].ToString());
                                    tongHopCong.ngayQuet = string.IsNullOrEmpty(row["Số ngày quét"].ToString()) ? 0 : Convert.ToDouble(row["Số ngày quét"].ToString());
                                    tongHopCong.congTac = string.IsNullOrEmpty(row["Công tác"].ToString()) ? 0 : Convert.ToDouble(row["Công tác"].ToString());
                                    tongHopCong.nghiPhep = string.IsNullOrEmpty(row["Nghỉ phép có lương"].ToString()) ? 0 : Convert.ToDouble(row["Nghỉ phép có lương"].ToString());
                                    tongHopCong.nghiPhepKhongLuong = string.IsNullOrEmpty(row["Nghỉ phép không lương"].ToString()) ? 0 : Convert.ToDouble(row["Nghỉ phép không lương"].ToString());
                                    tongHopCong.nghiLe = string.IsNullOrEmpty(row["Nghỉ lễ"].ToString()) ? 0 : Convert.ToDouble(row["Nghỉ lễ"].ToString());
                                    tongHopCong.luyKeThangTruoc = string.IsNullOrEmpty(row["Lũy kế tháng trước"].ToString()) ? 0 : Convert.ToDouble(row["Lũy kế tháng trước"].ToString());
                                    tongHopCong.tongCong = string.IsNullOrEmpty(row["Tổng cộng"].ToString()) ? 0 : Math.Round(Convert.ToDouble(row["Tổng cộng"].ToString()), 2, MidpointRounding.ToEven);
                                    nhanSuContext.SubmitChanges();
                                }
                                else
                                {
                                    congTH = new tbl_NS_BangTongHopCongThang();
                                    congTH.thang = thang ?? DateTime.Now.Month;
                                    congTH.nam = nam ?? DateTime.Now.Year;
                                    congTH.congChuan = string.IsNullOrEmpty(row["Công chuẩn"].ToString()) ? 0 : Convert.ToDouble(row["Công chuẩn"].ToString());
                                    congTH.ngayQuet = string.IsNullOrEmpty(row["Số ngày quét"].ToString()) ? 0 : Convert.ToDouble(row["Số ngày quét"].ToString());
                                    congTH.congTac = string.IsNullOrEmpty(row["Công tác"].ToString()) ? 0 : Convert.ToDouble(row["Công tác"].ToString());
                                    congTH.nghiPhep = string.IsNullOrEmpty(row["Nghỉ phép có lương"].ToString()) ? 0 : Convert.ToDouble(row["Nghỉ phép có lương"].ToString());
                                    congTH.nghiPhepKhongLuong = string.IsNullOrEmpty(row["Nghỉ phép không lương"].ToString()) ? 0 : Convert.ToDouble(row["Nghỉ phép không lương"].ToString());
                                    congTH.nghiLe = string.IsNullOrEmpty(row["Nghỉ lễ"].ToString()) ? 0 : Convert.ToDouble(row["Nghỉ lễ"].ToString());
                                    congTH.luyKeThangTruoc = string.IsNullOrEmpty(row["Lũy kế tháng trước"].ToString()) ? 0 : Convert.ToDouble(row["Lũy kế tháng trước"].ToString());
                                    congTH.tongCong = string.IsNullOrEmpty(row["Tổng cộng"].ToString()) ? 0 : Math.Round(Convert.ToDouble(row["Tổng cộng"].ToString()), 2, MidpointRounding.ToEven);
                                    congTH.ghiChu = row["Ghi chú"].ToString().Trim();
                                    congTH.hoTen = row["Họ tên"].ToString().Trim();
                                    congTH.maNhanVien = row["Mã nhân viên"].ToString().Trim();
                                    congTH.maChamCong = row["Mã chấm công"].ToString().Trim();
                                    congTH.phongBan = row["Phòng ban"].ToString().Trim();
                                    congTH.chucVu = row["Chức vụ"].ToString().Trim();
                                    listTHC.Add(congTH);
                                }
                            }
                        }
                        if (listTHC != null && listTHC.Count > 0)
                        {
                            nhanSuContext.tbl_NS_BangTongHopCongThangs.InsertAllOnSubmit(listTHC);
                            nhanSuContext.SubmitChanges();
                        }
                        // System.IO.File.Delete(Server.MapPath("/UploadFiles/NhanVien/" + fileName));
                    }
                }
                SaveActiveHistory("Import file điều chỉnh công tổng cộng tháng " + thang + " năm " + nam);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
        #endregion
    }
}
