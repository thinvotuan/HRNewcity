from django.contrib import admin
from .models import Department


@admin.register(Department)
class DepartmentAdmin(admin.ModelAdmin):
    list_display = ['code', 'name', 'manager', 'employee_count']
    search_fields = ['name', 'code']

    def employee_count(self, obj):
        return obj.employees.count()
    employee_count.short_description = 'Số nhân viên'
