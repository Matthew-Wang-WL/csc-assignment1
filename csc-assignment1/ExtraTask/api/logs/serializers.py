from rest_framework import serializers
 

class LogsSerializer(serializers.Serializer):

    objectDetected  = serializers.CharField()
    score = serializers.CharField()
    id = serializers.IntegerField() 
