from django.db import models
from employees.models import Employee


class AttendanceRecord(models.Model):
    STATUS_CHOICES = [
        ('present', 'Có mặt'),
        ('absent', 'Vắng mặt'),
        ('late', 'Đi muộn'),
        ('half_day', 'Nửa ngày'),
        ('remote', 'Làm từ xa'),
    ]

    employee = models.ForeignKey(
        Employee, on_delete=models.CASCADE, related_name='attendance_records', verbose_name='Nhân viên'
    )
    date = models.DateField(verbose_name='Ngày')
    check_in = models.TimeField(null=True, blank=True, verbose_name='Giờ vào')
    check_out = models.TimeField(null=True, blank=True, verbose_name='Giờ ra')
    status = models.CharField(
        max_length=20, choices=STATUS_CHOICES, default='present', verbose_name='Trạng thái'
    )
    note = models.TextField(blank=True, verbose_name='Ghi chú')
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    class Meta:
        verbose_name = 'Chấm công'
        verbose_name_plural = 'Chấm công'
        ordering = ['-date', 'employee']
        unique_together = ['employee', 'date']

    def __str__(self):
        return f'{self.employee.full_name} - {self.date}'

    @property
    def work_hours(self):
        if self.check_in and self.check_out:
            from datetime import datetime, date as date_type, time as time_type
            def to_time(val):
                if isinstance(val, time_type):
                    return val
                s = str(val)
                fmt = '%H:%M:%S' if s.count(':') == 2 else '%H:%M'
                return datetime.strptime(s, fmt).time()
            dt_in = datetime.combine(date_type.today(), to_time(self.check_in))
            dt_out = datetime.combine(date_type.today(), to_time(self.check_out))
            delta = dt_out - dt_in
            return round(delta.total_seconds() / 3600, 2)
        return 0
