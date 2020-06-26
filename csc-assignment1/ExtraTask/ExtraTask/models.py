from djongo import models
from django import forms
from django.contrib import admin
from ExtraTask import settings
  

class logs(models.Model):
    objectDetected= models.CharField(max_length=100)
    score = models.CharField(max_length=100)
    objects = models.DjongoManager()

class details:
    pass
