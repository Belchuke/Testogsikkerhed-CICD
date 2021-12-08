<template>
  <v-container style="background-color: #6a1b9a">
    <v-row align="center" justify="center" class="mt-12">
      <v-col cols="12" align="center">
        <v-card align="center" width="750px">
          <v-card-title primary-title class="justify-center"
            >All user</v-card-title
          >
          <v-card-text>
            <v-col align="center" justify="center">
              <v-data-table
                :headers="headers"
                :items="users"
                :items-per-page="10"
                class="elevation-1"
                @click:row="DeleteItem"
              >
              </v-data-table>
            </v-col>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" align="center">
        <v-card align="center" width="750px">
          <v-card-title primary-title class="justify-center"
            >Create User</v-card-title
          >
          <v-card-actions>
            <v-row>
              <v-col class="mr-2 my-0" cols="12">
                <v-text-field
                  class="px-2 py-0 my-0"
                  hide-details="auto"
                  outlined
                  placeholder="Name"
                  v-model="name"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-text-field
                  class="px-2 py-0"
                  hide-details="auto"
                  outlined
                  placeholder="age"
                  type="number"
                  v-model="age"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-text-field
                  class="px-2 py-0"
                  hide-details="auto"
                  outlined
                  placeholder="email"
                  v-model="email"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-text-field
                  class="px-2 py-0"
                  hide-details="auto"
                  outlined
                  placeholder="Password"
                  v-model="password"
                ></v-text-field>
              </v-col>
              <v-col cols="12" class="pa-4">
                <v-btn @click="CreateUser" color="#AB47BC" block
                  >Create User</v-btn
                >
              </v-col>
            </v-row>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from "axios";
export default {
  props: {
    token: String,
  },
  data: () => ({
    users: [],
    headers: [
      {
        text: "User Name",
        value: "name",
      },
      {
        text: "Email",
        value: "email",
      },
      {
        text: "Id of user",
        value: "id",
      },
      {
        text: "Age of user",
        value: "age",
      },
    ],
    name: null,
    age: null,
    email: null,
    password: null,
    retypePassword: null,
  }),
  mounted() {
    console.log(this.token);
    this.GetUsers();
  },
  methods: {
    async GetUsers() {
      const config = {
        headers: {
          Authorization: `Bearer ${this.token}`,
          "Content-Type": "application/json",
        },
      };
      axios({
        method: "get",
        url: "https://localhost:49167/api/Users",
        config: config,
      }).then((x) => {
        this.users = x.data;
      });
    },
    CreateUser() {
      const data = {
        name: this.name,
        age: this.age,
        email: this.email,
        password: this.password,
      }
      axios.post("https://localhost:49167/api/Users",data,{
        headers: {
          Authorization: `Bearer ${this.token}`,
        },
      }).then(() => {
        this.GetUsers();
      })
    },
    DeleteItem(user) {
      axios.delete("https://localhost:49167/api/Users/" + user.id, {
        headers: {
          Authorization: `Bearer ${this.token}`,
        },
      }).then(() => {
        this.GetUsers();
      });
    },
  },
};
</script>
