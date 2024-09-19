<template>
  <div>
    <h2>Welcome, {{ username }}! This page is for authenticated users only.</h2>
    <el-button type="primary" @click="goToUsers">Go to Users Management</el-button>
  </div>
</template>

<script>
import { USERS_URL } from '@/router/urls';

export default {
  computed: {
    username() {
      return localStorage.getItem('user') || 'Guest';
    },
  },
  methods: {
    goToUsers() {
      this.$router.push(USERS_URL);
    },
  },
};
</script>
