<template>
  <v-app fluid style="background-color: #6A1B9A;"> 
    <v-row align="center" justify="center">
      <v-card align="center" justify="center" width="500px">
        <v-card-title primary-title class="justify-center">Test Og Sikkerhed Project</v-card-title>
        <v-card-actions>
          <v-row>
            <v-col class="mr-2 my-0" cols="12">
              <v-text-field class="px-2 py-0 my-0" v-model="Login" hide-details="auto" outlined placeholder="Name"></v-text-field>
            </v-col>
            <v-col cols="12">
              <v-text-field class="px-2 py-0" v-model="Password" hide-details="auto" outlined placeholder="Password"></v-text-field>
            </v-col>
            <v-col cols="12" class="pa-4">
              <v-btn @click="tryLogin" color="#AB47BC" block>Login</v-btn>
            </v-col>
          </v-row>
        </v-card-actions>
    </v-card>
    </v-row>
  </v-app>
</template>

<script>
import axios from 'axios';

export default {
  name: 'Home',
  data: () => ({
    Login: null,
    Password: null
  }),
  methods: {
    async tryLogin() {

      axios({
          method: 'post', 
          url: 'https://localhost:49167/api/Users/authenticate', 
          data: { name: this.Login, password: this.Password,  },
          headers: { 'Content-Type': 'application/json'  }
        }).then(x => {
          this.$router.push({ name: 'main', params: { token: x.data.token }})
        })
    }
  }
}
</script>
