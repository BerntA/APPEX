<template>
  <v-app>
    <v-main>
      <v-container>
        <companies></companies>
        <customers></customers>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup>
import Companies from '@/components/Companies.vue'
import Customers from '@/components/Customers.vue'
</script>
