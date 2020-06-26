from djongo import models
from django import forms
from django.contrib import admin
from ExtraTask import settings
  

class log(models.Model):
    time = models.DateTimeField(auto_now=True) 
    hardHatDetected= models.CharField(max_length=100)
    objects = models.DjongoManager()

