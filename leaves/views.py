from django.shortcuts import render, get_object_or_404, redirect
from django.contrib.auth.decorators import login_required
from django.contrib import messages
from django.utils import timezone
from .models import LeaveRequest, LeaveType
from .forms import LeaveRequestForm, LeaveTypeForm, LeaveApprovalForm


@login_required
def leave_request_list(request):
    status_filter = request.GET.get('status', '')
    requests = LeaveRequest.objects.select_related('employee', 'leave_type').all()
    if status_filter:
        requests = requests.filter(status=status_filter)
    return render(request, 'leaves/leave_request_list.html', {
        'requests': requests,
        'status_filter': status_filter,
        'status_choices': LeaveRequest.STATUS_CHOICES,
    })


@login_required
def leave_request_detail(request, pk):
    leave_request = get_object_or_404(LeaveRequest, pk=pk)
    approval_form = LeaveApprovalForm(instance=leave_request)
    return render(request, 'leaves/leave_request_detail.html', {
        'leave_request': leave_request,
        'approval_form': approval_form,
    })


@login_required
def leave_request_create(request):
    if request.method == 'POST':
        form = LeaveRequestForm(request.POST)
        if form.is_valid():
            form.save()
            messages.success(request, 'Gửi đơn xin nghỉ phép thành công!')
            return redirect('leave_request_list')
    else:
        form = LeaveRequestForm()
    return render(request, 'leaves/leave_request_form.html', {'form': form, 'title': 'Xin nghỉ phép'})


@login_required
def leave_request_update(request, pk):
    leave_request = get_object_or_404(LeaveRequest, pk=pk)
    if leave_request.status != 'pending':
        messages.error(request, 'Chỉ có thể chỉnh sửa đơn đang chờ duyệt.')
        return redirect('leave_request_detail', pk=pk)
    if request.method == 'POST':
        form = LeaveRequestForm(request.POST, instance=leave_request)
        if form.is_valid():
            form.save()
            messages.success(request, 'Cập nhật đơn xin nghỉ phép thành công!')
            return redirect('leave_request_detail', pk=pk)
    else:
        form = LeaveRequestForm(instance=leave_request)
    return render(request, 'leaves/leave_request_form.html', {
        'form': form,
        'title': 'Cập nhật đơn xin nghỉ phép',
        'leave_request': leave_request,
    })


@login_required
def leave_request_approve(request, pk):
    leave_request = get_object_or_404(LeaveRequest, pk=pk)
    if request.method == 'POST':
        form = LeaveApprovalForm(request.POST, instance=leave_request)
        if form.is_valid():
            obj = form.save(commit=False)
            if obj.status in ('approved', 'rejected'):
                obj.approved_at = timezone.now()
            obj.save()
            messages.success(request, 'Cập nhật trạng thái đơn xin nghỉ phép thành công!')
    return redirect('leave_request_detail', pk=pk)


@login_required
def leave_request_delete(request, pk):
    leave_request = get_object_or_404(LeaveRequest, pk=pk)
    if request.method == 'POST':
        leave_request.delete()
        messages.success(request, 'Xóa đơn xin nghỉ phép thành công!')
        return redirect('leave_request_list')
    return render(request, 'leaves/leave_request_confirm_delete.html', {'leave_request': leave_request})


@login_required
def leave_type_list(request):
    leave_types = LeaveType.objects.all()
    return render(request, 'leaves/leave_type_list.html', {'leave_types': leave_types})


@login_required
def leave_type_create(request):
    if request.method == 'POST':
        form = LeaveTypeForm(request.POST)
        if form.is_valid():
            form.save()
            messages.success(request, 'Thêm loại nghỉ phép thành công!')
            return redirect('leave_type_list')
    else:
        form = LeaveTypeForm()
    return render(request, 'leaves/leave_type_form.html', {'form': form, 'title': 'Thêm loại nghỉ phép'})


@login_required
def leave_type_update(request, pk):
    leave_type = get_object_or_404(LeaveType, pk=pk)
    if request.method == 'POST':
        form = LeaveTypeForm(request.POST, instance=leave_type)
        if form.is_valid():
            form.save()
            messages.success(request, 'Cập nhật loại nghỉ phép thành công!')
            return redirect('leave_type_list')
    else:
        form = LeaveTypeForm(instance=leave_type)
    return render(request, 'leaves/leave_type_form.html', {
        'form': form,
        'title': 'Cập nhật loại nghỉ phép',
        'leave_type': leave_type,
    })


@login_required
def leave_type_delete(request, pk):
    leave_type = get_object_or_404(LeaveType, pk=pk)
    if request.method == 'POST':
        leave_type.delete()
        messages.success(request, 'Xóa loại nghỉ phép thành công!')
        return redirect('leave_type_list')
    return render(request, 'leaves/leave_type_confirm_delete.html', {'leave_type': leave_type})
