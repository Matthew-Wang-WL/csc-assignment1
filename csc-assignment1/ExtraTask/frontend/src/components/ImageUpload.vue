<template>
    <v-container>
        <div class="mb-6 d-flex justify-center">
            <div id="preview">
                <img v-if="url" :src="url" id="image" />
                <canvas id="canvas" />
            </div>
        </div>
        <v-container class="d-flex justify-center">
            <input type="file" @change="onFileChange" accept="image/*" />
            <v-btn color="primary" @click="onUpload" class="ml-8">Upload</v-btn>
        </v-container>
        <v-container class="d-flex justify-center pa-8">
            <span id="msg" class="overline"></span>
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
        url: null,
        logs: [],
        headers: [
            { text: 'ID', value: 'id' },
            { text: 'Object Detected', value: 'objectDetected' },
            { text: 'Score', value: 'score' },
        ],
        isLoading: true
    }),
    mounted() {
        this.fetchRecords();
    },
    methods: {
        onFileChange(e) {
            const file = e.target.files[0];
            this.url = URL.createObjectURL(file);
        },
        async onUpload() {
            if (this.url != null) {
                const image = document.getElementById('image');

                await cocoSsd
                    .load()
                    .then(model => model.detect(image))
                    .then(predictions => {
                        console.log(predictions);
                        var formData = new FormData();
                        if (predictions.length != 0) {
                            formData.append(
                                'objectDetected',
                                predictions[0].class
                            );
                            formData.append('score', predictions[0].score);

                            console.log(predictions[0].class);
                            console.log(predictions[0].score);
                        } else {
                            formData.append('objectDetected', 'None');
                            formData.append('score', 0);
                        }

                        // Pass results to API to save in database
                        axios({
                            method: 'post',
                            url: '/api/logs/',
                            data: formData,
                            headers: {
                                'Content-Type': 'multipart/form-data'
                            }
                        })
                            .then(response => {
                                //success
                                console.log(response);
                                document.getElementById('msg').innerHTML = 'Results were uploaded. View them in your slack workspace or in the table below.'
                                document.getElementById('msg').classList.add("green--text")
                                document.getElementById('msg').classList.remove("red--text")
                            })
                            .catch(response => {
                                //error
                                console.log(response);
                                document.getElementById('msg').innerHTML = 'Error has occured. Failed to upload results.';
                                document.getElementById('msg').classList.remove("green--text")
                                document.getElementById('msg').classList.add("red--text")
                            });
                    });
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
#preview {
    position: relative;
}
#canvas {
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
}
</style>
