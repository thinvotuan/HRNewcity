from django.shortcuts import render, get_object_or_404, redirect
from django.contrib.auth.decorators import login_required
from django.contrib import messages
from .models import Department
from .forms import DepartmentForm


@login_required
def department_list(request):
    departments = Department.objects.prefetch_related('employees').all()
    return render(request, 'departments/department_list.html', {'departments': departments})


@login_required
def department_detail(request, pk):
    department = get_object_or_404(Department, pk=pk)
    employees = department.employees.all()
    return render(request, 'departments/department_detail.html', {
        'department': department,
        'employees': employees,
    })


@login_required
def department_create(request):
    if request.method == 'POST':
        form = DepartmentForm(request.POST)
        if form.is_valid():
            form.save()
            messages.success(request, 'Thêm phòng ban thành công!')
            return redirect('department_list')
    else:
        form = DepartmentForm()
    return render(request, 'departments/department_form.html', {'form': form, 'title': 'Thêm phòng ban'})


@login_required
def department_update(request, pk):
    department = get_object_or_404(Department, pk=pk)
    if request.method == 'POST':
        form = DepartmentForm(request.POST, instance=department)
        if form.is_valid():
            form.save()
            messages.success(request, 'Cập nhật phòng ban thành công!')
            return redirect('department_list')
    else:
        form = DepartmentForm(instance=department)
    return render(request, 'departments/department_form.html', {
        'form': form,
        'title': 'Cập nhật phòng ban',
        'department': department,
    })


@login_required
def department_delete(request, pk):
    department = get_object_or_404(Department, pk=pk)
    if request.method == 'POST':
        department.delete()
        messages.success(request, 'Xóa phòng ban thành công!')
        return redirect('department_list')
    return render(request, 'departments/department_confirm_delete.html', {'department': department})
