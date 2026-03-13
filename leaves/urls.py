from django.urls import path
from . import views

urlpatterns = [
    path('', views.leave_request_list, name='leave_request_list'),
    path('create/', views.leave_request_create, name='leave_request_create'),
    path('<int:pk>/', views.leave_request_detail, name='leave_request_detail'),
    path('<int:pk>/edit/', views.leave_request_update, name='leave_request_update'),
    path('<int:pk>/approve/', views.leave_request_approve, name='leave_request_approve'),
    path('<int:pk>/delete/', views.leave_request_delete, name='leave_request_delete'),
    path('types/', views.leave_type_list, name='leave_type_list'),
    path('types/create/', views.leave_type_create, name='leave_type_create'),
    path('types/<int:pk>/edit/', views.leave_type_update, name='leave_type_update'),
    path('types/<int:pk>/delete/', views.leave_type_delete, name='leave_type_delete'),
]
