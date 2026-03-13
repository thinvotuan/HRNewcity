from django.db import models


class Department(models.Model):
    name = models.CharField(max_length=200, verbose_name='Tên phòng ban')
    code = models.CharField(max_length=20, unique=True, verbose_name='Mã phòng ban')
    description = models.TextField(blank=True, verbose_name='Mô tả')
    manager = models.ForeignKey(
        'employees.Employee',
        on_delete=models.SET_NULL,
        null=True,
        blank=True,
        related_name='managed_department',
        verbose_name='Trưởng phòng',
    )
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    class Meta:
        verbose_name = 'Phòng ban'
        verbose_name_plural = 'Phòng ban'
        ordering = ['name']

    def __str__(self):
        return f'{self.code} - {self.name}'
