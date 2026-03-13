from django.shortcuts import render, get_object_or_404, redirect
from django.contrib.auth.decorators import login_required
from django.contrib import messages
from django.db.models import Q
from .models import Employee
from .forms import EmployeeForm, EmployeeSearchForm


@login_required
def employee_list(request):
    form = EmployeeSearchForm(request.GET)
    employees = Employee.objects.select_related('department').all()

    if form.is_valid():
        query = form.cleaned_data.get('query')
        department = form.cleaned_data.get('department')
        status = form.cleaned_data.get('status')

        if query:
            employees = employees.filter(
                Q(first_name__icontains=query) |
                Q(last_name__icontains=query) |
                Q(employee_id__icontains=query) |
                Q(email__icontains=query) |
                Q(phone__icontains=query)
            )
        if department:
            employees = employees.filter(department_id=department)
        if status:
            employees = employees.filter(status=status)

    return render(request, 'employees/employee_list.html', {
        'employees': employees,
        'form': form,
    })


@login_required
def employee_detail(request, pk):
    employee = get_object_or_404(Employee, pk=pk)
    recent_attendance = employee.attendance_records.order_by('-date')[:10]
    recent_leaves = employee.leave_requests.order_by('-created_at')[:5]
    return render(request, 'employees/employee_detail.html', {
        'employee': employee,
        'recent_attendance': recent_attendance,
        'recent_leaves': recent_leaves,
    })


@login_required
def employee_create(request):
    if request.method == 'POST':
        form = EmployeeForm(request.POST, request.FILES)
        if form.is_valid():
            form.save()
            messages.success(request, 'Thêm nhân viên thành công!')
            return redirect('employee_list')
    else:
        form = EmployeeForm()
    return render(request, 'employees/employee_form.html', {'form': form, 'title': 'Thêm nhân viên'})


@login_required
def employee_update(request, pk):
    employee = get_object_or_404(Employee, pk=pk)
    if request.method == 'POST':
        form = EmployeeForm(request.POST, request.FILES, instance=employee)
        if form.is_valid():
            form.save()
            messages.success(request, 'Cập nhật nhân viên thành công!')
            return redirect('employee_detail', pk=employee.pk)
    else:
        form = EmployeeForm(instance=employee)
    return render(request, 'employees/employee_form.html', {
        'form': form,
        'title': 'Cập nhật nhân viên',
        'employee': employee,
    })


@login_required
def employee_delete(request, pk):
    employee = get_object_or_404(Employee, pk=pk)
    if request.method == 'POST':
        employee.delete()
        messages.success(request, 'Xóa nhân viên thành công!')
        return redirect('employee_list')
    return render(request, 'employees/employee_confirm_delete.html', {'employee': employee})
