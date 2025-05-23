<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import type { HttpResponseModel } from '@/types/common'
import type { LoginResponseModel } from '@/types/user'
import { ref, defineProps, watch } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'

const baseUrl = import.meta.env.VITE_API_URL

const props = defineProps<{ redirectUrl?: string }>()

const username = ref<string>('')
const password = ref<string>('')

const router = useRouter()
const authStore = useAuthStore()

const loading = ref<boolean>(false)
const error = ref<string | null>(null)

watch(error, () => {
    if (error.value && error.value.length > 0) {
        toast.error(error.value, { onClose: () => error.value = null })
    }
})

const handleLogin = async (e: MouseEvent) => {
    loading.value = true

    const payload = {
        username: username.value,
        password: password.value
    }

    try {
        const response = await fetch(`${baseUrl}/api/user/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify(payload)
        })

        const { data, errors }: HttpResponseModel<LoginResponseModel> = await response.json()
        if (!response.ok) {
            const errorMsg = errors.find(el => el.length > 0) || "Something went wrong. Please try again."
            error.value = errorMsg
            return false
        }

        if (!data) {
            const errorMsg = "Something went wrong. Please try again."
            error.value = errorMsg
            return false
        }

        const { user: { firstname, lastname, email, isAdmin, isArtist }, jwt } = data

        authStore.setState({
            token: jwt, firstname, lastname, email, isAdmin, isArtist
        })

        router.push(props.redirectUrl || '/admin')
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        error.value = errorMessage
        throw err
    } finally {
        loading.value = false
    }
}
</script>

<template>
    <div class="login-container" @click="console.log('double fuck')">
        <h2>Login</h2>
        <form>
            <div class="form-group">
                <label for="username">Username</label>
                <input type="text" id="username" name="username" v-model="username" required>
            </div>
            <div class="form-group">
                <label for="password">Password</label>
                <input type="password" id="password" name="password" v-model="password" required>
            </div>
            <button type="submit" :disabled="loading" @click.prevent.stop="handleLogin">
                {{ loading ? 'Logging in' : 'Login' }}</button>
        </form>
    </div>
</template>

<style>
body {
    font-family: Arial, sans-serif;
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    margin: 0;
    background-color: #f0f0f0;
}

.login-container {
    background-color: white;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    width: 300px;
    text-align: center;
}

h2 {
    margin-bottom: 20px;
    color: #333;
}

.form-group {
    margin-bottom: 15px;
    text-align: left;
}

label {
    display: block;
    margin-bottom: 5px;
    color: #555;
}

input {
    width: 100%;
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    box-sizing: border-box;
}

button {
    width: 100%;
    padding: 10px;
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 16px;
}

button:hover {
    background-color: #0056b3;
}

button:disabled {
    background-color: #007bffc7;
    cursor: not-allowed;
}

.message {
    margin-top: 15px;
    padding: 10px;
    border-radius: 4px;
    display: none;
}

.success {
    background-color: #d4edda;
    color: #155724;
    display: block;
}

.error {
    background-color: #f8d7da;
    color: #721c24;
    display: block;
}
</style>