from django.forms import ModelForm
from django import forms 
from django.forms import ModelForm

class ImageUploadForm(forms.Form):
        image = forms.ImageField()
