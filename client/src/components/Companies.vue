<template>
  <div>
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

    <v-card :loading="loading" title="Legg til ny kunde" class="ma-2 pa-2">
      <v-text-field v-model="search" v-on:keyup.enter="searchForCompanies" label="Søk etter firma" density="compact"
        prepend-inner-icon="mdi-magnify" variant="outlined" hide-details single-line>
        <template v-slot:append>
          <v-btn @click="searchForCompanies">
            Søk
          </v-btn>
        </template>
      </v-text-field>
      <v-data-table :headers="companyTableHeaders" :items="companiesFiltered" density="compact" items-per-page="10" hover>
        <template v-slot:item.action="{ item }">
          <v-icon class="me-2" size="small" title="Legg til" @click="openCompanyDialog(item)">
            mdi-plus
          </v-icon>
        </template>
      </v-data-table>
    </v-card>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
  name: "CompaniesView",
  data() {
    return {
      search: '',
      loading: false,
      companyTableHeaders: [
        { key: 'navn', title: 'Navn' },
        { key: 'organisasjonsnummer', title: 'Orgnummer' },
        { key: 'registreringsdatoEnhetsregisteret', title: 'Registrert' },
        { key: 'action', title: '', width: '30px', sortable: false },
      ],
      companyDialog: false,
      note: '',
      activeItem: null,
      timeout: null,
      companies: [],
    }
  },
  watch: {
    async search() {
      if (this.timeout != null) {
        clearTimeout(this.timeout)
      }
      this.timeout = setTimeout(async () => {
        await this.searchForCompanies()
      }, 500)
    },
  },
  computed: {
    ...mapGetters([
      'customers',
    ]),
    companiesFiltered() {
      return this.companies.filter(x => this.customers.find(c => c.organizationNumber == x.organisasjonsnummer) == null)
    },
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
    async upsertCustomer(item) {
      this.loading = true
      const payload = {
        organizationNumber: item.organisasjonsnummer,
        name: item.navn,
        note: this.note,
      }
      await this.$store.dispatch('upsertCustomer', payload)
      this.companyDialog = false
      this.loading = false
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
  },
}
</script>
