import { createStore } from 'vuex'
import axios from 'axios'

const store = createStore({
  state() {
    return {
      customers: [],
    }
  },
  mutations: {
    setCustomers(state, items) {
      state.customers = items || []
    }
  },
  actions: {
    async fetchCustomers({ commit }) {
      await axios.get(`/api/customer`).then(response => {
        commit('setCustomers', response.data)
      }).catch(err => {
        console.error(err)
      })
    },
    async upsertCustomer({ dispatch, state }, payload) {
      if (payload == null) {
        return
      }

      const existingCustomer = state.customers.find(c => c.organizationNumber == payload.organizationNumber)

      if (existingCustomer == null) {
        await axios.post('/api/customer', payload).then(async response => {
          if (response.status !== 200) {
            return
          }
          await dispatch('fetchCustomers')
        }).catch(err => {
          console.error(err)
        })
      } else {
        await axios.put('/api/customer', payload).then(async response => {
          if (response.status !== 200) {
            return
          }
          await dispatch('fetchCustomers')
        }).catch(err => {
          console.error(err)
        })
      }
    },
    async removeCustomer({ dispatch }, item) {
      if (item == null) {
        return
      }

      await axios.delete(`/api/customer/${item.organizationNumber}`).then(async response => {
        if (response.status !== 200) {
          return
        }
        await dispatch('fetchCustomers')
      }).catch(err => {
        console.error(err)
      })
    },
  },
  modules: {
  },
  getters: {
    customers(state) {
      return state.customers
    }
  }
})

export default store
