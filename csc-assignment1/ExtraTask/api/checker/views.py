# Import packages
import os
import cv2 as cv
import numpy as np
import tensorflow as tf
# patch tf1 into `utils.ops`
#utils_ops.tf = tf.compat.v1

# Patch the location of gfile
tf.gfile = tf.io.gfile
#import six.moves.urllib as urllib
from six import Module_six_moves_urllib as urllib
#import codecs
#import datetime 
#from bson import ObjectId
#import pymongo
#import gridfs
#from pymongo import MongoClient
#from rest_framework import status
#from rest_framework import serializers
#from rest_framework.response import Response
#from rest_framework.views import APIView 
#from ExtraTask.models import details, log
#from django.shortcuts import render, render_to_response
#from django.contrib.sites.shortcuts import get_current_site
#import requests

# Initialize Mongodb file configuration 
#client = pymongo.MongoClient('mongodb+srv://csc_user:admin@extratask-d0sjm.mongodb.net/test?retryWrites=true&w=majority')
#db = client["ExtraTask"]
#fs = gridfs.GridFS(db, collection='data')

# Read the graph.
with tf.gfile.FastGFile('frozen_inference_graph.pb', 'rb') as f:
    graph_def = tf.GraphDef()
    graph_def.ParseFromString(f.read())

with tf.Session() as sess:
    # Restore session
    sess.graph.as_default()
    tf.import_graph_def(graph_def, name='')

    # Read and preprocess an image.
    img = cv.imread('images/test.jpg')
    rows = img.shape[0]
    cols = img.shape[1]
    inp = cv.resize(img, (300, 300))
    inp = inp[:, :, [2, 1, 0]]  # BGR2RGB

    # Run the model
    out = sess.run([sess.graph.get_tensor_by_name('num_detections:0'),
                    sess.graph.get_tensor_by_name('detection_scores:0'),
                    sess.graph.get_tensor_by_name('detection_boxes:0'),
                    sess.graph.get_tensor_by_name('detection_classes:0')],
                   feed_dict={'image_tensor:0': inp.reshape(1, inp.shape[0], inp.shape[1], 3)})

    # Visualize detected bounding boxes.
    num_detections = int(out[0][0])
    for i in range(num_detections):
        classId = int(out[3][0][i])
        score = float(out[1][0][i])
        bbox = [float(v) for v in out[2][0][i]]
        if score > 0.3:
            x = bbox[1] * cols
            y = bbox[0] * rows
            right = bbox[3] * cols
            bottom = bbox[2] * rows
            cv.rectangle(img, (int(x), int(y)), (int(right), int(bottom)), (125, 255, 51), thickness=2)

cv.imshow('TensorFlow MobileNet-SSD', img)
cv.waitKey()