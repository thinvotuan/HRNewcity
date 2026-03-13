# HRNewcity - Hệ thống quản lý nhân sự

HRNewcity là hệ thống quản lý nhân sự toàn diện xây dựng trên nền tảng Django, cung cấp các chức năng quản lý nhân viên, phòng ban, chấm công và nghỉ phép.

## Tính năng

- **Quản lý nhân viên**: Thêm, sửa, xóa, tìm kiếm nhân viên. Xem hồ sơ chi tiết bao gồm thông tin cá nhân, lịch sử chấm công và đơn nghỉ phép.
- **Quản lý phòng ban**: Tổ chức nhân viên theo phòng ban, gán trưởng phòng.
- **Chấm công**: Theo dõi giờ vào/ra, trạng thái làm việc (có mặt, vắng mặt, đi muộn, làm từ xa). Lọc theo tháng/năm và nhân viên.
- **Nghỉ phép**: Quản lý đơn xin nghỉ phép, duyệt/từ chối đơn, cấu hình các loại nghỉ phép.
- **Bảng điều khiển**: Tổng quan số liệu nhân sự theo thời gian thực.
- **Xác thực**: Đăng nhập/đăng xuất, bảo mật với login_required.

## Yêu cầu

- Python 3.10+
- Django 6.0+
- Pillow (cho ảnh nhân viên)

## Cài đặt và chạy

```bash
# Clone repository
git clone <repo-url>
cd HRNewcity

# Cài đặt thư viện
pip install -r requirements.txt

# Tạo database và chạy migration
python manage.py migrate

# Tạo dữ liệu mẫu và tài khoản admin
python seed_data.py

# Chạy server
python manage.py runserver
```

Mở trình duyệt tại: http://localhost:8000

**Tài khoản mặc định:** `admin` / `admin123`

## Cấu trúc dự án

```
HRNewcity/
├── hrnewcity/          # Project settings & URLs
├── employees/          # Quản lý nhân viên
├── departments/        # Quản lý phòng ban
├── attendance/         # Chấm công
├── leaves/             # Nghỉ phép
├── templates/          # HTML templates
├── static/             # Static files
├── seed_data.py        # Dữ liệu mẫu
├── tests.py            # Unit tests
└── requirements.txt
```

## Chạy tests

```bash
python manage.py test tests
```
