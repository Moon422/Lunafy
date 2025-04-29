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
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue')
    },
    {
      path: '/admin',
      name: 'admin',
      component: () => import('@/layouts/AdminLayout.vue'),
      children: [
        {
          path: '',
          name: 'admin-default',
          component: () => import('@/views/admin/home.vue')
        },
        {
          path: 'home',
          name: 'admin-home',
          component: () => import('@/views/admin/home.vue')
        },
        {
          path: 'users',
          name: 'admin-users',
          component: () => import('@/views/admin/users.vue')
        },
        {
          path: 'tracks',
          name: 'admin-tracks',
          component: () => import('@/views/admin/tracks.vue')
        },
        {
          path: 'albums',
          name: 'admin-albums',
          component: () => import('@/views/admin/albums.vue')
        },
        {
          path: 'artists',
          name: 'admin-artists',
          component: () => import('@/views/admin/artists.vue')
        },
        {
          path: 'playlists',
          name: 'admin-playlists',
          component: () => import('@/views/admin/playlists.vue')
        },
        {
          path: 'analytics',
          name: 'admin-analytics',
          component: () => import('@/views/admin/analytics.vue')
        },
        {
          path: 'settings',
          name: 'admin-settings',
          component: () => import('@/views/admin/settings.vue')
        }
      ]
    }
  ],
})

export default router
