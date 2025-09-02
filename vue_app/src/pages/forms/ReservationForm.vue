<template>
  <div class="reservation-component" data-testid="reservation-component">
    <navbar />
    <sidebar />

    <div class="main">
      <h1 class="page-title" data-testid="reservation-title">
        {{ state.isCreate ? 'New Reservation' : `${state.detailsType} Details` }}
      </h1>

      <form
        @submit.prevent="onSubmit"
        class="creating-form"
        :data-testid="state.isCreate ? 'reservation-create-form' : 'reservation-details-form'"
      >
        <div v-if="state.isCreate" class="tab-switcher" data-testid="tab-switcher">
          <span :class="{ active: !uiForm.confirmed }" @click="uiForm.confirmed = false" data-testid="arrival-tab">Arrival</span>
          <span> - </span>
          <span :class="{ active: uiForm.confirmed }" @click="uiForm.confirmed = true" data-testid="reservation-tab">Reservation</span>
        </div>

        <div class="date-inputs" data-testid="date-inputs">
          <div class="input-form">
            <label>In: </label>
            <input
              v-model="uiForm.in"
              class="input"
              type="date"
              :readonly="!state.isEditing && !state.isCreate"
              @input="touchField('in')"
              data-testid="date-in"
            />
            <span class="error-message" v-if="v$.uiForm.in.$error" data-testid="date-in-error">
              {{ v$.uiForm.in.$errors[0]?.$message || 'The field is required*' }}
            </span>
            <span class="error-message" v-if="state.errors.In" data-testid="date-in-api-error">{{ state.errors.In[0] }}</span>
          </div>

          <div class="input-form">
            <label>Out: </label>
            <input
              v-model="uiForm.out"
              class="input"
              type="date"
              :min="minOutDate"
              :readonly="!state.isEditing && !state.isCreate"
              @input="touchField('out')"
              data-testid="date-out"
            />
            <span class="error-message" v-if="v$.uiForm.out.$error" data-testid="date-out-error">
              {{ v$.uiForm.out.$errors[0]?.$message || 'The out date must be after the in date*' }}
            </span>
            <span class="error-message" v-if="state.errors.Out" data-testid="date-out-api-error">{{ state.errors.Out[0] }}</span>
          </div>
        </div>

        <div class="guest" data-testid="room-section">
          <label>Room information</label>

          <template v-if="!state.roomAttached && (state.isEditing || state.isCreate)">
            <div class="date-inputs">
              <div class="input-form">
                <label>Capacity: </label>
                <input
                  v-model.number="uiForm.capacity"
                  class="input"
                  type="number"
                  placeholder="Enter room capacity"
                  :readonly="!state.isEditing && !state.isCreate"
                  @input="touchField('capacity')"
                  data-testid="capacity-input"
                />
                <span class="error-message" v-if="v$.uiForm.capacity.$error" data-testid="capacity-error">
                  {{ v$.uiForm.capacity.$errors[0]?.$message || 'The capacity must be from 1 to 40*' }}
                </span>
                <span class="error-message" v-if="state.errors.Capacity" data-testid="capacity-api-error">{{ state.errors.Capacity[0] }}</span>
              </div>

              <div class="input-form">
                <label>Type: </label>
                <template v-if="!state.isEditing && !state.isCreate">
                  <input class="input" type="text" :value="selectedRoomTypeTitle" readonly data-testid="room-type-readonly" />
                </template>
                <template v-else>
                  <select
                    v-model.number="uiForm.idRoomType"
                    class="input"
                    @change="touchField('idRoomType')"
                    data-testid="room-type-select"
                  >
                    <option disabled :value="0">Select type</option>
                    <option
                      v-for="roomType in state.roomTypes"
                      :key="roomType.idType"
                      :value="Number(roomType.idType)"
                      data-testid="room-type-option"
                    >
                      {{ roomType.title }}
                    </option>
                  </select>
                </template>
                <span class="error-message" v-if="v$.uiForm.idRoomType.$error" data-testid="room-type-error">
                  {{ v$.uiForm.idRoomType.$errors[0]?.$message || 'The field is required*' }}
                </span>
                <span class="error-message" v-if="state.errors.IdRoomType" data-testid="room-type-api-error">{{ state.errors.IdRoomType[0] }}</span>
              </div>
            </div>

            <div class="input-form" v-if="(state.isEditing || state.isCreate) && !state.roomAttached">
              <label>Room Selection: </label>

              <template v-if="canPickRoom">
                <select
                  v-model.number="state.tempRoomId"
                  class="input"
                  data-testid="room-select"
                >
                  <option disabled :value="0">Select a room</option>
                  <option
                    v-for="room in sortedFilteredRooms"
                    :key="room.idRoom"
                    :value="Number(room.idRoom)"
                    data-testid="room-option"
                  >
                    {{ room.number }} - Capacity: {{ room.capacity }}
                  </option>
                </select>

                <span class="error-message" v-if="v$.uiForm.idRoom.$error && canPickRoom" data-testid="room-select-error">
                  {{ v$.uiForm.idRoom.$errors[0]?.$message || 'The field is required*' }}
                </span>
                <span class="error-message" v-if="state.errors.IdRoom" data-testid="room-select-api-error">{{ state.errors.IdRoom[0] }}</span>

                <button
                  class="form-btn"
                  type="button"
                  @click.prevent="addRoom"
                  :disabled="!state.tempRoomId || !canPickRoom"
                  data-testid="add-room-btn"
                >
                  Add room
                </button>
              </template>

              <template v-else>
                <div class="muted" data-testid="room-hint">Enter capacity and choose a room type to see available rooms.</div>
              </template>
            </div>

          </template>

          <template v-else>
            <div class="card guest-card" data-testid="selected-room-card">
              <div class="guest-row">
                <span class="guest-name" data-testid="selected-room-number">Room #{{ selectedRoom?.number }}</span>
                <span class="guest-passport" data-testid="selected-room-capacity">Capacity: {{ selectedRoom?.capacity }}</span>
              </div>
              <div class="guest-row muted">
                <span v-if="selectedRoom?.type" data-testid="selected-room-type">Type: {{ selectedRoom.type }}</span>
                <span v-if="selectedRoom?.price" data-testid="selected-room-price">Price/night: {{ selectedRoom.price }}</span>
              </div>
            </div>

            <button
              v-if="state.isEditing || state.isCreate"
              class="form-btn danger"
              type="button"
              @click.prevent="removeRoom"
              data-testid="remove-room-btn"
            >
              Remove room
            </button>
          </template>
        </div>

        <div class="guest" data-testid="guest-section">
          <label>Guest personal information</label>
          <div class="input-form">
            <template v-if="state.isEditing || state.isCreate">
              <template v-if="!state.guestAttached">
                <label>Guest Selection:</label>
                <select
                  v-model.number="state.tempGuestId"
                  class="input"
                  data-testid="guest-select"
                >
                  <option disabled :value="0">Select a guest</option>
                  <option
                    v-for="guest in state.guests"
                    :key="guest.idPerson"
                    :value="Number(guest.idPerson)"
                    data-testid="guest-option"
                  >
                    {{ guest.name }} {{ guest.surname }}, {{ guest.passport }}
                  </option>
                </select>

                <span class="error-message" v-if="v$.uiForm.idGuest.$error" data-testid="guest-select-error">
                  {{ v$.uiForm.idGuest.$errors[0]?.$message || 'The field is required*' }}
                </span>
                <span class="error-message" v-if="state.errors.IdGuest" data-testid="guest-select-api-error">{{ state.errors.IdGuest[0] }}</span>

                <button
                  class="form-btn"
                  type="button"
                  @click.prevent="addGuest"
                  :disabled="!state.tempGuestId"
                  data-testid="add-guest-btn"
                >
                  Add guest
                </button>
              </template>

              <template v-else>
                <div class="card guest-card" data-testid="selected-guest-card">
                  <div class="guest-row">
                    <span class="guest-name" data-testid="selected-guest-name">{{ selectedGuest?.name }} {{ selectedGuest?.surname }}</span>
                    <span class="guest-passport" data-testid="selected-guest-passport">{{ selectedGuest?.passport }}</span>
                  </div>
                  <div class="guest-row muted">
                    <span v-if="selectedGuest?.email" data-testid="selected-guest-email">✉️ {{ selectedGuest.email }}</span>
                  </div>
                </div>
                <button class="form-btn danger" type="button" @click.prevent="removeGuest" data-testid="remove-guest-btn">Remove guest</button>
              </template>
            </template>

            <template v-else>
              <div class="card guest-card" data-testid="readonly-guest-card">
                <div class="guest-row">
                  <span class="guest-name" data-testid="readonly-guest-name">{{ selectedGuest?.name }} {{ selectedGuest?.surname }}</span>
                  <span class="guest-passport" data-testid="readonly-guest-passport">{{ selectedGuest?.passport }}</span>
                </div>
                <div class="guest-row muted">
                  <span v-if="selectedGuest?.email" data-testid="readonly-guest-email">✉️ {{ selectedGuest.email }}</span>
                </div>
              </div>
            </template>
          </div>
        </div>

        <div class="guest" data-testid="deposit-section">
          <label>Deposit</label>
          <div class="input-form">
            <template v-if="!state.isEditing && !state.isCreate && !state.hasDeposit">
              <label data-testid="no-deposit-label">There is no deposit.</label>
            </template>

            <template v-else-if="state.hasDeposit">
              <label>Deposit sum: </label>
              <input
                v-model.number="uiForm.depositSum"
                class="input"
                type="number"
                :readonly="!state.isEditing && !state.isCreate"
                @input="touchField('depositSum')"
                data-testid="deposit-input"
              />
              <span class="error-message" v-if="v$.uiForm.depositSum.$error" data-testid="deposit-input-error">
                {{ v$.uiForm.depositSum.$errors[0]?.$message || 'Deposit sum is required*' }}
              </span>
              <span class="error-message" v-if="state.errors.DepositSum" data-testid="deposit-input-api-error">{{ state.errors.DepositSum[0] }}</span>

              <label>Choose type: </label>
              <template v-if="!state.isEditing && !state.isCreate">
                <input class="input" type="text" :value="selectedDepositTypeTitle" readonly data-testid="deposit-type-readonly" />
              </template>
              <template v-else>
                <select
                  v-model.number="uiForm.idDepositType"
                  class="input"
                  @change="touchField('idDepositType')"
                  data-testid="deposit-select"
                >
                  <option disabled :value="0">Select type</option>
                  <option
                    v-for="type in state.depositTypes"
                    :key="type.idType"
                    :value="Number(type.idType)"
                    data-testid="deposit-option"
                  >
                    {{ type.title }}
                  </option>
                </select>
              </template>

              <span class="error-message" v-if="v$.uiForm.idDepositType.$error" data-testid="deposit-select-error">
                {{ v$.uiForm.idDepositType.$errors[0]?.$message || 'Deposit type is required*' }}
              </span>
              <span class="error-message" v-if="state.errors.IdDepositType" data-testid="deposit-select-api-error">{{ state.errors.IdDepositType[0] }}</span>
            </template>

            <template v-else>
              <label data-testid="no-deposit-label">There is no deposit.</label>
            </template>

            <button
              v-if="state.isEditing || state.isCreate"
              @click.prevent="toggleDeposit"
              :class="state.hasDeposit ? 'form-btn danger' : 'form-btn'"
              data-testid="add-deposit-btn"
              type="button"
            >
              {{ state.hasDeposit ? 'Remove deposit' : 'Add deposit' }}
            </button>
          </div>
        </div>

        <div class="guest" data-testid="services-section">
          <label>Service option</label>
          <div class="input-form">
            <label v-if="state.isEditing || state.isCreate" data-testid="service-select-label">Choose a service</label>
            <label v-else-if="uiForm.services.length > 0" data-testid="service-list-label">Added services</label>
            <label v-else data-testid="service-empty-label">There are no services</label>

            <select
              v-if="state.isEditing || state.isCreate"
              v-model="state.selectedService"
              class="input"
              data-testid="service-select"
            >
              <option disabled value="0">Select a service</option>
              <option
                v-for="service in state.services"
                :key="service.idService"
                :value="service"
                data-testid="service-option"
              >
                {{ service.title }}: {{ service.sum }}
              </option>
            </select>
            <button
              v-if="state.isEditing || state.isCreate"
              @click.prevent="addService"
              class="form-btn"
              type="button"
              data-testid="add-service-btn"
            >
              Add
            </button>

            <div class="service-list" v-if="uiForm.services.length > 0" data-testid="added-services">
              <ul class="added-services-list">
                <li class="element" v-for="(service, index) in uiForm.services" :key="index" data-testid="added-service-item">
                  <span class="service-title" data-testid="added-service-title">{{ service.title }}: {{ service.sum }}</span>
                  <div class="service-actions">
                    <button
                      class="btn"
                      v-if="state.isEditing || state.isCreate"
                      @click.prevent="removeService(index)"
                      type="button"
                      :data-testid="'remove-service-btn-' + index"
                    >
                      Remove
                    </button>
                  </div>
                </li>
              </ul>
            </div>

          </div>
        </div>

        <div class="guest" data-testid="summary-section">
          <label>Summary</label>
          <div class="input-form">
            <div class="summary-row">
              <span>Room:</span>
              <strong data-testid="summary-room">{{ roomTotal }}</strong>
            </div>
            <div class="summary-row">
              <span>Services:</span>
              <strong data-testid="summary-services">{{ servicesTotal }}</strong>
            </div>
            <hr class="summary-sep" />
            <div class="summary-row total">
              <span>Total:</span>
              <strong data-testid="summary-total">{{ grandTotal }}</strong>
            </div>
          </div>
        </div>

        <div class="registration-class" data-testid="actions">
          <router-link class="registration-btn" :to="state.isCreate ? '/arrivals' : '/arrivals'" data-testid="cancel-button">Cancel</router-link>

          <template v-if="!state.isCreate">
            <button
              v-if="!state.isEditing && uiForm.confirmed"
              class="registration-btn"
              type="button"
              @click="confirmReservation"
              data-testid="close-button"
            >
              Close
            </button>
            <button
              v-else-if="!state.isEditing && !uiForm.confirmed"
              class="registration-btn"
              type="button"
              @click="confirmReservation"
              data-testid="confirm-button"
            >
              Confirm
            </button>
            <button
              class="registration-btn"
              type="button"
              @click="toggleEdit"
              :data-testid="state.isEditing ? 'save-button' : 'edit-button'"
            >
              {{ state.isEditing ? 'Save' : 'Edit' }}
            </button>
          </template>

          <button v-else class="registration-btn" type="submit" data-testid="submit-button">Confirm</button>
        </div>
      </form>
    </div>
  </div>
</template>



<script>
import { reactive, computed, watch, onMounted, nextTick, toRaw } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, helpers } from '@vuelidate/validators';
import { useStore } from 'vuex';
import { useRouter, useRoute, onBeforeRouteUpdate } from 'vue-router';
import { differenceInCalendarDays } from 'date-fns';
import { notify } from '@kyvg/vue3-notification';

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
      idRoom: 0,
      idRoomType: 0,
      idGuest: 0,
      confirmed: false,
      depositSum: '',
      idDepositType: 0,
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
      selectedService: 0,
      errors: {},
      hasDeposit: false,
      guestAttached: false,
      tempGuestId: 0,
      roomAttached: false,
      tempRoomId: 0,
    });

    const uiForm = state.uiForm;

    const afterInMessage = helpers.withMessage(
        'The out date must be after the in date*',
        (val) => !!val && new Date(val) > new Date(uiForm.in)
    );

    const selectedGtZero = helpers.withMessage(
        'The field is required*',
        (v) => Number(v) > 0
    );

    const capMin = helpers.withMessage('The capacity must be greater than 0*', (v) => Number(v) > 0);
    const capMax = helpers.withMessage('The capacity must be equal or less than 40*', (v) => Number(v) <= 40);

    const requiredIfHasDeposit = helpers.withMessage(
        'The field is required*',
        (value) => !state.hasDeposit || required.$validator(value)
    );
    const numericIfHasDeposit = helpers.withMessage(
        'The field can contain only digits*',
        (value) => !state.hasDeposit || numeric.$validator(value)
    );
    const depositPositive = helpers.withMessage(
        'The value must be greater than 0*',
        (value) => !state.hasDeposit || Number(value) > 0
    );
    const depositTypeRequired = helpers.withMessage(
        'Deposit type is required*',
        (value) => !state.hasDeposit || Number(value) > 0
    );

    const rules = {
      uiForm: {
        in: { required: helpers.withMessage('The field is required*', required) },
        out: { required: helpers.withMessage('The field is required*', required), afterInMessage },
        capacity: { required: helpers.withMessage('The field is required*', required), numeric, capMin, capMax },
        idRoomType: { selectedGtZero },
        idRoom: { selectedGtZero },
        idGuest: { selectedGtZero },
        depositSum: { required: requiredIfHasDeposit, numeric: numericIfHasDeposit, depositPositive },
        idDepositType: { depositTypeRequired },
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

    const canPickRoom = computed(() => Number(uiForm.capacity) > 0 && Number(uiForm.idRoomType) > 0);
    watch([() => uiForm.capacity, () => uiForm.idRoomType], () => {
      if (!state.roomAttached) {
        state.tempRoomId = 0;
        delete state.errors.IdRoom;
        v$.value?.uiForm?.idRoom?.$reset?.();
      }
    });

    async function fetchForDetails(id) {
      const dto = await reservations.getArrival(id);
      const r = dto?.data ?? dto;

      uiForm.in = isoToLocalDate(r.in);
      uiForm.out = isoToLocalDate(r.out);
      uiForm.capacity = Number(r.capacity || 0);
      uiForm.idRoom = Number(r.idRoom || 0);
      uiForm.idRoomType = Number(r.idRoomType || 0);
      uiForm.idGuest = Number(r.idGuest || 0);
      uiForm.depositSum = r.depositSum ?? '';
      uiForm.idDepositType = Number(r.idDepositType || 0);
      uiForm.confirmed = !!r.confirmed;
      uiForm.services = Array.isArray(r.services) ? r.services : [];

      state.hasDeposit = Number(uiForm.idDepositType) > 0 || Number(uiForm.depositSum) > 0;
      state.guestAttached = uiForm.idGuest > 0;
      state.tempGuestId = 0;

      state.roomAttached = uiForm.idRoom > 0;
      state.tempRoomId = 0;

      state.rooms = toArray(await roomsApi.free(uiForm.idRoom));

      v$.value.$reset();
      recalcPrice();
    }

    const selectedRoomTypeTitle = computed(() => {
      const item = (state.roomTypes || []).find((t) => String(t.idType) === String(uiForm.idRoomType));
      return item?.title ?? '';
    });

    const selectedDepositTypeTitle = computed(() => {
      const item = (state.depositTypes || []).find((t) => String(t.idType) === String(uiForm.idDepositType));
      return item?.title ?? '';
    });

    const selectedRoomLabel = computed(() => {
      const r = (state.rooms || []).find((x) => String(x.idRoom) === String(uiForm.idRoom));
      return r ? `${r.number} - Capacity: ${r.capacity}` : '';
    });

    const selectedGuest = computed(() => {
      return (state.guests || []).find((x) => String(x.idPerson) === String(uiForm.idGuest)) || null;
    });

    const selectedGuestLabel = computed(() => {
      const g = selectedGuest.value;
      return g ? `${g.name} ${g.surname}, ${g.passport}` : '';
    });

    const selectedRoom = computed(() => {
      return (state.rooms || []).find((x) => String(x.idRoom) === String(uiForm.idRoom)) || null;
    });

    const sortedFilteredRooms = computed(() => {
      let filtered = [...(state.rooms || [])];
      if (uiForm.idRoomType) {
        const typeTitle = (state.roomTypes || []).find((t) => String(t.idType) === String(uiForm.idRoomType))?.title;
        if (typeTitle) filtered = filtered.filter((r) => r.type === typeTitle);
      }
      if (uiForm.capacity) {
        filtered = filtered.filter((r) => Number(r.capacity) >= Number(uiForm.capacity));
      }
      return filtered.sort((a, b) => a.capacity - b.capacity);
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
      const room = (state.rooms || []).find((x) => String(x.idRoom) === String(uiForm.idRoom));
      uiForm.price = room && nights >= 0 ? Number(room.price) * nights : 0;
    }
    watch(() => [uiForm.in, uiForm.out, uiForm.idRoom], recalcPrice, { deep: true });

    const roomTotal = computed(() => Number(uiForm.price) || 0);
    const servicesTotal = computed(() =>
        (Array.isArray(uiForm.services) ? uiForm.services : []).reduce(
            (acc, s) => acc + Number(s.sum ?? s.Sum ?? 0),
            0
        )
    );
    const grandTotal = computed(() => roomTotal.value + servicesTotal.value);

    function touchField(name) {
      const node = v$.value?.uiForm?.[name];
      if (node && typeof node.$touch === 'function') node.$touch();
    }

    function resetDepositValidation() {
      const v = v$.value?.uiForm;
      v?.idDepositType?.$reset?.();
      v?.depositSum?.$reset?.();
    }

    function toggleDeposit() {
      state.hasDeposit = !state.hasDeposit;
      if (!state.hasDeposit) {
        uiForm.depositSum = '';
        uiForm.idDepositType = 0;
      } else {
        if (!uiForm.idDepositType && state.depositTypes.length) {
          uiForm.idDepositType = Number(state.depositTypes[0].idType);
        }
        if (!uiForm.depositSum) uiForm.depositSum = 1;
      }
      delete state.errors.DepositSum;
      delete state.errors.IdDepositType;
      nextTick().then(resetDepositValidation);
    }

    function addGuest() {
      if (!Number(state.tempGuestId)) {
        v$.value?.uiForm?.idGuest?.$touch?.();
        return;
      }
      uiForm.idGuest = Number(state.tempGuestId);
      state.guestAttached = true;
      state.tempGuestId = 0;
    }
    function removeGuest() {
      state.guestAttached = false;
      uiForm.idGuest = 0;
      state.tempGuestId = 0;
      delete state.errors.IdGuest;
      v$.value?.uiForm?.idGuest?.$reset?.();
    }

    function addRoom() {
      if (!Number(state.tempRoomId)) {
        v$.value?.uiForm?.idRoom?.$touch?.();
        return;
      }
      uiForm.idRoom = Number(state.tempRoomId);
      state.roomAttached = true;
      state.tempRoomId = 0;
      delete state.errors.IdRoom;
    }
    function removeRoom() {
      state.roomAttached = false;
      uiForm.idRoom = 0;
      state.tempRoomId = 0;
      delete state.errors.IdRoom;
      v$.value?.uiForm?.idRoom?.$reset?.();
    }

    function addService() {
      const s = state.selectedService;
      if (!s || s === 0) return;
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
      if (v$.value.$error) return;

      try {
        if (state.isCreate) {
          const rawServices = Array.isArray(uiForm.services) ? toRaw(uiForm.services) : [];
          const payload = {
            In: uiForm.in,
            Out: uiForm.out,
            Capacity: Number(uiForm.capacity),
            Price: Number(grandTotal.value),
            IdRoom: Number(uiForm.idRoom),
            Confirmed: !!uiForm.confirmed,
            Sum: state.hasDeposit ? Number(uiForm.depositSum) : 0,
            IdDepositType: state.hasDeposit ? Number(uiForm.idDepositType) : 0,
            IdPerson: Number(state.guestAttached ? uiForm.idGuest : 0),
            IdUser: Number(store.getters.getUserData?.idUser),
            Services: rawServices.map((s) => ({ IdService: Number(s.idService ?? s.IdService) })),
          };

          const res = await reservations.create(payload);
          const body = res?.data ?? res ?? {};
          const code = body?.httpStatusCode ?? res?.status ?? 0;

          if (code === 200 || code === 201) {
            notify({ title: 'Reservation Created', text: 'Reservation has been created successfully.', type: 'success', duration: 3000 });
            await router.push({ path: '/arrivals', query: { created: 'true' } });
          } else {
            state.errors = body?.errors || {};
            notify({ title: 'Create failed', text: body?.message || 'Validation failed', type: 'error' });
          }
        }
      } catch (err) {
        state.errors = err?.response?.data?.errors || err?.details || {};
        notify({ title: 'Create failed', text: err?.response?.data?.message || err?.message || 'Unexpected error', type: 'error' });
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
          price: Number(grandTotal.value),
          confirmed: !!uiForm.confirmed,
          depositSum: state.hasDeposit ? Number(uiForm.depositSum) : 0,
          idDepositType: state.hasDeposit ? uiForm.idDepositType : 0,
          services: uiForm.services,
        };
        const res = await reservations.update(payload);
        const body = res?.data ?? res ?? {};
        const code = body?.httpStatusCode ?? res?.status ?? 0;

        if (code && code !== 200) {
          state.errors = body?.errors || {};
          notify({ title: 'Update failed', text: body?.message || 'Validation failed', type: 'error' });
          return;
        }
        notify({ title: 'Reservation Updated', text: 'Reservation details were successfully updated.', type: 'success', duration: 4000 });
        state.isEditing = false;
        v$.value.$reset();
      } catch (err) {
        state.errors = err?.response?.data?.errors || err?.details || {};
        notify({ title: 'Update failed', text: err?.response?.data?.message || err?.message || 'Unexpected error', type: 'error' });
      }
    }

    async function confirmReservation() {
      try {
        const res = await reservations.confirm(props.idReservation);
        const code = res?.data?.httpStatusCode ?? res?.httpStatusCode ?? res?.status ?? 0;
        if (code === 200) {
          notify({ title: 'Reservation Confirmed', text: 'Reservation has been confirmed.', type: 'success', duration: 3000 });
          await router.push({ path: '/arrivals', query: { confirmed: 'true' } });
        }
      } catch (err) {
        notify({ title: 'Confirmation failed', text: err?.response?.data?.message || err?.message || 'Unexpected error', type: 'error' });
      }
    }

    function resetFormForCreate() {
      Object.assign(state.uiForm, defaultUiForm());
      state.errors = {};
      state.hasDeposit = false;

      state.guestAttached = false;
      state.tempGuestId = 0;

      state.roomAttached = false;
      state.tempRoomId = 0;

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
      await initByMode().catch((e) => {
        notify({ title: 'Load failed', text: e?.message || 'Failed to load data', type: 'error' });
      });
    });

    onBeforeRouteUpdate(async () => {
      await initByMode().catch((e) => {
        notify({ title: 'Load failed', text: e?.message || 'Failed to load data', type: 'error' });
      });
    });

    watch(() => [props.idReservation, props.detailsType], async () => {
      await initByMode().catch((e) => {
        notify({ title: 'Load failed', text: e?.message || 'Failed to load data', type: 'error' });
      });
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
      selectedGuest,
      selectedRoom,
      sortedFilteredRooms,
      toggleDeposit,
      addService,
      removeService,
      onSubmit,
      toggleEdit,
      confirmReservation,
      touchField,
      addGuest,
      removeGuest,
      addRoom,
      removeRoom,
      canPickRoom,
      roomTotal,
      servicesTotal,
      grandTotal,
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
  gap: 12px;
  width: 100%;
}

.input-form {
  display: flex;
  flex-direction: column;
  position: relative;
  flex: 1;
}

.input-form label {
  margin-bottom: 5px;
  font-weight: bold;
  color: black;
}

.input[readonly] {
  background: #f3f3f3;
  color: #666;
}

.input-form input[type="text"],
.input-form input[type="date"],
.input-form input[type="number"],
.input-form select {
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

.form-btn.danger {
  background-color: #b35252;
}

.error-message {
  color: red;
  margin: 10px 0;
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

.service-list {
  overflow-y: auto;
  border: 1px solid #D3C1AC;
  background: #F7EFE7;
  border-radius: 10px;
  padding: 12px;
  margin: 12px 0;
  box-shadow: 0 2px 6px rgba(0,0,0,.06);
}

.service-list ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.element {
  display: grid;
  grid-template-columns: 1fr 120px;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  margin: 8px 0;
  background-color: #C8B6A6;
  border-radius: 8px;
  font-weight: 700;
  font-size: 15px;
  box-shadow: 0 2px 4px rgba(0,0,0,.1);
}

.service-title {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.service-actions {
  display: grid;
  grid-template-columns: 1fr;
  gap: 8px;
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 36px;
  padding: 0 12px;
  font-size: 0.85rem;
  font-weight: 700;
  border-radius: 10px;
  border: 1px solid #D3C1AC;
  background-color: #444444;
  color: #FFFFFF;
  cursor: pointer;
  transition: background-color 0.2s ease, opacity 0.2s ease, transform 0.05s ease;
  text-decoration: none;
  box-sizing: border-box;
}

.btn:hover  { background-color: #3a3a3a; }
.btn:active { transform: translateY(1px); }
.btn:disabled {
  background-color: #9a9a9a;
  border-color: #bdbdbd;
  color: #ececec;
  cursor: not-allowed;
  opacity: 0.85;
}

.card.guest-card {
  background: #C8B6A6;
  border-radius: 8px;
  padding: 12px 14px;
  margin: 8px 0 12px;
  box-shadow: 0 2px 6px rgba(0,0,0,.08);
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.guest-row {
  display: flex;
  gap: 12px;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
}

.guest-name {
  font-weight: 700;
}
.guest-passport {
  font-weight: 600;
}
.muted {
  opacity: .85;
  font-size: .95rem;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 6px 0;
  font-size: 15px;
}
.summary-row.total {
  font-size: 18px;
}
.summary-sep {
  border: none;
  border-top: 1px solid #ddd;
  margin: 6px 0;
}

@media (max-width: 768px) {
  .creating-form { max-width: 100%; padding: 20px 16px; }
  .date-inputs { flex-direction: column; }
  .registration-class { gap: 10px; }
  .registration-btn, .form-btn { flex: 1; min-width: 0; }
  .element { grid-template-columns: 1fr; }
}
</style>
