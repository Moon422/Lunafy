<script setup lang="ts">
import { defineProps, type Component } from 'vue'
const { label, icon } = defineProps<{ label: string, icon: Component }>()
</script>

<template>
    <div>
        <RouterLink to="#">
            <icon />
            {{ label }}
        </RouterLink>
    </div>
</template>

<style scoped></style>