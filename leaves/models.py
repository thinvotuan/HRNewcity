from django.db import models
from employees.models import Employee


class LeaveType(models.Model):
    name = models.CharField(max_length=100, verbose_name='Loại nghỉ phép')
    annual_days = models.PositiveIntegerField(default=0, verbose_name='Số ngày phép/năm')
    is_paid = models.BooleanField(default=True, verbose_name='Nghỉ có lương')
    description = models.TextField(blank=True, verbose_name='Mô tả')

    class Meta:
        verbose_name = 'Loại nghỉ phép'
        verbose_name_plural = 'Loại nghỉ phép'

    def __str__(self):
        return self.name


class LeaveRequest(models.Model):
    STATUS_CHOICES = [
        ('pending', 'Chờ duyệt'),
        ('approved', 'Đã duyệt'),
        ('rejected', 'Từ chối'),
        ('cancelled', 'Đã hủy'),
    ]

    employee = models.ForeignKey(
        Employee, on_delete=models.CASCADE, related_name='leave_requests', verbose_name='Nhân viên'
    )
    leave_type = models.ForeignKey(
        LeaveType, on_delete=models.PROTECT, verbose_name='Loại nghỉ phép'
    )
    start_date = models.DateField(verbose_name='Ngày bắt đầu')
    end_date = models.DateField(verbose_name='Ngày kết thúc')
    reason = models.TextField(verbose_name='Lý do')
    status = models.CharField(
        max_length=20, choices=STATUS_CHOICES, default='pending', verbose_name='Trạng thái'
    )
    approved_by = models.ForeignKey(
        Employee,
        on_delete=models.SET_NULL,
        null=True,
        blank=True,
        related_name='approved_leaves',
        verbose_name='Người duyệt',
    )
    approved_at = models.DateTimeField(null=True, blank=True, verbose_name='Ngày duyệt')
    rejection_reason = models.TextField(blank=True, verbose_name='Lý do từ chối')
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    class Meta:
        verbose_name = 'Đơn xin nghỉ phép'
        verbose_name_plural = 'Đơn xin nghỉ phép'
        ordering = ['-created_at']

    def __str__(self):
        return f'{self.employee.full_name} - {self.leave_type} ({self.start_date} đến {self.end_date})'

    @property
    def days_count(self):
        if self.start_date and self.end_date:
            delta = self.end_date - self.start_date
            return delta.days + 1
        return 0
