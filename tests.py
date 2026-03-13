from django.test import TestCase, Client
from django.contrib.auth.models import User
from django.urls import reverse
from datetime import date
from departments.models import Department
from employees.models import Employee
from attendance.models import AttendanceRecord
from leaves.models import LeaveType, LeaveRequest


class BaseTestCase(TestCase):
    def setUp(self):
        self.client = Client()
        self.user = User.objects.create_superuser('testadmin', 'test@test.com', 'testpass123')
        self.client.login(username='testadmin', password='testpass123')

        self.department = Department.objects.create(
            name='Phòng Công nghệ thông tin',
            code='CNTT',
            description='IT Department',
        )
        self.employee = Employee.objects.create(
            employee_id='NV001',
            first_name='An',
            last_name='Nguyễn',
            gender='M',
            date_of_birth=date(1990, 1, 1),
            phone='0901234567',
            email='an@test.com',
            department=self.department,
            position='Developer',
            hire_date=date(2020, 1, 1),
            status='active',
            basic_salary=15000000,
        )
        self.leave_type = LeaveType.objects.create(
            name='Nghỉ phép năm',
            annual_days=12,
            is_paid=True,
        )


class DepartmentViewTests(BaseTestCase):
    def test_department_list(self):
        resp = self.client.get(reverse('department_list'))
        self.assertEqual(resp.status_code, 200)
        self.assertContains(resp, 'CNTT')

    def test_department_create(self):
        resp = self.client.post(reverse('department_create'), {
            'name': 'Phòng Kinh doanh',
            'code': 'KD',
            'description': '',
        })
        self.assertEqual(resp.status_code, 302)
        self.assertTrue(Department.objects.filter(code='KD').exists())

    def test_department_update(self):
        resp = self.client.post(
            reverse('department_update', args=[self.department.pk]),
            {'name': 'IT Updated', 'code': 'CNTT', 'description': ''}
        )
        self.assertEqual(resp.status_code, 302)
        self.department.refresh_from_db()
        self.assertEqual(self.department.name, 'IT Updated')

    def test_department_delete(self):
        dept = Department.objects.create(name='Temp', code='TMP')
        resp = self.client.post(reverse('department_delete', args=[dept.pk]))
        self.assertEqual(resp.status_code, 302)
        self.assertFalse(Department.objects.filter(code='TMP').exists())

    def test_department_detail(self):
        resp = self.client.get(reverse('department_detail', args=[self.department.pk]))
        self.assertEqual(resp.status_code, 200)
        self.assertContains(resp, self.department.name)


class EmployeeViewTests(BaseTestCase):
    def test_employee_list(self):
        resp = self.client.get(reverse('employee_list'))
        self.assertEqual(resp.status_code, 200)
        self.assertContains(resp, 'NV001')

    def test_employee_list_search(self):
        resp = self.client.get(reverse('employee_list'), {'query': 'Nguyễn'})
        self.assertEqual(resp.status_code, 200)
        self.assertContains(resp, 'NV001')

    def test_employee_detail(self):
        resp = self.client.get(reverse('employee_detail', args=[self.employee.pk]))
        self.assertEqual(resp.status_code, 200)
        self.assertContains(resp, 'NV001')

    def test_employee_create(self):
        resp = self.client.post(reverse('employee_create'), {
            'employee_id': 'NV002',
            'first_name': 'Bình',
            'last_name': 'Trần',
            'gender': 'F',
            'date_of_birth': '1992-05-20',
            'phone': '0912345678',
            'email': 'binh@test.com',
            'department': self.department.pk,
            'position': 'HR',
            'hire_date': '2021-01-01',
            'status': 'active',
            'basic_salary': 12000000,
        })
        self.assertEqual(resp.status_code, 302)
        self.assertTrue(Employee.objects.filter(employee_id='NV002').exists())

    def test_employee_update(self):
        resp = self.client.post(
            reverse('employee_update', args=[self.employee.pk]),
            {
                'employee_id': 'NV001',
                'first_name': 'An',
                'last_name': 'Nguyễn Updated',
                'gender': 'M',
                'date_of_birth': '1990-01-01',
                'phone': '0901234567',
                'email': 'an@test.com',
                'department': self.department.pk,
                'position': 'Senior Developer',
                'hire_date': '2020-01-01',
                'status': 'active',
                'basic_salary': 20000000,
            }
        )
        self.assertEqual(resp.status_code, 302)
        self.employee.refresh_from_db()
        self.assertEqual(self.employee.position, 'Senior Developer')

    def test_employee_delete(self):
        emp = Employee.objects.create(
            employee_id='NV_DEL',
            first_name='Delete',
            last_name='Test',
            gender='M',
            date_of_birth=date(1990, 1, 1),
            phone='0000000000',
            email='del@test.com',
            position='Test',
            hire_date=date(2020, 1, 1),
        )
        resp = self.client.post(reverse('employee_delete', args=[emp.pk]))
        self.assertEqual(resp.status_code, 302)
        self.assertFalse(Employee.objects.filter(employee_id='NV_DEL').exists())

    def test_employee_full_name(self):
        self.assertEqual(self.employee.full_name, 'Nguyễn An')


class AttendanceViewTests(BaseTestCase):
    def setUp(self):
        super().setUp()
        self.attendance = AttendanceRecord.objects.create(
            employee=self.employee,
            date=date(2026, 3, 1),
            check_in='08:00',
            check_out='17:00',
            status='present',
        )

    def test_attendance_list(self):
        resp = self.client.get(reverse('attendance_list'))
        self.assertEqual(resp.status_code, 200)

    def test_attendance_create(self):
        resp = self.client.post(reverse('attendance_create'), {
            'employee': self.employee.pk,
            'date': '2026-03-10',
            'check_in': '08:00',
            'check_out': '17:30',
            'status': 'present',
            'note': '',
        })
        self.assertEqual(resp.status_code, 302)
        self.assertTrue(AttendanceRecord.objects.filter(
            employee=self.employee, date=date(2026, 3, 10)
        ).exists())

    def test_attendance_work_hours(self):
        self.assertEqual(self.attendance.work_hours, 9.0)

    def test_attendance_update(self):
        resp = self.client.post(
            reverse('attendance_update', args=[self.attendance.pk]),
            {
                'employee': self.employee.pk,
                'date': '2026-03-01',
                'check_in': '09:00',
                'check_out': '17:00',
                'status': 'late',
                'note': '',
            }
        )
        self.assertEqual(resp.status_code, 302)
        self.attendance.refresh_from_db()
        self.assertEqual(self.attendance.status, 'late')

    def test_attendance_delete(self):
        att = AttendanceRecord.objects.create(
            employee=self.employee,
            date=date(2026, 3, 5),
            status='present',
        )
        resp = self.client.post(reverse('attendance_delete', args=[att.pk]))
        self.assertEqual(resp.status_code, 302)
        self.assertFalse(AttendanceRecord.objects.filter(pk=att.pk).exists())


class LeaveViewTests(BaseTestCase):
    def setUp(self):
        super().setUp()
        self.leave_request = LeaveRequest.objects.create(
            employee=self.employee,
            leave_type=self.leave_type,
            start_date=date(2026, 3, 20),
            end_date=date(2026, 3, 22),
            reason='Nghỉ phép du lịch',
            status='pending',
        )

    def test_leave_type_list(self):
        resp = self.client.get(reverse('leave_type_list'))
        self.assertEqual(resp.status_code, 200)
        self.assertContains(resp, 'Nghỉ phép năm')

    def test_leave_type_create(self):
        resp = self.client.post(reverse('leave_type_create'), {
            'name': 'Nghỉ ốm',
            'annual_days': 15,
            'is_paid': True,
            'description': '',
        })
        self.assertEqual(resp.status_code, 302)
        self.assertTrue(LeaveType.objects.filter(name='Nghỉ ốm').exists())

    def test_leave_request_list(self):
        resp = self.client.get(reverse('leave_request_list'))
        self.assertEqual(resp.status_code, 200)

    def test_leave_request_create(self):
        resp = self.client.post(reverse('leave_request_create'), {
            'employee': self.employee.pk,
            'leave_type': self.leave_type.pk,
            'start_date': '2026-04-01',
            'end_date': '2026-04-03',
            'reason': 'Test leave',
        })
        self.assertEqual(resp.status_code, 302)

    def test_leave_request_detail(self):
        resp = self.client.get(reverse('leave_request_detail', args=[self.leave_request.pk]))
        self.assertEqual(resp.status_code, 200)

    def test_leave_request_days_count(self):
        self.assertEqual(self.leave_request.days_count, 3)

    def test_leave_request_approve(self):
        resp = self.client.post(
            reverse('leave_request_approve', args=[self.leave_request.pk]),
            {
                'status': 'approved',
                'approved_by': self.employee.pk,
                'rejection_reason': '',
            }
        )
        self.assertEqual(resp.status_code, 302)
        self.leave_request.refresh_from_db()
        self.assertEqual(self.leave_request.status, 'approved')

    def test_leave_request_delete(self):
        lr = LeaveRequest.objects.create(
            employee=self.employee,
            leave_type=self.leave_type,
            start_date=date(2026, 5, 1),
            end_date=date(2026, 5, 2),
            reason='To be deleted',
        )
        resp = self.client.post(reverse('leave_request_delete', args=[lr.pk]))
        self.assertEqual(resp.status_code, 302)
        self.assertFalse(LeaveRequest.objects.filter(pk=lr.pk).exists())


class DashboardTest(BaseTestCase):
    def test_dashboard(self):
        resp = self.client.get(reverse('dashboard'))
        self.assertEqual(resp.status_code, 200)
        self.assertContains(resp, 'HRNewcity')

    def test_login_required(self):
        self.client.logout()
        resp = self.client.get(reverse('dashboard'))
        self.assertEqual(resp.status_code, 302)
        self.assertIn('/login/', resp.url)
