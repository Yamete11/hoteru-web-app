<template>
  <div class="reservation-component">
    <navbar />
    <sidebar />

    <div class="main">
      <h1 class="page-title">{{ state.isCreate ? 'New Reservation' : `${state.detailsType} Details` }}</h1>

      <form @submit.prevent="onSubmit" class="creating-form" :data-testid="state.isCreate ? 'reservation-create-form' : 'reservation-details-form'">
        <div v-if="state.isCreate" class="tab-switcher">
          <span :class="{ active: !uiForm.confirmed }" @click="uiForm.confirmed = false" data-testid="arrival-tab">Arrival</span>
          <span> - </span>
          <span :class="{ active: uiForm.confirmed }" @click="uiForm.confirmed = true" data-testid="reservation-tab">Reservation</span>
        </div>

        <div class="date-inputs">
          <div class="input-form">
            <label>In: </label>
            <input v-model="uiForm.in" class="input" type="date" :readonly="!state.isEditing && !state.isCreate" @input="v$.uiForm.in.$touch()" data-testid="date-in" />
            <span class="error-message" v-if="v$.uiForm.in.$error">
              <span v-if="v$.uiForm.in.required?.$invalid">The field is required*</span>
            </span>
            <span class="error-message" v-if="state.errors.In">{{ state.errors.In[0] }}</span>
          </div>
          <div class="input-form">
            <label>Out: </label>
            <input v-model="uiForm.out" class="input" type="date" :min="minOutDate" :readonly="!state.isEditing && !state.isCreate" @input="v$.uiForm.out.$touch()" data-testid="date-out" />
            <span class="error-message" v-if="v$.uiForm.out.$error">
              <span v-if="v$.uiForm.out.required?.$invalid">The field is required*</span>
              <span v-else-if="v$.uiForm.out.minValue?.$invalid">The out date must be after the in date*</span>
            </span>
            <span class="error-message" v-if="state.errors.Out">{{ state.errors.Out[0] }}</span>
          </div>
        </div>

        <div class="guest">
          <label>Room information</label>
          <div class="date-inputs">
            <div class="input-form">
              <label>Capacity: </label>
              <input v-model.number="uiForm.capacity" class="input" type="number" placeholder="Enter room capacity" :readonly="!state.isEditing && !state.isCreate" @input="v$.uiForm.capacity.$touch()" data-testid="capacity-input"/>
              <span class="error-message" v-if="v$.uiForm.capacity.$error">
                <span v-if="!v$.uiForm.capacity.required.$response">The field is required*</span>
                <span v-else-if="!v$.uiForm.capacity.numeric.$response">The field must contain only numeric*</span>
                <span v-else-if="!v$.uiForm.capacity.minValue.$response">The capacity must be greater than 0*</span>
                <span v-else-if="!v$.uiForm.capacity.maxValue.$response">The capacity must be equal or less than 40*</span>
              </span>
              <span class="error-message" v-if="state.errors.Capacity">{{ state.errors.Capacity[0] }}</span>
            </div>

            <div class="input-form">
              <label>Type: </label>
              <template v-if="!state.isEditing && !state.isCreate">
                <input class="input" type="text" :value="selectedRoomTypeTitle" readonly />
              </template>
              <template v-else>
                <select v-model="uiForm.idRoomType" class="input" @change="v$.uiForm.idRoomType.$touch()" data-testid="room-type-select">
                  <option disabled value="">Select type</option>
                  <option v-for="roomType in state.roomTypes" :key="roomType.idType" :value="roomType.idType" data-testid="room-type-option">{{ roomType.title }}</option>
                </select>
              </template>
              <span class="error-message" v-if="v$.uiForm.idRoomType.$error">
                <span v-if="!v$.uiForm.idRoomType.required.$response">The field is required*</span>
              </span>
              <span class="error-message" v-if="state.errors.IdRoomType">{{ state.errors.IdRoomType[0] }}</span>
            </div>
          </div>

          <div class="input-form">
            <label>Room Selection: </label>
            <template v-if="!state.isEditing && !state.isCreate">
              <input class="input" type="text" :value="selectedRoomLabel" readonly />
            </template>
            <template v-else>
              <select v-model="uiForm.idRoom" class="input" @change="v$.uiForm.idRoom.$touch()" data-testid="room-select">
                <option disabled value="">Select a room</option>
                <option v-for="room in sortedFilteredRooms" :key="room.idRoom" :value="room.idRoom" data-testid="room-option">
                  {{ room.number }} - Capacity: {{ room.capacity }}
                </option>
              </select>
            </template>
            <span class="error-message" v-if="v$.uiForm.idRoom.$error">
              <span v-if="!v$.uiForm.idRoom.required.$response">The field is required*</span>
            </span>
            <span class="error-message" v-if="state.errors.IdRoom">{{ state.errors.IdRoom[0] }}</span>
            <label>Price: {{ uiForm.price }}</label>
          </div>
        </div>

        <div class="guest">
          <label>Guest personal information</label>
          <div class="input-form">
            <label>Guest Selection: </label>
            <template v-if="!state.isEditing && !state.isCreate">
              <input class="input" type="text" :value="selectedGuestLabel" readonly />
            </template>
            <template v-else>
              <select v-model="uiForm.idGuest" class="input" @change="v$.uiForm.idGuest.$touch()" data-testid="guest-select">
                <option disabled value="">Select a guest</option>
                <option v-for="guest in state.guests" :key="guest.idPerson" :value="guest.idPerson" data-testid="guest-option">
                  {{ guest.name }} {{ guest.surname }}, {{ guest.passport }}
                </option>
              </select>
            </template>
            <span class="error-message" v-if="v$.uiForm.idGuest.$error">
              <span v-if="!v$.uiForm.idGuest.required.$response">The field is required*</span>
            </span>
            <span class="error-message" v-if="state.errors.IdGuest">{{ state.errors.IdGuest[0] }}</span>
            <router-link v-if="state.isEditing || state.isCreate" class="form-btn" to="/new-guest">Add new guest</router-link>
          </div>
        </div>

        <div class="guest">
          <label>Deposit</label>
          <div class="input-form">
            <template v-if="!state.isEditing && !state.isCreate && !state.hasDeposit">
              <label>There is no deposit.</label>
            </template>

            <template v-else-if="state.hasDeposit">
              <label>Deposit sum: </label>
              <input v-model.number="uiForm.depositSum" class="input" type="number" :readonly="!state.isEditing && !state.isCreate" @input="v$.uiForm.depositSum.$touch()" data-testid="deposit-input" />
              <span class="error-message" v-if="v$.uiForm.depositSum.$error">
                <span v-if="v$.uiForm.depositSum.required?.$invalid">The field is required*</span>
                <span v-else-if="v$.uiForm.depositSum.numeric?.$invalid">The field can contain only digits*</span>
                <span v-else-if="v$.uiForm.depositSum.minValue?.$invalid">The value must be greater than 0*</span>
              </span>
              <span class="error-message" v-if="state.errors.DepositSum">{{ state.errors.DepositSum[0] }}</span>

              <label>Choose type: </label>
              <template v-if="!state.isEditing && !state.isCreate">
                <input class="input" type="text" :value="selectedDepositTypeTitle" readonly />
              </template>
              <template v-else>
                <select v-model="uiForm.idDepositType" class="input" @change="v$.uiForm.idDepositType.$touch()" data-testid="deposit-select">
                  <option disabled value="">Select type</option>
                  <option v-for="type in state.depositTypes" :key="type.idType" :value="type.idType" data-testid="deposit-option">{{ type.title }}</option>
                </select>
              </template>
              <span class="error-message" v-if="v$.uiForm.idDepositType.$error">
                <span v-if="!v$.uiForm.idDepositType.required.$response">The field is required*</span>
              </span>
              <span class="error-message" v-if="state.errors.IdDepositType">{{ state.errors.IdDepositType[0] }}</span>
            </template>

            <template v-else>
              <label>There is no deposit.</label>
            </template>

            <button v-if="state.isEditing || state.isCreate" @click.prevent="toggleDeposit" class="form-btn" data-testid="add-deposit-btn">{{ state.hasDeposit ? 'Delete deposit' : 'Add deposit' }}</button>
          </div>
        </div>

        <div class="guest">
          <label>Service option</label>
          <div class="input-form">
            <label v-if="state.isEditing || state.isCreate">Choose a service</label>
            <label v-else-if="uiForm.services.length > 0">Added services</label>
            <label v-else>There are no services</label>

            <select v-if="state.isEditing || state.isCreate" v-model="state.selectedService" class="input" data-testid="service-select">
              <option disabled value="" selected>Select a service</option>
              <option v-for="service in state.services" :key="service.idService" :value="service" data-testid="service-option">
                {{ service.title }}: {{ service.sum }}
              </option>
            </select>
            <button v-if="state.isEditing || state.isCreate" @click.prevent="addService" class="form-btn">Add</button>

            <div class="service-list" v-if="uiForm.services.length > 0">
              <ul class="added-services-list">
                <li class="element" v-for="(service, index) in uiForm.services" :key="index">
                  <span>{{ service.title }}: {{ service.sum }}</span>
                  <button class="btn" v-if="state.isEditing || state.isCreate" @click.prevent="removeService(index)">Remove</button>
                </li>
              </ul>
            </div>
          </div>
        </div>

        <div class="registration-class">
          <router-link class="registration-btn" :to="state.isCreate ? '/arrivals' : '/arrivals'" data-testid="cancel-button">Cancel</router-link>

          <template v-if="!state.isCreate">
            <button v-if="!state.isEditing && uiForm.confirmed" class="registration-btn" type="button" @click="confirmReservation">Close</button>
            <button v-else-if="!state.isEditing && !uiForm.confirmed" class="registration-btn" type="button" @click="confirmReservation">Confirm</button>
            <button class="registration-btn" type="button" @click="toggleEdit">{{ state.isEditing ? 'Save' : 'Edit' }}</button>
          </template>

          <button v-else class="registration-btn" type="submit" data-testid="submit-button">Confirm</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { reactive, computed, watch, onMounted } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, helpers } from '@vuelidate/validators';
import { useStore } from 'vuex';
import { useRouter, useRoute, onBeforeRouteUpdate } from 'vue-router';
import { differenceInCalendarDays } from 'date-fns';
import { notify } from '@kyvg/vue3-notification';
import { toRaw } from 'vue';

import * as reservations from '@/api/reservations';
import * as roomsApi from '@/api/rooms';
import * as guestsApi from '@/api/guests';
import * as depositsApi from '@/api/deposits';
import * as servicesApi from '@/api/services';

export default {
  name: 'ReservationForm',
  props: {
    idReservation: { type: Number, required: false, default: null },
    detailsType: { type: String, default: 'Arrival' },
  },
  setup(props) {
    const store = useStore();
    const router = useRouter();
    const route = useRoute();

    const toArray = (x) => {
      if (Array.isArray(x)) return x;
      if (Array.isArray(x?.list)) return x.list;
      if (Array.isArray(x?.data)) return x.data;
      return [];
    };

    const today = new Date();
    const todayStr = today.toISOString().split('T')[0];
    const tomorrow = new Date(today);
    tomorrow.setDate(today.getDate() + 1);
    const tomorrowStr = tomorrow.toISOString().split('T')[0];

    const defaultUiForm = () => ({
      in: todayStr,
      out: tomorrowStr,
      capacity: 0,
      price: 0,
      idRoom: '',
      idRoomType: '',
      idGuest: '',
      confirmed: false,
      depositSum: '',
      idDepositType: '',
      services: [],
    });

    const state = reactive({
      isCreate: !props.idReservation,
      isEditing: !props.idReservation,
      detailsType: props.detailsType,
      uiForm: defaultUiForm(),
      rooms: [],
      roomTypes: [],
      guests: [],
      depositTypes: [],
      services: [],
      selectedService: null,
      errors: {},
      hasDeposit: false,
    });

    const uiForm = state.uiForm;

    const requiredIfHasDeposit = helpers.withAsync((value) => !state.hasDeposit || required.$validator(value));
    const numericIfHasDeposit = helpers.withAsync((value) => !state.hasDeposit || numeric.$validator(value));
    const requiredNonZero = helpers.withAsync((value) =>
        value !== '' && value !== 0 && value !== null && value !== undefined
    );

    const rules = {
      uiForm: {
        in: { required },
        out: { required, minValue: (val) => new Date(val) > new Date(uiForm.in) },
        capacity: { required, numeric, minValue: (v) => v > 0, maxValue: (v) => v <= 40 },
        idRoomType: { required },
        idRoom: { required },
        idGuest: { required },
        depositSum: {
          required: requiredIfHasDeposit,
          numeric: numericIfHasDeposit,
          minValue: (v) => !state.hasDeposit || v > 0,
        },
        idDepositType: { required: (value) => !state.hasDeposit || requiredNonZero(value) },
      },
    };
    const v$ = useVuelidate(rules, state);

    async function fetchCommon() {
      const [roomTypes, guestList, depositTypes, serviceList] = await Promise.all([
        roomsApi.types(),
        guestsApi.list(),
        depositsApi.list(),
        servicesApi.list(),
      ]);
      state.roomTypes = toArray(roomTypes);
      state.guests = toArray(guestList);
      state.depositTypes = toArray(depositTypes);
      state.services = toArray(serviceList);
    }

    async function fetchForCreate() {
      state.rooms = toArray(await roomsApi.free());
    }

    async function fetchForDetails(id) {
      const dto = await reservations.getArrival(id);
      const r = dto?.data ?? dto;

      uiForm.in = isoToLocalDate(r.in);
      uiForm.out = isoToLocalDate(r.out);
      uiForm.capacity = Number(r.capacity || 0);
      uiForm.idRoom = r.idRoom;
      uiForm.idRoomType = r.idRoomType;
      uiForm.idGuest = r.idGuest;
      uiForm.depositSum = r.depositSum ?? '';
      uiForm.idDepositType = r.idDepositType ?? '';
      uiForm.confirmed = !!r.confirmed;
      uiForm.services = Array.isArray(r.services) ? r.services : [];

      state.hasDeposit = Number(uiForm.idDepositType) > 0 || Number(uiForm.depositSum) > 0;

      state.rooms = toArray(await roomsApi.free(uiForm.idRoom));

      v$.value.$reset();
      recalcPrice();
    }

    const selectedRoomTypeTitle = computed(() => {
      const list = Array.isArray(state.roomTypes) ? state.roomTypes : [];
      const item = list.find((t) => String(t.idType) === String(uiForm.idRoomType));
      return item?.title ?? '';
    });

    const selectedDepositTypeTitle = computed(() => {
      const list = Array.isArray(state.depositTypes) ? state.depositTypes : [];
      const found = list.find((t) => String(t.idType) === String(uiForm.idDepositType));
      return found?.title ?? '';
    });

    const selectedRoomLabel = computed(() => {
      const list = Array.isArray(state.rooms) ? state.rooms : [];
      const r = list.find((x) => String(x.idRoom) === String(uiForm.idRoom));
      return r ? `${r.number} - Capacity: ${r.capacity}` : '';
    });

    const selectedGuestLabel = computed(() => {
      const list = Array.isArray(state.guests) ? state.guests : [];
      const g = list.find((x) => String(x.idPerson) === String(uiForm.idGuest));
      return g ? `${g.name} ${g.surname}, ${g.passport}` : '';
    });

    const sortedFilteredRooms = computed(() => {
      const roomsList = Array.isArray(state.rooms) ? state.rooms : [];
      const typesList = Array.isArray(state.roomTypes) ? state.roomTypes : [];
      let filtered = roomsList;

      if (uiForm.idRoomType) {
        const typeTitle = typesList.find((t) => String(t.idType) === String(uiForm.idRoomType))?.title;
        if (typeTitle) filtered = filtered.filter((r) => r.type === typeTitle);
      }
      if (uiForm.capacity) {
        filtered = filtered.filter((r) => Number(r.capacity) >= Number(uiForm.capacity));
      }
      return filtered.slice().sort((a, b) => a.capacity - b.capacity);
    });

    const minOutDate = computed(() => {
      if (uiForm.in) {
        const d = new Date(uiForm.in);
        d.setDate(d.getDate() + 1);
        return d.toISOString().split('T')[0];
      }
      return '';
    });

    watch(
        () => uiForm.in,
        (newIn) => {
          const inDate = new Date(newIn);
          const outDate = new Date(uiForm.out);
          if (!(outDate > inDate)) {
            const nd = new Date(inDate);
            nd.setDate(nd.getDate() + 1);
            uiForm.out = nd.toISOString().split('T')[0];
          }
        }
    );

    function recalcPrice() {
      if (!uiForm.in || !uiForm.out || !uiForm.idRoom) {
        uiForm.price = 0;
        return;
      }
      const out = new Date(uiForm.out);
      const ini = new Date(uiForm.in);
      const nights = differenceInCalendarDays(out, ini);
      const list = Array.isArray(state.rooms) ? state.rooms : [];
      const room = list.find((x) => String(x.idRoom) === String(uiForm.idRoom));
      uiForm.price = room && nights >= 0 ? Number(room.price) * nights : 0;
    }
    watch(() => [uiForm.in, uiForm.out, uiForm.idRoom], recalcPrice, { deep: true });

    function toggleDeposit() {
      state.hasDeposit = !state.hasDeposit;
      if (!state.hasDeposit) {
        uiForm.depositSum = '';
        uiForm.idDepositType = '';
      } else {
        if (!uiForm.idDepositType && state.depositTypes.length) {
          uiForm.idDepositType = state.depositTypes[0].idType;
        }
        if (!uiForm.depositSum) uiForm.depositSum = 1;
      }
      delete state.errors.DepositSum;
      delete state.errors.IdDepositType;
      v$.value.$reset();
    }

    function addService() {
      const s = state.selectedService;
      if (!s) return;
      const exists = uiForm.services.some((x) => String(x.idService) === String(s.idService));
      if (!exists) uiForm.services.push(s);
    }
    function removeService(idx) {
      uiForm.services.splice(idx, 1);
    }

    function isoToLocalDate(isoStr) {
      const date = new Date(isoStr);
      const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
      return local.toISOString().split('T')[0];
    }

    async function onSubmit() {
      v$.value.$touch();
      console.log(v$.value.$error)
      if (v$.value.$error) return;

      try {
        if (state.isCreate) {
          const rawServices = Array.isArray(uiForm.services) ? toRaw(uiForm.services) : [];

          const payload = {
            In: uiForm.in,
            Out: uiForm.out,
            Capacity: Number(uiForm.capacity),
            Price: Number(uiForm.price),
            IdRoom: Number(uiForm.idRoom),
            Confirmed: !!uiForm.confirmed,
            Sum: state.hasDeposit ? Number(uiForm.depositSum) : 0,
            IdDepositType: state.hasDeposit ? Number(uiForm.idDepositType) : 0,
            IdPerson: Number(uiForm.idGuest),
            IdUser: Number(store.getters.getUserData?.idUser),
            Services: rawServices.map(s => ({ IdService: Number(s.idService ?? s.IdService) })),
          };

          console.log('payload to send:', payload);

          const res = await reservations.create(payload);
          const body = res?.data ?? res ?? {};
          const code = body?.httpStatusCode ?? res?.status ?? 0;

          if (code === 200 || code === 201) {
            notify({
              title: 'Reservation Created',
              text: 'Reservation has been created successfully.',
              type: 'success',
              duration: 3000
            });
            await router.push({ path: '/arrivals', query: { created: 'true' } });
          } else {
            state.errors = body?.errors || {};
            notify({ title: 'Create failed', text: body?.message || 'Validation failed', type: 'error' });
          }
        }
      } catch (err) {
        state.errors = err?.response?.data?.errors || err?.details || {};
        notify({
          title: 'Create failed',
          text: err?.response?.data?.message || err?.message || 'Unexpected error',
          type: 'error'
        });
      }
    }

    async function toggleEdit() {
      if (!state.isEditing) {
        state.isEditing = true;
        v$.value.$reset();
        return;
      }
      v$.value.$touch();
      if (v$.value.$error) return;

      try {
        const payload = {
          idReservation: props.idReservation,
          in: uiForm.in,
          out: uiForm.out,
          capacity: Number(uiForm.capacity),
          idRoom: uiForm.idRoom,
          idRoomType: uiForm.idRoomType,
          idGuest: uiForm.idGuest,
          price: Number(uiForm.price),
          confirmed: !!uiForm.confirmed,
          depositSum: state.hasDeposit ? Number(uiForm.depositSum) : 0,
          idDepositType: state.hasDeposit ? uiForm.idDepositType : 0,
          services: uiForm.services,
        };
        const res = await reservations.update(payload);
        if (res?.httpStatusCode && res.httpStatusCode !== 200) {
          state.errors = res.errors || {};
          notify({ title: 'Update failed', text: res.message || 'Validation failed', type: 'error' });
          return;
        }
        notify({ title: 'Reservation Updated', text: 'Reservation details were successfully updated.', type: 'success', duration: 4000 });
        state.isEditing = false;
        v$.value.$reset();
      } catch (err) {
        state.errors = err?.details || {};
        notify({ title: 'Update failed', text: err?.message || 'Unexpected error', type: 'error' });
      }
    }

    async function confirmReservation() {
      try {
        const res = await reservations.confirm(props.idReservation);
        if (res?.httpStatusCode === 200) {
          notify({ title: 'Reservation Confirmed', text: 'Reservation has been confirmed.', type: 'success', duration: 3000 });
          await router.push({ path: '/arrivals', query: { confirmed: 'true' } });
        }
      } catch (err) {
        notify({ title: 'Confirmation failed', text: err?.message || 'Unexpected error', type: 'error' });
      }
    }

    function resetFormForCreate() {
      Object.assign(state.uiForm, defaultUiForm()); // СБРОС полей, не меняя ссылку
      state.errors = {};
      state.hasDeposit = false;
      v$.value.$reset();
    }

    async function initByMode() {
      await fetchCommon();
      if (props.idReservation) {
        state.isCreate = false;
        state.isEditing = false;
        await fetchForDetails(props.idReservation);
      } else {
        state.isCreate = true;
        state.isEditing = true;
        resetFormForCreate();
        await fetchForCreate();
      }
    }

    onMounted(async () => {
      try {
        await initByMode();
      } catch (e) {
        notify({ title: 'Load failed', text: e?.message || 'Failed to load data', type: 'error' });
      }
    });

    onBeforeRouteUpdate(async () => {
      try {
        await initByMode();
      } catch (e) {
        notify({ title: 'Load failed', text: e?.message || 'Failed to load data', type: 'error' });
      }
    });

    watch(() => [props.idReservation, props.detailsType], async () => {
      try {
        await initByMode();
      } catch (e) {
        notify({ title: 'Load failed', text: e?.message || 'Failed to load data', type: 'error' });
      }
    });

    return {
      state,
      uiForm,
      v$,
      minOutDate,
      selectedRoomTypeTitle,
      selectedDepositTypeTitle,
      selectedRoomLabel,
      selectedGuestLabel,
      sortedFilteredRooms,
      toggleDeposit,
      addService,
      removeService,
      onSubmit,
      toggleEdit,
      confirmReservation,
    };
  },
};
</script>



<style scoped>
.reservation-component {
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
  padding-top: 2vh;
  margin: 5%;
}
.creating-form {
  width: 100%;
  max-width: 720px;
  background: #fff;
  border-radius: 8px;
  padding: 24px 20px;
  box-shadow: 0 6px 16px rgba(0,0,0,.08);
  display: flex;
  flex-direction: column;
  gap: 14px;
}
.page-title,
h1 {
  margin: 0 0 8px;
  color: #000;
  text-align: center;
}
.registration-class {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}


.date-inputs {
  display: flex;
  justify-content: space-between;
  width: 100%;
}
.input-form {
  display: flex;
  flex-direction: column;
  position: relative;
  flex: 1;
  margin-right: 10px;
}

.input[readonly] {
  background: #f3f3f3;
  color: #666;
}
.input-form:last-child {
  margin-right: 0;
}
.input-form label {
  margin-bottom: 5px;
  font-weight: bold;
  color: black;
}
.input-form input[type="text"], .input-form input[type="date"], .input-form input[type="number"], .input-form select {
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-bottom: 10px;
}
.registration-btn,
.form-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px 16px;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-weight: 700;
  color: #fff;
  cursor: pointer;
  min-width: 120px;
  height: 44px;
  box-sizing: border-box;
}
.error-message {
  color: red;
  margin: 10px 0;
}
h1 {
  display: flex;
  justify-content: center;
  color: black;
}
.guest {
  display: flex;
  flex-direction: column;
  border: 2px solid #989595;
  border-radius: 5px;
  padding: 10px;
  margin-top: 20px;
}
.tab-switcher {
  display: flex;
  justify-content: center;
  margin: 12px 0 18px;
  gap: 12px;
}
.tab-switcher span {
  cursor: pointer;
  padding: 10px;
  border-bottom: 2px solid transparent;
}
.tab-switcher span.active {
  border-color: #8D7B68;
  font-weight: bold;
  font-size: 20px;
}

.element {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  margin: 10px 0;
  background-color: #C8B6A6;
  border-radius: 5px;
  font-weight: 700;
  font-size: 15px;
  box-shadow: 0 2px 4px rgba(0,0,0,.1);
}
.service-list {
  overflow-y: auto;
  border: 1px solid #000;
  border-radius: 5px;
  padding: 10px;
  margin: 10px 0;
}

.service-list ul {
  list-style: none;
  padding: 0;
  margin: 0;
  max-width: 800px;
}
.btn {
  padding: 8px 12px;
  font-size: .85rem;
  font-weight: 700;
  border-radius: 10px;
  border: 1px solid #D3C1AC;
  background-color: #444;
  color: #fff;
  cursor: pointer;
  transition: background-color .3s ease;
}

@media (max-width: 768px) {
  .creating-form { max-width: 100%; padding: 20px 16px; }
  .date-inputs { grid-template-columns: 1fr; }
  .registration-class { justify-content: stretch; }
  .registration-btn, .form-btn { flex: 1; }
}
</style>
