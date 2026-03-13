from django.db import models
from django.contrib.auth.models import User


class Employee(models.Model):
    GENDER_CHOICES = [
        ('M', 'Nam'),
        ('F', 'Nữ'),
        ('O', 'Khác'),
    ]
    STATUS_CHOICES = [
        ('active', 'Đang làm việc'),
        ('inactive', 'Đã nghỉ việc'),
        ('probation', 'Thử việc'),
    ]

    employee_id = models.CharField(max_length=20, unique=True, verbose_name='Mã nhân viên')
    user = models.OneToOneField(
        User, on_delete=models.SET_NULL, null=True, blank=True, verbose_name='Tài khoản'
    )
    first_name = models.CharField(max_length=100, verbose_name='Tên')
    last_name = models.CharField(max_length=100, verbose_name='Họ')
    gender = models.CharField(max_length=1, choices=GENDER_CHOICES, verbose_name='Giới tính')
    date_of_birth = models.DateField(verbose_name='Ngày sinh')
    national_id = models.CharField(max_length=20, blank=True, verbose_name='CMND/CCCD')
    phone = models.CharField(max_length=20, verbose_name='Số điện thoại')
    email = models.EmailField(verbose_name='Email')
    address = models.TextField(blank=True, verbose_name='Địa chỉ')
    photo = models.ImageField(upload_to='employees/', blank=True, null=True, verbose_name='Ảnh')

    department = models.ForeignKey(
        'departments.Department',
        on_delete=models.SET_NULL,
        null=True,
        blank=True,
        related_name='employees',
        verbose_name='Phòng ban',
    )
    position = models.CharField(max_length=200, verbose_name='Chức vụ')
    hire_date = models.DateField(verbose_name='Ngày vào làm')
    end_date = models.DateField(null=True, blank=True, verbose_name='Ngày nghỉ việc')
    status = models.CharField(
        max_length=20, choices=STATUS_CHOICES, default='active', verbose_name='Trạng thái'
    )
    basic_salary = models.DecimalField(
        max_digits=12, decimal_places=0, default=0, verbose_name='Lương cơ bản'
    )

    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    class Meta:
        verbose_name = 'Nhân viên'
        verbose_name_plural = 'Nhân viên'
        ordering = ['last_name', 'first_name']

    def __str__(self):
        return f'{self.employee_id} - {self.full_name}'

    @property
    def full_name(self):
        return f'{self.last_name} {self.first_name}'
