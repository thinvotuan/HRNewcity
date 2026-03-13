using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatDongSan.Models.PhieuDeNghi
{
    public class PhieuKeKhaiYTeModel
    {
        public string MaPhieu { get; set; }

        public string MaNhanVienLapPhieu { get; set; }

        public string TenNhanVienLapPhieu { get; set; }

        public string MaNhanVienKk { get; set; }

        public string TenNhanVienKK { get; set; }

        public DateTime? NgayLap { get; set; }

        public bool? TrangThaiLamViecTuXa { get; set; }

        public bool? TrangThaiTiepXuc { get; set; }

        public DateTime? NgayTiepXuc { get; set; }

        public bool? TrangThaiPhongToaTaiKhuVucMinhSong { get; set; }

        public DateTime? TuNgayPhongToa { get; set; }

        public DateTime? DenNgayPhongToa { get; set; }

        public bool? TrangThaiPhongToaGan { get; set; }

        public string GhiChuPhongToaGan { get; set; }

        public bool? TrangThaiLayMauXetNghiem { get; set; }

        public int SoLanXetNghiem { get; set; }

        public DateTime? NgayLayMau { get; set; }

        public bool? TrangThaiHo { get; set; }

        public bool? TrangThaiSotTren37 { get; set; }

        public bool? TrangThaiKhoTho { get; set; }

        public bool? TrangThaiDauHong { get; set; }

        public bool? TrangThaiMatViGiac { get; set; }

        public bool? TrangThaiMatKhuuGiac { get; set; }

        public bool? TrieuChungKhac { get; set; }

        public bool? GioiTinh { get; set; }

        public string DienThoai { get; set; }

        public string DiaChiHienNay { get; set; }

        public string MaTinhThanh { get; set; }

        public string TenTinhThanh { get; set; }

        public int MaQuanHuyen { get; set; }

        public string TenQuanHuyen { get; set; }

        public bool? TrangThaiNguoiKhai { get; set; }

        public bool? XacNhan { get; set; }

        public string TenPhuongXa { get; set; }

        public DateTime? TuNgayPhongToaGan { get; set; }

        public DateTime? DenNgayPhongToaGan { get; set; }
    }
}