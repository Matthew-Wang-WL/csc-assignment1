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
from .serializers import LogsSerializer 
from django.shortcuts import render, render_to_response
from django.contrib.sites.shortcuts import get_current_site
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
                    viewlog.hardHatDetected = eachLog.hardHatDetected

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
             image = form.cleaned_data['image']  
             objectsDetected = form.cleaned_data['objectsDetected']   
             objectsViolated = form.cleaned_data['objectsViolated']   
             time = datetime.datetime.now()

             try :
                 # add image to database
                 stored = fs.put(image,timestamp = time ) 
                 storedlogId = str(stored) 
              # create log object to store details
                 logentry = log ()
                 logentry.image= storedlogId 
                 logentry.objectsViolated = objectsViolated 
                 logentry.objectsDetected = objectsDetected  
                 #save log to database
                 logentry.save()

                 if not logentry.objectsViolated :
                     #return success response
                     content = {"Success": "Log is saved " } 
                     return Response (content ,status=status.HTTP_200_OK )
                 else :
                     try :
                         date = logentry.time.strftime("%d-%m-%y") 
                         time =logentry.time.strftime("%H:%M") 
                         violatedObjects = getobjectstring (logentry.objectsViolated)
                         data = {'text':'A violation as occured at {0} : {1} and the following PPE was violated {2}'.format(date,time,violatedObjects) } 
                         # Initialize URL
                         baseurl = request.get_host()
                         url = 'http://'+baseurl+'/api/slack/send_message/'
                         # send response
                         response = requests.get(url, data = data)
                         # check response
                         if response.status_code == 200 :
                             # return success response
                             content = {"Success": "Log is saved and message sent to manager " } 
                             return Response (content ,status=status.HTTP_200_OK )
                         else :
                             # raise error if action wasnt successful
                             raise Exception(response.content)
                     except :
                         # return error response
                         content = {"Success": "Log is saved and  error while sending message to safety manager " } 
                         return Response (content ,status=status.HTTP_400_BAD_REQUEST )

             except :
                 # return error response
                 content = { "Error" :  "Error occured" }
                 return  Response (content ,status=status.HTTP_400_BAD_REQUEST )
             
         else : 
             #  return error response if form is invalid
             content ={"Form is invalid. Add an image , violated ppe equipment or  detected ppe equipment"}
             return  Response (content ,status=status.HTTP_400_BAD_REQUEST )

