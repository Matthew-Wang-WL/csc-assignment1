import codecs
import datetime 
from bson import ObjectId

import pymongo
import gridfs
from pymongo import MongoClient
import os 
from rest_framework import status
from rest_framework import serializers
from rest_framework.response import Response
from rest_framework.views import APIView 
from ExtraTask.models import details, logs
from ExtraTask.forms import ImageUploadForm
from .serializers import LogsSerializer 
from django.shortcuts import render, render_to_response
from django.contrib.sites.shortcuts import get_current_site
import slack
import requests

from slack import WebClient
from slack.errors import SlackApiError

# Initialize Mongodb file configuration
client = pymongo.MongoClient('mongodb+srv://csc_user:admin@extratask-d0sjm.mongodb.net/test?retryWrites=true&w=majority')
db = client["ExtraTask"]
fs = gridfs.GridFS(db, collection='ExtraTask_logs')

class LogsView(APIView):
    def get(self, request):
            #initialize variables
            items = [] 
                   
            try :            
                alllogs = logs.objects.all()
                for eachLog in alllogs:
                    # create new details object and ppe equipment list
                    viewlog = details()

                    # initialize variables
                    viewlog.id = eachLog.id 
                    viewlog.objectDetected = eachLog.objectDetected
                    viewlog.score = eachLog.score

                    # add object to list
                    items.append(viewlog) 

            except : 
                # return error response incase of error
                content = {"Error":"Error while retrieving log data"}
                return Response(content ,status=status.HTTP_400_BAD_REQUEST)
            # initialze serializer with records
            serializer = LogsSerializer(items, many=True)

            # return succes response
            content = {"Success": serializer.data}
            return Response(content,status=status.HTTP_200_OK)
   
    def post(self, request):        
        #initialize variables of form
        form = ImageUploadForm(request.POST, request.FILES) 
        if form.is_valid():
              
             #initialize variables to add into object
             objectDetected = form.cleaned_data['objectDetected']  
             score = form.cleaned_data['score']   

             try :
              # create log object to store details
                 logentry = logs()
                 logentry.objectDetected = objectDetected
                 logentry.score = score
                 #save log to database
                 logentry.save()

                 #Slack message
                 text = '{0} detected'.format(logentry.objectDetected)  
                 # get all slack credentials
                 accessTokens = 'xoxb-1204644572997-1229905883280-MykozGkYHsQE3pJZ0ZjEQ7cY'
                 channels = 'C0162709R2P'

                 client = slack.WebClient(token= accessTokens)
                 client.chat_postMessage(channel= channels, text=text )
                 # return OK response 
                 content = {"success":"Results have been sent to your slack workspace."}
                 return Response(content ,status=status.HTTP_200_OK)
             except: 
                # return error response incase of error
                content = {"Error":"Error while retrieving log data"}
                return Response(content ,status=status.HTTP_400_BAD_REQUEST)
        else : 
        #       return error response if form is invalid
             content = {"Error":"Form is invalid"}
             return  Response(content ,status=status.HTTP_400_BAD_REQUEST)