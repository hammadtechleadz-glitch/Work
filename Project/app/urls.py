from django.urls import path
from django.contrib.auth import views as auth_views
from . import views

urlpatterns = [

    path('', views.note_list, name='note_list'),

    path('add/', views.AddNote, name='add_note'),

    path('edit/<int:id>/', views.EditNote, name='edit_note'),

    path('delete/<int:eid>/', views.DeleteNote, name='delete_note'),

    path('view/<int:eid>/', views.ViewNote, name='view_note'),

    path('notes/autosave/', views.autosave_note, name='autosave_note'),

    path('login/', auth_views.LoginView.as_view(template_name='login.html'), name='login'),

    path('logout/', auth_views.LogoutView.as_view(), name='logout'),

    path('register/', views.RegisterUser, name='register'),
]