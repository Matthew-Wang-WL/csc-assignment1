from rest_framework import serializers
 

class LogsSerializer(serializers.Serializer):

    time = serializers.CharField()  
    objectDetected  = serializers.CharField()
    score = serializers.DecimalField()
    id = serializers.IntegerField() 
