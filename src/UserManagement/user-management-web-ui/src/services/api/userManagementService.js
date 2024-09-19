import createApiClient from './axiosConfiguration';

const API_URL = process.env.VUE_APP_USER_MANAGEMENT_API_URL;
const apiClient = createApiClient(API_URL);

export const getUsers = (pageIndex, pageSize) => {
  return apiClient.get('/users', {
    params: {
      pageIndex: pageIndex,
      pageSize: pageSize,
    },
  });
};

export const updateUsers = (modifiedUsers) => {
  return apiClient.put('/users', { users: modifiedUsers });
};
