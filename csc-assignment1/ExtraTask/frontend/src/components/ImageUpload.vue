<template>
    <v-container>
        <v-container class="d-flex justify-center">
            <img v-if="fileUrl" :src="fileUrl" />
            <v-file-input
                v-model="file"
                accept="image/*"
                label="Select an image..."
                @change="onFileChange"
            ></v-file-input>
            <v-btn color="primary" @click="onUpload" class="ml-8">Upload</v-btn>
        </v-container>
        <v-data-table
            :headers="headers"
            :items="logs"
            item-key="id"
            class="elevation-2"
            :loading="isLoading"
        >
        </v-data-table>
    </v-container>
</template>

<script>
import axios from 'axios';
import * as cocoSsd from '@tensorflow-models/coco-ssd';

export default {
    name: 'ImageUpload',

    data: () => ({
        file: null,
        fileUrl: null,
        logs: [],
        headers: [
            { text: 'ID', value: 'id' },
            { text: 'Hard Hat Detected', value: 'hardHatDetected' },
            { text: 'Time', value: 'time' }
        ],
        isLoading: true
    }),
    mounted() {
        this.fetchRecords();
    },
    methods: {
        onFileChange: (e) => {
            if (this.file != null) {
                this.fileUrl = URL.createObjectURL(this.file);
            }
        },
        onUpload() {
            if (this.file != null) {
                const image = document.getElementById('image');
                cocoSsd
                    .load()
                    .then(model => model.detect(image))
                    .then(predictions => console.log(predictions));
                /*
                // Send results to api to store in database
                let formData = new formData();
                formData.append('file', this.file, this.file.name);

                axios({
                    method: 'post',
                    url: '/api/checker/',
                    data: formData,
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                })
                    .then(response => {
                        //display success message
                        console.log(response);
                    })
                    .catch(response => {
                        //display error message
                        console.log(response);
                    });*/
            } else {
                //display error message
            }
        },
        fetchRecords() {
            // Fetch PPE records from the database
            this.isLoading = true;

            axios.get('/api/logs').then(response => {
                this.logs = response.data.Success;
            });
            this.isLoading = false;
        }
    }
};
</script>

<style scoped>
.v-input {
    max-width: 40% !important;
    align-self: center;
}
.v-btn {
    align-self: center;
}
.v-data-table {
    max-width: 70% !important;
    margin: 0 auto;
}
</style>
