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
      path: '/forbidden',
      name: 'forbidden',
      component: () => import('@/views/Forbidden.vue')
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
          children: [
            {
              path: '',
              name: 'admin-users',
              component: () => import('@/views/admin/users.vue')
            },
            {
              path: ':id',
              name: 'admin-user-edit',
              component: () => import('@/views/admin/users/edit.vue')
            },
            {
              path: 'create',
              name: 'admin-user-create',
              component: () => import('@/views/admin/users/create.vue')
            }
          ]
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
          children: [
            {
              path: '',
              name: 'admin-artists',
              component: () => import('@/views/admin/artists.vue')
            },
            {
              path: ':id',
              name: 'admin-artist-edit',
              component: () => import('@/views/admin/artists/edit.vue')
            },
            {
              path: 'create',
              name: 'admin-artist-create',
              component: () => import('@/views/admin/artists/create.vue')
            }
          ]
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
