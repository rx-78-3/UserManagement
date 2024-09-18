<template>
  <div>
    <h2>Users Management</h2>
    <el-alert v-if="error" 
      class="margin-bottom-10"
      type="error" 
      :closable="false"
    >
      {{ error }}
    </el-alert>

    <el-alert v-if="modifiedUsers.length > 0"
        class="margin-bottom-10"
        title="One or more users have been modified."
        type="warning"
        show-icon
    >
      <el-button type="primary" @click="saveChanges" :loading="saving">Save</el-button>
    </el-alert>

    <el-list>
      <el-list-item v-for="user in users" :key="user.userName">
        <el-row class="margin-bottom-10">
          <el-col :span="12">
            <span @click="toggleUser(user)" style="cursor: pointer;">
              {{ user.userName }}
            </span> - <strong>{{ user.isActive ? 'Yes' : 'No' }}</strong>
          </el-col>
        </el-row>

        <el-collapse-transition>
          <div v-if="user.showDetails" class="margin-bottom-10">
            <el-card>
              <p>Username: {{ user.userName }}</p>
              <el-form-item label="Active">
              <el-switch v-model="user.isActive" @change="markModified(user)"></el-switch>
              </el-form-item>
              <el-button type="success" @click="closeDetails(user)">OK</el-button>
            </el-card>
          </div>
        </el-collapse-transition>
      </el-list-item>
    </el-list>

    <el-pagination class="pagination"
      v-if="totalCount > 0"
      background
      layout="prev, pager, next"
      :total="totalCount"
      :page-size="pageSize"
      v-model:current-page="pageIndex"
      @current-change="fetchUsers"
    ></el-pagination>
  </div>
</template>

<script>
import { getUsers, updateUsers } from '@/services/api/userManagementService';


export default {
  data() {
    return {
      users: [],
      pageIndex: 1,
      pageSize: 10,
      totalCount: 0,
      modifiedUsers: [],
      saving: false,
      error: '',
    };
  },
  mounted() {
    this.fetchUsers();
  },
  methods: {
    async fetchUsers() {
      try {
        const response = await getUsers(this.pageIndex - 1, this.pageSize);
        this.users = response.data.data;
        this.totalCount = response.data.totalCount;
      } catch (err) {
        this.error = 'Error fetching users. Please try again later.';
        console.error(err);
      }
    },
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
    async saveChanges() {
      this.saving = true;
      this.error = '';
      try {
        await updateUsers(this.modifiedUsers);
        this.modifiedUsers = [];
        this.fetchUsers();
      } catch (err) {
        this.error = 'Error saving changes. Please try again later.';
        console.error(err);
      } finally {
        this.saving = false;
      }
    },
  },
};
</script>

<style scoped>
    .pagination {
        margin-top: 20px;
    }
    
    .margin-bottom-10 {
      margin-bottom: 10px;
    }
</style>