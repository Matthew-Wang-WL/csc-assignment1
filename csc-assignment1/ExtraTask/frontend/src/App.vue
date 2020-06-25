<template>
    <v-app>
        <v-app-bar app color="primary" dark class="justify-center d-flex">
            <span>HARD HAT DETECTION</span>
        </v-app-bar>

        <v-main>
            <router-view></router-view>
        </v-main>
    </v-app>
</template>

<script>
export default {
    name: 'App'
};
</script>
