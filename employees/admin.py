from django.contrib import admin
from .models import Employee


@admin.register(Employee)
class EmployeeAdmin(admin.ModelAdmin):
    list_display = ['employee_id', 'full_name', 'department', 'position', 'status', 'hire_date']
    list_filter = ['department', 'status', 'gender']
    search_fields = ['employee_id', 'first_name', 'last_name', 'email', 'phone']
    date_hierarchy = 'hire_date'
