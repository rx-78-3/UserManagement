import { createRouter, createWebHashHistory } from 'vue-router';
import LoginPage from '@/components/LoginPage.vue';
import WelcomePage from '@/components/WelcomePage.vue';
import UsersPage from '@/components/UsersPage.vue';
import { LOGIN_URL, WELCOME_URL, USERS_URL } from '@/router/urls.js';

const routes = [
    {
        // Redirect from root to login.
        path: '/',
        redirect: LOGIN_URL,
    },
    {
        path: LOGIN_URL,
        name: 'Login',
        component: LoginPage,
    },
    {
        path: WELCOME_URL,
        name: 'Welcome',
        component: WelcomePage,
        meta: { requiresAuth: true },
    },
    {
        path: USERS_URL,
        name: 'Users',
        component: UsersPage,
        meta: { requiresAuth: true },
    },
];

const router = createRouter({
    history: createWebHashHistory(),
    routes,
})

// Navigation guard to check if user is authenticated.
router.beforeEach((to, from, next) => {
    const isAuthenticated = !!localStorage.getItem('token');
    if (to.matched.some((record) => record.meta.requiresAuth) && !isAuthenticated) {
        next({ name: 'Login' });
    } else {
        next();
    }
});

export default router;