<template>
  <div class="history-component" data-testid="history-details-page">
    <navbar />
    <sidebar />

    <div class="main">
      <h1 class="page-title">History Details</h1>

      <div v-if="state.isLoading" class="loading" data-testid="history-loading">Loading...</div>

      <div v-else-if="state.error" class="error" data-testid="history-error">
        <p>{{ state.error }}</p>
        <router-link class="registration-btn" to="/history">Back</router-link>
      </div>

      <div v-else class="creating-form" data-testid="history-details">
        <div class="details-grid">
          <div class="row" data-testid="row-name">
            <span class="label">Name:</span>
            <span class="value">{{ display.nameFull }}</span>
          </div>
          <div class="row" data-testid="row-room-number">
            <span class="label">Room number:</span>
            <span class="value">{{ display.roomNumber }}</span>
          </div>
          <div class="row" data-testid="row-room-type">
            <span class="label">Room type:</span>
            <span class="value">{{ display.roomType }}</span>
          </div>
          <div class="row" data-testid="row-in">
            <span class="label">In:</span>
            <span class="value">{{ display.checkIn }}</span>
          </div>
          <div class="row" data-testid="row-out">
            <span class="label">Out:</span>
            <span class="value">{{ display.checkOut }}</span>
          </div>
          <div class="row" data-testid="row-booked-by">
            <span class="label">Booked by:</span>
            <span class="value">{{ display.bookedBy }}</span>
          </div>
          <div class="row" data-testid="row-deposit">
            <span class="label">Deposit:</span>
            <span class="value">{{ display.deposit }}</span>
          </div>
          <div class="row" data-testid="row-bill">
            <span class="label">Bill:</span>
            <span class="value">{{ display.bill }}</span>
          </div>
          <div class="row" data-testid="row-created">
            <span class="label">Bill created at:</span>
            <span class="value">{{ display.created }}</span>
          </div>
        </div>

        <div class="registration-class">
          <router-link class="registration-btn" to="/history">Back</router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { reactive, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { notify } from '@kyvg/vue3-notification';
import * as reservations from '@/api/reservations';

export default {
  name: 'HistoryDetails',
  props: {
    idReservation: { type: [Number, String], required: false, default: null },
  },
  setup(props) {
    const route = useRoute();

    const state = reactive({
      isLoading: true,
      error: '',
      reservation: {},
    });

    const fmtDateTime = (v) => {
      if (!v) return '';
      const d = new Date(v);
      return isNaN(d.getTime()) ? String(v) : d.toLocaleString();
    };

    const display = computed(() => ({
      nameFull: `${(state.reservation.name || '')} ${(state.reservation.surname || '')}`.trim(),
      roomNumber: state.reservation.roomNumber ?? '',
      roomType:   state.reservation.roomType  ?? '',
      checkIn: fmtDateTime(state.reservation.in),
      checkOut: fmtDateTime(state.reservation.out),
      deposit: [state.reservation.depositSum, state.reservation.depositType]
          .filter((v) => v !== undefined && v !== null && v !== '')
          .join(' '),
      bill: `${state.reservation.billSum ?? ''}`,
      created: fmtDateTime(state.reservation.created),
      bookedBy: state.reservation.bookedBy ?? ''
    }));

    async function fetchSpecificHistory(id) {
      state.isLoading = true;
      state.error = '';
      try {
        const dto = await reservations.getHistory(id);
        console.log(dto.data);
        state.reservation = dto.data || {};
      } catch (error) {
        state.error = error?.response?.data?.message || error?.message || 'Failed to load history details';
        notify({ title: 'Load failed', text: state.error, type: 'error' });
      } finally {
        state.isLoading = false;
      }
    }

    onMounted(() => {
      const id = Number(props.idReservation || route.params?.idReservation);
      if (!id) {
        state.isLoading = false;
        state.error = 'Invalid reservation id';
        return;
      }
      fetchSpecificHistory(id);
    });

    return { state, reservation: state.reservation, display };
  },
};
</script>

<style scoped>
.history-component {
  display: flex;
  flex-direction: column;
  background-color: #F1DEC9;
  min-height: 100vh;
}
.main {
  display: flex;
  align-items: center;
  flex-direction: column;
  flex-grow: 1;
  padding-top: 8vh;
  margin: 0 auto;
  width: 80%;
}
.page-title{ color:#000; }
.creating-form {
  display: flex;
  align-items: stretch;
  flex-direction: column;
  width: 100%;
  max-width: 700px;
}

.details-grid {
  display: grid;
  grid-template-columns: 180px 1fr;
  gap: 10px 16px;
  width: 100%;
  background: #ffffff66;
  border: 1px solid #ccc;
  border-radius: 8px;
  padding: 16px;
}
.row {
  display: contents;
}
.label {
  font-weight: 700;
  color: #000;
}
.value {
  color: #000;
}

.loading { margin: 20px 0; }
.error { margin: 20px 0; color: #b00020; }

.registration-btn{
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-weight: bold;
  color: white;
  margin: 16px 10px 0;
  cursor: pointer;
  display: inline-block;
}
.registration-class{
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
}
</style>
