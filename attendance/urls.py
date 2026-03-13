from django.urls import path
from . import views

urlpatterns = [
    path('', views.attendance_list, name='attendance_list'),
    path('create/', views.attendance_create, name='attendance_create'),
    path('<int:pk>/edit/', views.attendance_update, name='attendance_update'),
    path('<int:pk>/delete/', views.attendance_delete, name='attendance_delete'),
]
