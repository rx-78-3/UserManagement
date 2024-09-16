<template>
  <div>
    <h2>Welcome, {{ username }}! This page is for authenticated users only.</h2>
    <button @click="goToUsers">Go to Users Management</button>
  </div>
</template>

<script>
export default {
  computed: {
    username() {
      return localStorage.getItem('user') || 'Guest';
    },
  },
  methods: {
    goToUsers() {
      this.$router.push({ name: 'Users' });
    },
  },
};
</script>