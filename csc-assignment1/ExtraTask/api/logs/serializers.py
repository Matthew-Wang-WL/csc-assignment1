from rest_framework import serializers
 

class LogsSerializer(serializers.Serializer):

    time = serializers.CharField()  
    hardHatDetected  = serializers.CharField()
    id = serializers.IntegerField() 
