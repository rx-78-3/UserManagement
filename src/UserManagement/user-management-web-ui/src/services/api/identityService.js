import axios from 'axios';

const API_URL = process.env.VUE_APP_IDENTITY_API_URL;

const login = (userName, password) => {
    return axios.post(`${API_URL}/login`, { userName, password });
};

export default login;
