from django.urls import path
from api.logs.views import LogsView

urlpatterns = [
    path('logs/', LogsView.as_view()),       
]


