import { createRouter, createWebHashHistory } from 'vue-router';
import LoginPage from '../components/LoginPage.vue';
import WelcomePage from '../components/WelcomePage.vue';
import UsersPage from '../components/UsersPage.vue';

const routes = [
    {
        // Redirect from root to login.
        path: '/',
        redirect: '/login',
    },
    {
        path: '/login',
        name: 'Login',
        component: LoginPage,
    },
    {
        path: '/welcome',
        name: 'Welcome',
        component: WelcomePage,
        meta: { requiresAuth: true },
    },
    {
        path: '/users',
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
    const isAuthenticated = !!localStorage.getItem('user'); // Simulating authentication status.
    if (to.matched.some((record) => record.meta.requiresAuth) && !isAuthenticated) {
        next({ name: 'Login' });
    } else {
        next();
    }
});

export default router;