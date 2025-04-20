<template>
  <div>
    <v-dialog v-model="removeConfirmDialog" max-width="300px">
      <v-card>
        <v-card-title class="text-h5">Er du sikker?</v-card-title>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue-darken-1" variant="text" @click="closeDeleteDialog">Avbryt</v-btn>
          <v-btn color="blue-darken-1" variant="text" @click="removeCustomer(activeItem)">Fjern</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="editDialog" max-width="500px">
      <v-card>
        <v-card-title class="text-h5">{{ activeItem.name }}</v-card-title>
        <v-card-text>
          <v-textarea v-model="note" label="Notat" variant="outlined"></v-textarea>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue-darken-1" variant="text" @click="closeEditDialog">Lukk</v-btn>
          <v-btn color="blue-darken-1" variant="text" @click="openDeleteDialog">Fjern</v-btn>
          <v-btn color="blue-darken-1" variant="text" @click="upsertCustomer(activeItem)">Oppdater</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-card :loading="loading" title="Mine kunder" class="ma-2 pa-2 mt-6">
      <v-text-field v-model="customerFilter" label="Filter" density="compact" prepend-inner-icon="mdi-magnify"
        variant="outlined" hide-details single-line>
      </v-text-field>
      <v-data-table :headers="customerTableHeaders" :items="customers" :search="customerFilter" density="compact"
        items-per-page="10" hover>
        <template v-slot:item.action="{ item }">
          <v-icon class="me-2" size="small" title="Endre" @click="openEditDialog(item)">
            mdi-information-variant
          </v-icon>
        </template>
      </v-data-table>
    </v-card>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
  name: "CustomersView",
  async mounted() {
    await this.$store.dispatch('fetchCustomers')
  },
  data() {
    return {
      customerFilter: '',
      customerTableHeaders: [
        { key: 'name', title: 'Navn' },
        { key: 'organizationNumber', title: 'Orgnummer' },
        { key: 'action', title: '', width: '30px', sortable: false },
      ],
      editDialog: false,
      removeConfirmDialog: false,
      loading: false,
      activeItem: null,
      note: '',
    }
  },
  computed: {
    ...mapGetters([
      'customers',
    ])
  },
  methods: {
    openEditDialog(item) {
      this.activeItem = item
      this.note = item.note
      this.editDialog = true
    },
    closeEditDialog() {
      this.editDialog = false
      this.removeConfirmDialog = false
    },
    openDeleteDialog() {
      this.removeConfirmDialog = true
    },
    closeDeleteDialog() {
      this.removeConfirmDialog = false
    },
    async removeCustomer(item) {
      this.loading = true
      await this.$store.dispatch('removeCustomer', item)
      this.closeEditDialog()
      this.loading = false
    },
    async upsertCustomer(item) {
      this.loading = true
      item.note = this.note
      await this.$store.dispatch('upsertCustomer', item)
      this.closeEditDialog()
      this.loading = false
    },
  },
}
</script>
