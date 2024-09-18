<template>
  <div>
    <h2>Login</h2>
    <el-alert v-if="errorMessage" type="error" :closable="false">{{ errorMessage }}</el-alert>

    <el-form @submit.prevent="login" label-width="auto" :model="form" status-icon>
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
    login() {
      const { username, password } = this.form;

      if (username && password === 'password') {
        localStorage.setItem('user', username);
        this.$router.push({ name: 'Welcome' });
      } else {
        this.errorMessage = 'We could not log you in. Please check your username/password and try again.';
        this.form.password = '';
      }
    },
  },
};
</script>

