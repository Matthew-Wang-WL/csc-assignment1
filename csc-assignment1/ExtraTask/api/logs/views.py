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
from ExtraTask.models import details, log
from ExtraTask.forms import ImageUploadForm
from .serializers import LogsSerializer 
from django.shortcuts import render, render_to_response
from django.contrib.sites.shortcuts import get_current_site
import slack
import requests

# Initialize Mongodb file configuration 
client = pymongo.MongoClient('mongodb+srv://csc_user:admin@extratask-d0sjm.mongodb.net/test?retryWrites=true&w=majority')
db = client["ExtraTask"]
fs = gridfs.GridFS(db, collection='data')

class LogsView(APIView):
    def get(self, request):

        #initialize variables
        items = [] 

        # get all records of logs
        alllogs = log.objects.all()

        try :            

            for eachLog in alllogs:
                    # create new details object and ppe equipment list 
                    viewlog = details()

                    # get time and date from timestamp and convert to string
                    date = eachLog.time.strftime("%d-%m-%y") 
                    time =eachLog.time.strftime("%H:%M") 

                    # initialize variables
                    viewlog.id = eachLog.id
                    viewlog.time = "{0} , {1}".format(date , time)   
                    viewlog.objectDetected = eachLog.objectDetected
                    viewlog.score = eachLog.score

                    # add object to list
                    items.append(viewlog) 

        except: 
            # return error response incase of error
            content = {"Error":"Error while retrieving log data"}
            return Response (content ,status=status.HTTP_400_BAD_REQUEST )

        # initialze serializer with records
        serializer = LogsSerializer(items, many=True)

        # return succes response
        content ={"Success": serializer.data}
        return Response(content,status=status.HTTP_200_OK)
   
    def post(self, request):        
        #initialize variables of form
         form = ImageUploadForm(request.POST, request.FILES) 
         if form.is_valid():
              
             #initialize variables to add into object
             objectDetected = form.cleaned_data['objectDetected']  
             score = form.cleaned_data['score']   
             time = datetime.datetime.now()

             try :
              # create log object to store details
                 logentry = log()
                 logentry.objectDetected = objectDetected
                 logentry.score = score
                 logentry.time = time
                 #save log to database
                 logentry.save()

                 date = logentry.time.strftime("%d-%m-%y") 
                 time =logentry.time.strftime("%H:%M") 
                 object = getobjectstring (logentry.objectDetected)

                 #Slack message
                 text = {'text':'{2} detected at {0} : {1}'.format(date,time,object) } 
                 # get all slack credentials
                 accessTokens = 'xoxb-1204644572997-1229905883280-MykozGkYHsQE3pJZ0ZjEQ7cY'
                 channels = 'C0162709R2P'
                 # publish message 
                 client = slack.WebClient(accessTokens)
                 client.chat_postMessage(channels, text=text )
                
                 # check response
                 if response.status_code == 200 :
                    # return success response
                    content = {"Success": "Log is saved and message sent to manager " } 
                    return Response(content ,status=status.HTTP_200_OK)
                 else :
                    # return error response
                    content = { "Error" :  "Error occured" }
                    return  Response(content ,status=status.HTTP_400_BAD_REQUEST)
             
         else : 
             #  return error response if form is invalid
             content = {"Form is invalid"}
             return  Response(content ,status=status.HTTP_400_BAD_REQUEST)