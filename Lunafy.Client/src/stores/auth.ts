import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
    state: (): {
        token: string | null,
        firstname: string | null,
        lastname: string | null,
        email: string | null,
        isAdmin: boolean,
        isArtist: boolean,
    } => ({
        token: null,
        firstname: null,
        lastname: null,
        email: null,
        isAdmin: false,
        isArtist: false,
    }),
    actions: {
        setState({ token, firstname, lastname, email, isAdmin, isArtist }: {
            token: string | null,
            firstname: string | null,
            lastname: string | null,
            email: string | null,
            isAdmin: boolean,
            isArtist: boolean,
        }) {
            this.token = token
            this.firstname = firstname
            this.lastname = lastname
            this.email = email
            this.isAdmin = isAdmin
            this.isArtist = isArtist
        },
        clearState() {
            this.token = null
            this.firstname = null
            this.lastname = null
            this.email = null
            this.isAdmin = false
            this.isArtist = false
        }
    }
})