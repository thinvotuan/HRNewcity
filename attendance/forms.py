from django import forms
from .models import AttendanceRecord


class AttendanceForm(forms.ModelForm):
    class Meta:
        model = AttendanceRecord
        fields = ['employee', 'date', 'check_in', 'check_out', 'status', 'note']
        widgets = {
            'employee': forms.Select(attrs={'class': 'form-select'}),
            'date': forms.DateInput(attrs={'class': 'form-control', 'type': 'date'}),
            'check_in': forms.TimeInput(attrs={'class': 'form-control', 'type': 'time'}),
            'check_out': forms.TimeInput(attrs={'class': 'form-control', 'type': 'time'}),
            'status': forms.Select(attrs={'class': 'form-select'}),
            'note': forms.Textarea(attrs={'class': 'form-control', 'rows': 2}),
        }


class AttendanceFilterForm(forms.Form):
    month = forms.IntegerField(
        min_value=1, max_value=12, required=False,
        widget=forms.NumberInput(attrs={'class': 'form-control', 'placeholder': 'Tháng'}),
    )
    year = forms.IntegerField(
        min_value=2000, required=False,
        widget=forms.NumberInput(attrs={'class': 'form-control', 'placeholder': 'Năm'}),
    )
    employee = forms.ChoiceField(
        required=False,
        widget=forms.Select(attrs={'class': 'form-select'}),
    )

    def __init__(self, *args, **kwargs):
        from employees.models import Employee
        super().__init__(*args, **kwargs)
        emp_choices = [('', 'Tất cả nhân viên')] + [
            (e.id, e.full_name) for e in Employee.objects.filter(status='active').order_by('last_name')
        ]
        self.fields['employee'].choices = emp_choices
