from djongo import models
from django import forms
from django.contrib import admin
from ExtraTask import settings
  

class log(models.Model):
    time = models.DateTimeField(auto_now=True) 
    objectDetected= models.CharField(max_length=100)
    score = models.DecimalField(max_digits=3, decimal_places=2)
    objects = models.DjongoManager()

class details:
    pass
