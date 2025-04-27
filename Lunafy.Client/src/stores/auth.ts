import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        token: '' as string,
    }),
    actions: {
        setToken(newToken: string) {
            this.token = newToken
        },
        clearToken() {
            this.token = ''
        }
    }
})