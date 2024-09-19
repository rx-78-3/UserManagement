<template>
  <div>
    <h2>Login</h2>
    <el-alert v-if="errorMessage" class="margin-bottom-10" type="error" :closable="false">{{ errorMessage }}</el-alert>

    <el-form @submit.prevent="loginUser" label-width="auto" :model="form" status-icon>
      <el-form-item label="Username">
        <el-input v-model="form.username" placeholder="Enter username"></el-input>
      </el-form-item>
      <el-form-item label="Password">
        <el-input v-model="form.password" placeholder="Enter password" type="password"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" native-type="submit">Login</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import login from '@/services/api/identityService';
import { WELCOME_URL } from '@/router/urls';

export default {
  data() {
    return {
      form: {
        username: '',
        password: '',
      },
      errorMessage: '',
    };
  },
  methods: {
    async loginUser() {
      const { username, password } = this.form;
      
      try {
        const loginResponse = await login(username, password);
        localStorage.setItem('token', loginResponse.data.token);
        this.$router.push(WELCOME_URL);
      } catch (error) {
        console.log(error);
        if (error.response) {
          const statusCode = error.response.status;
          if (statusCode === 400 || statusCode === 401 || statusCode === 403) {
            this.errorMessage = 'We could not log you in. Please check your username/password and try again.';
          } else {
            this.errorMessage = 'An error occurred while logging in.';
          }
        } else if (error.request) {
          this.errorMessage = 'No response received from the server.';
        } else {
          this.errorMessage = 'Error setting up request: ' + error.message;
        }

        this.form.password = '';
      }
    },
  },
};
</script>
