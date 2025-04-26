import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('../views/AboutView.vue'),
    },
    {
      path: '/admin',
      name: 'admin',
      component: () => import('@/layouts/AdminLayout.vue'),
      children: [
        {
          path: '/admin',
          name: 'admin-home',
          component: () => import('@/views/admin/home.vue')
        },
        {
          path: '/admin/users',
          name: 'admin-user',
          component: () => import('@/views/admin/users.vue')
        }
      ]
    }
  ],
})

export default router
