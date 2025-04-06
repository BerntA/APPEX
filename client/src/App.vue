<template>
  <v-app>
    <v-main>
      <v-container>
        <v-dialog v-model="companyDialog" max-width="500px">
          <v-card>
            <v-card-title class="text-h5" v-if="activeItem">{{ activeItem.navn }}</v-card-title>
            <v-card-text>
              <v-textarea v-model="note" label="Notat" variant="outlined"></v-textarea>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue-darken-1" variant="text" @click="closeCompanyDialog">Avbryt</v-btn>
              <v-btn color="blue-darken-1" variant="text" @click="upsertCustomer(activeItem)">Lagre og legg til</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>

        <v-dialog v-model="deleteDialog" max-width="500px">
          <v-card>
            <v-card-title class="text-h5">Er du sikker?</v-card-title>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue-darken-1" variant="text" @click="closeDeleteDialog">Avbryt</v-btn>
              <v-btn color="blue-darken-1" variant="text" @click="removeCustomer(activeItem)">Fjern</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>

        <v-card :loading="loading" title="Legg til ny kunde" class="ma-2 pa-2">
          <v-text-field v-model="search" v-on:keyup.enter="searchForCompanies" label="Søk etter firma" density="compact"
            prepend-inner-icon="mdi-magnify" variant="outlined" hide-details single-line>
            <template v-slot:append>
              <v-btn @click="searchForCompanies">
                Søk
              </v-btn>
            </template>
          </v-text-field>
          <v-data-table :headers="companyTableHeaders" :items="companies" :search="search" density="compact"
            items-per-page="10" hover>
            <template v-slot:item.action="{ item }">
              <v-icon class="me-2" size="small" title="Legg til" @click="openCompanyDialog(item)">
                mdi-plus
              </v-icon>
            </template>
          </v-data-table>
        </v-card>

        <v-card title="Mine kunder" class="ma-2 pa-2 mt-6">
          <v-text-field v-model="customerFilter" label="Filter" density="compact" prepend-inner-icon="mdi-magnify" variant="outlined" hide-details single-line>
          </v-text-field>
          <v-data-table :headers="customerTableHeaders" :items="customers" :search="customerFilter" density="compact" items-per-page="10" hover
            show-expand>
            <template v-slot:item.action="{ item }">
              <v-icon class="me-2" size="small" title="Fjern" @click="openDeleteDialog(item)">
                mdi-delete
              </v-icon>
            </template>

            <template v-slot:expanded-row="{ columns, item }">
              <tr>
                <td :colspan="columns.length">
                  {{ item.note }}
                </td>
              </tr>
            </template>
          </v-data-table>
        </v-card>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
export default {
  name: "App",
  async mounted() {
    await this.getCustomers()
  },
  data() {
    return {
      search: '',
      customerFilter: '',
      loading: false,
      companies: [],
      customers: [],
      companyTableHeaders: [
        { key: 'navn', title: 'Navn' },
        { key: 'organisasjonsnummer', title: 'Orgnummer' },
        { key: 'registreringsdatoEnhetsregisteret', title: 'Registrert' },
        { key: 'action', title: '', width: '30px', sortable: false },
      ],
      customerTableHeaders: [
        { key: 'name', title: 'Navn' },
        { key: 'organizationNumber', title: 'Orgnummer' },
        { key: 'action', title: '', width: '30px', sortable: false },
      ],
      companyDialog: false,
      deleteDialog: false,
      note: '',
      activeItem: null,
    }
  },
  methods: {
    openCompanyDialog(item) {
      this.note = ''
      this.activeItem = item
      this.companyDialog = true
    },
    closeCompanyDialog() {
      this.activeItem = null
      this.companyDialog = false
    },
    openDeleteDialog(item) {
      this.activeItem = item
      this.deleteDialog = true
    },
    closeDeleteDialog() {
      this.activeItem = null
      this.deleteDialog = false
    },
    async upsertCustomer(item) {
      if (item == null) {
        return
      }

      const existingCustomer = this.customers.find(c => c.organizationNumber == item.organisasjonsnummer)
      const payload = {
        organizationNumber: item.organisasjonsnummer,
        name: item.navn,
        note: this.note,
      }

      if (existingCustomer == null) {
        await this.$http.post('/api/customer', payload).then(async response => {
          if (response.status !== 200) {
            return
          }
          await this.getCustomers()
        }).catch(err => {
          console.error(err)
        }).finally(() => {
          this.companyDialog = false
        })
      } else {
        await this.$http.put('/api/customer', payload).then(async response => {
          if (response.status !== 200) {
            return
          }
          await this.getCustomers()
        }).catch(err => {
          console.error(err)
        }).finally(() => {
          this.companyDialog = false
        })
      }
    },
    async removeCustomer(item) {
      if (item == null) {
        return
      }

      await this.$http.delete(`/api/customer/${item.organizationNumber}`).then(async response => {
        if (response.status !== 200) {
          return
        }
        await this.getCustomers()
      }).catch(err => {
        console.error(err)
      }).finally(() => {
        this.deleteDialog = false
      })
    },
    async searchForCompanies() {
      if (this.search == null || this.search.length == 0) {
        this.companies = []
        return
      }

      this.loading = true
      await this.$http.get(`/api/company/${this.search}`).then(response => {
        this.companies = response.data
      }).catch(err => {
        console.error(err)
      }).finally(() => {
        this.loading = false
      })
    },
    async getCustomers() {
      await this.$http.get(`/api/customer`).then(response => {
        this.customers = response.data
      }).catch(err => {
        console.error(err)
      })
    },
  },
}
</script>
