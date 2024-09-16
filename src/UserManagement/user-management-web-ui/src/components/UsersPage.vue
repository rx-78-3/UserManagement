<template>
  <div>
    <h2>Users Management</h2>
    <div v-if="modifiedUsers.length > 0" class="save-panel">
      <p>One or more users have been modified.</p>
      <button @click="saveChanges">Save</button>
    </div>
    <ul>
      <li v-for="user in users" :key="user.username">
        <span @click="toggleUser(user)">{{ user.username }}</span> - {{ user.isActive ? 'Yes' : 'No' }}
        <div v-if="user.showDetails">
          <p>Username: {{ user.username }}</p>
          <label>
            Active: 
            <input type="checkbox" v-model="user.isActive" @change="markModified(user)" />
          </label>
          <button @click="closeDetails(user)">OK</button>
        </div>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  data() {
    return {
      users: [
        { username: 'someuser1', isActive: true, showDetails: false },
        { username: 'someuser2', isActive: false, showDetails: false },
      ],
      modifiedUsers: [],
    };
  },
  methods: {
    toggleUser(user) {
      user.showDetails = !user.showDetails;
    },
    closeDetails(user) {
      user.showDetails = false;
    },
    markModified(user) {
      if (!this.modifiedUsers.includes(user)) {
        this.modifiedUsers.push(user);
      }
    },
    saveChanges() {
      console.log('Modified users:', this.modifiedUsers);
      this.modifiedUsers = [];
    },
  },
};
</script>

<style scoped>
.save-panel {
  background-color: yellow;
  padding: 10px;
  margin-bottom: 10px;
}
</style>