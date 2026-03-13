"""
Seed data script for HRNewcity system.
Run with: python manage.py shell < seed_data.py
"""
import os
import django
os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'hrnewcity.settings')
django.setup()

from django.contrib.auth.models import User
from departments.models import Department
from employees.models import Employee
from leaves.models import LeaveType
from attendance.models import AttendanceRecord
from datetime import date, time

# Create superuser
if not User.objects.filter(username='admin').exists():
    User.objects.create_superuser('admin', 'admin@hrnewcity.vn', 'admin123')
    print('Superuser created: admin / admin123')

# Create departments
depts_data = [
    ('BGĐ', 'Ban Giám đốc', 'Lãnh đạo và quản lý cấp cao công ty'),
    ('CNTT', 'Phòng Công nghệ thông tin', 'Phát triển và vận hành hệ thống IT'),
    ('KD', 'Phòng Kinh doanh', 'Phát triển kinh doanh và bán hàng'),
    ('KT', 'Phòng Kế toán', 'Quản lý tài chính và kế toán'),
    ('NS', 'Phòng Nhân sự', 'Quản lý nguồn nhân lực'),
    ('HC', 'Phòng Hành chính', 'Quản lý hành chính và văn phòng'),
]

departments = {}
for code, name, desc in depts_data:
    dept, _ = Department.objects.get_or_create(code=code, defaults={'name': name, 'description': desc})
    departments[code] = dept
    print(f'Department: {dept}')

# Create employees
employees_data = [
    ('NV001', 'Nguyễn', 'Văn An', 'M', date(1980, 5, 15), '0901234567', 'nguyen.van.an@hrnewcity.vn', 'BGĐ', 'Giám đốc', date(2015, 1, 1), 'active', 50000000),
    ('NV002', 'Trần', 'Thị Bình', 'F', date(1985, 8, 20), '0912345678', 'tran.thi.binh@hrnewcity.vn', 'NS', 'Trưởng phòng Nhân sự', date(2016, 3, 1), 'active', 25000000),
    ('NV003', 'Lê', 'Hoàng Cường', 'M', date(1990, 3, 10), '0923456789', 'le.hoang.cuong@hrnewcity.vn', 'CNTT', 'Lập trình viên Senior', date(2018, 6, 15), 'active', 20000000),
    ('NV004', 'Phạm', 'Thị Dung', 'F', date(1992, 11, 25), '0934567890', 'pham.thi.dung@hrnewcity.vn', 'KT', 'Kế toán viên', date(2019, 9, 1), 'active', 15000000),
    ('NV005', 'Hoàng', 'Văn Em', 'M', date(1988, 7, 8), '0945678901', 'hoang.van.em@hrnewcity.vn', 'KD', 'Trưởng phòng Kinh doanh', date(2017, 4, 1), 'active', 22000000),
    ('NV006', 'Võ', 'Thị Phương', 'F', date(1995, 2, 14), '0956789012', 'vo.thi.phuong@hrnewcity.vn', 'CNTT', 'Lập trình viên', date(2020, 1, 6), 'active', 16000000),
    ('NV007', 'Đỗ', 'Minh Quân', 'M', date(1993, 6, 30), '0967890123', 'do.minh.quan@hrnewcity.vn', 'KD', 'Nhân viên kinh doanh', date(2021, 3, 15), 'active', 12000000),
    ('NV008', 'Bùi', 'Thị Hoa', 'F', date(1997, 9, 5), '0978901234', 'bui.thi.hoa@hrnewcity.vn', 'HC', 'Nhân viên hành chính', date(2022, 7, 1), 'probation', 10000000),
]

employees = {}
for emp_id, last, first, gender, dob, phone, email, dept_code, position, hire_date, status, salary in employees_data:
    emp, _ = Employee.objects.get_or_create(
        employee_id=emp_id,
        defaults={
            'first_name': first,
            'last_name': last,
            'gender': gender,
            'date_of_birth': dob,
            'phone': phone,
            'email': email,
            'department': departments[dept_code],
            'position': position,
            'hire_date': hire_date,
            'status': status,
            'basic_salary': salary,
        }
    )
    employees[emp_id] = emp
    print(f'Employee: {emp}')

# Set department managers
departments['BGĐ'].manager = employees['NV001']
departments['BGĐ'].save()
departments['NS'].manager = employees['NV002']
departments['NS'].save()
departments['KD'].manager = employees['NV005']
departments['KD'].save()

# Create leave types
leave_types_data = [
    ('Nghỉ phép năm', 12, True),
    ('Nghỉ ốm', 15, True),
    ('Nghỉ thai sản', 180, True),
    ('Nghỉ không lương', 0, False),
    ('Nghỉ lễ tết', 10, True),
]

for name, days, paid in leave_types_data:
    lt, _ = LeaveType.objects.get_or_create(name=name, defaults={'annual_days': days, 'is_paid': paid})
    print(f'Leave type: {lt}')

# Create attendance records for today
today = date.today()
for emp_id, emp in list(employees.items())[:5]:
    rec, _ = AttendanceRecord.objects.get_or_create(
        employee=emp,
        date=today,
        defaults={
            'check_in': time(8, 0),
            'check_out': time(17, 0),
            'status': 'present',
        }
    )
    print(f'Attendance: {rec}')

print('\nSeed data created successfully!')
print('Login: admin / admin123')
