from django.shortcuts import render
from django.contrib.auth.decorators import login_required
from datetime import date


@login_required
def dashboard(request):
    from employees.models import Employee
    from departments.models import Department
    from attendance.models import AttendanceRecord
    from leaves.models import LeaveRequest

    today = date.today()
    context = {
        'total_employees': Employee.objects.filter(status='active').count(),
        'total_departments': Department.objects.count(),
        'today_attendance': AttendanceRecord.objects.filter(date=today).count(),
        'pending_leaves': LeaveRequest.objects.filter(status='pending').count(),
        'recent_employees': Employee.objects.select_related('department').order_by('-created_at')[:5],
        'recent_leaves': LeaveRequest.objects.select_related('employee', 'leave_type').filter(
            status='pending'
        ).order_by('-created_at')[:5],
    }
    return render(request, 'dashboard.html', context)
