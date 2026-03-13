from django.shortcuts import render, get_object_or_404, redirect
from django.contrib.auth.decorators import login_required
from django.contrib import messages
from datetime import date
from .models import AttendanceRecord
from .forms import AttendanceForm, AttendanceFilterForm


@login_required
def attendance_list(request):
    form = AttendanceFilterForm(request.GET)
    records = AttendanceRecord.objects.select_related('employee').all()

    today = date.today()
    month = today.month
    year = today.year

    if form.is_valid():
        if form.cleaned_data.get('month'):
            month = form.cleaned_data['month']
        if form.cleaned_data.get('year'):
            year = form.cleaned_data['year']
        if form.cleaned_data.get('employee'):
            records = records.filter(employee_id=form.cleaned_data['employee'])

    records = records.filter(date__month=month, date__year=year).order_by('-date', 'employee__last_name')

    return render(request, 'attendance/attendance_list.html', {
        'records': records,
        'form': form,
        'month': month,
        'year': year,
    })


@login_required
def attendance_create(request):
    if request.method == 'POST':
        form = AttendanceForm(request.POST)
        if form.is_valid():
            form.save()
            messages.success(request, 'Thêm chấm công thành công!')
            return redirect('attendance_list')
    else:
        form = AttendanceForm(initial={'date': date.today()})
    return render(request, 'attendance/attendance_form.html', {'form': form, 'title': 'Thêm chấm công'})


@login_required
def attendance_update(request, pk):
    record = get_object_or_404(AttendanceRecord, pk=pk)
    if request.method == 'POST':
        form = AttendanceForm(request.POST, instance=record)
        if form.is_valid():
            form.save()
            messages.success(request, 'Cập nhật chấm công thành công!')
            return redirect('attendance_list')
    else:
        form = AttendanceForm(instance=record)
    return render(request, 'attendance/attendance_form.html', {
        'form': form,
        'title': 'Cập nhật chấm công',
        'record': record,
    })


@login_required
def attendance_delete(request, pk):
    record = get_object_or_404(AttendanceRecord, pk=pk)
    if request.method == 'POST':
        record.delete()
        messages.success(request, 'Xóa chấm công thành công!')
        return redirect('attendance_list')
    return render(request, 'attendance/attendance_confirm_delete.html', {'record': record})
