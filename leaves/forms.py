from django import forms
from .models import LeaveRequest, LeaveType


class LeaveTypeForm(forms.ModelForm):
    class Meta:
        model = LeaveType
        fields = ['name', 'annual_days', 'is_paid', 'description']
        widgets = {
            'name': forms.TextInput(attrs={'class': 'form-control'}),
            'annual_days': forms.NumberInput(attrs={'class': 'form-control'}),
            'is_paid': forms.CheckboxInput(attrs={'class': 'form-check-input'}),
            'description': forms.Textarea(attrs={'class': 'form-control', 'rows': 3}),
        }


class LeaveRequestForm(forms.ModelForm):
    class Meta:
        model = LeaveRequest
        fields = ['employee', 'leave_type', 'start_date', 'end_date', 'reason']
        widgets = {
            'employee': forms.Select(attrs={'class': 'form-select'}),
            'leave_type': forms.Select(attrs={'class': 'form-select'}),
            'start_date': forms.DateInput(attrs={'class': 'form-control', 'type': 'date'}),
            'end_date': forms.DateInput(attrs={'class': 'form-control', 'type': 'date'}),
            'reason': forms.Textarea(attrs={'class': 'form-control', 'rows': 3}),
        }

    def clean(self):
        cleaned_data = super().clean()
        start_date = cleaned_data.get('start_date')
        end_date = cleaned_data.get('end_date')
        if start_date and end_date and end_date < start_date:
            raise forms.ValidationError('Ngày kết thúc phải sau ngày bắt đầu.')
        return cleaned_data


class LeaveApprovalForm(forms.ModelForm):
    class Meta:
        model = LeaveRequest
        fields = ['status', 'approved_by', 'rejection_reason']
        widgets = {
            'status': forms.Select(attrs={'class': 'form-select'}),
            'approved_by': forms.Select(attrs={'class': 'form-select'}),
            'rejection_reason': forms.Textarea(attrs={'class': 'form-control', 'rows': 3}),
        }
