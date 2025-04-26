<script setup lang="ts">
import { defineProps, type Component } from 'vue'

const { title, icon } = defineProps<{ title: string, icon?: Component }>()
</script>

<template>
    <hr>
    <strong class="d-flex w-100 align-items-center fw-semibold">
        <icon />
        {{ title }}
    </strong>
    <slot></slot>
</template>

<style scoped></style>