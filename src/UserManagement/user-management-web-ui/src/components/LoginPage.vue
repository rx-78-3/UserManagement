<template>
  <div>
    <h2>Login</h2>
    <div v-if="errorMessage" class="error">{{ errorMessage }}</div>
    <form @submit.prevent="login">
      <div>
        <label>Username:</label>
        <input type="text" v-model="username" />
      </div>
      <div>
        <label>Password:</label>
        <input type="password" v-model="password" />
      </div>
      <button type="submit">Submit</button>
    </form>
  </div>
</template>

<script>
export default {
  data() {
    return {
      username: '',
      password: '',
      errorMessage: '',
    };
  },
  methods: {
    login() {
      if (this.username && this.password === 'password') {
        localStorage.setItem('user', this.username); // Store user in localStorage
        this.$router.push({ name: 'Welcome' });
      } else {
        this.errorMessage = 'We could not log you in. Please check your username/password and try again.';
        this.password = ''; // Clear password field
      }
    },
  },
};
</script>

<style scoped>
.error {
  color: red;
  margin-bottom: 10px;
}
</style>
