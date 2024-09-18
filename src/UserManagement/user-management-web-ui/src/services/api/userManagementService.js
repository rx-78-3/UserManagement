import axios from 'axios';

const API_URL = process.env.VUE_APP_USER_MANAGEMENT_API_URL;

export const getUsers = (pageIndex, pageSize) => {
  return axios.get(`${API_URL}`, {
    params: {
      pageIndex: pageIndex,
      pageSize: pageSize,
    },
  });
};

export const updateUsers = (modifiedUsers) => {
  return axios.put(`${API_URL}`, { users: modifiedUsers });
};
