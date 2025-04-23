<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterView } from 'vue-router'

onMounted(() => {
  setTimeout(() => window.HSStaticMethods.autoInit(), 100)
})
</script>

<template>
  <!-- <header>
    <div class="wrapper">

      <nav>
        <RouterLink to="/">Home</RouterLink>
        <RouterLink to="/about">About</RouterLink>
        <RouterLink to="/admin">Admin</RouterLink>
      </nav>
    </div>
  </header> -->

  <RouterView />
</template>
