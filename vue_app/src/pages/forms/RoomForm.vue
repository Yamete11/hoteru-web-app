<template>
  <div class="newRoom-component" data-testid="new-room-component">
    <navbar data-testid="navbar" />
    <sidebar data-testid="sidebar" />

    <div class="main" data-testid="main-content">
      <h1 data-testid="form-title">{{ state.isCreate ? 'New Room' : 'Room Details' }}</h1>

      <form @submit.prevent="onSubmit" class="creating-form" data-testid="room-form">
        <div class="input-form" data-testid="input-number">
          <label>Number: </label>
          <input
              :data-testid="state.isCreate ? 'number-input' : 'input-number'"
              v-model="state.formData.number"
              class="input"
              type="text"
              placeholder="Enter room number"
              :readonly="!state.isEditing"
              @input="v$.formData.number.$touch()"
          />
          <span class="error-message" v-if="v$.formData.number.$error" data-testid="error-number-validation">
            <span v-if="!v$.formData.number.required.$response">Number is required*</span>
            <span v-if="!v$.formData.number.maxLength.$response">Number must be less than 20 characters*</span>
          </span>
          <span class="error-message" v-if="fieldError('Number','number')" data-testid="error-number-server">{{ fieldError('Number','number') }}</span>
        </div>

        <div class="input-form" data-testid="input-capacity">
          <label>Capacity: </label>
          <input
              :data-testid="state.isCreate ? 'capacity-input' : 'input-capacity'"
              v-model.number="state.formData.capacity"
              class="input"
              type="number"
              placeholder="Enter room capacity"
              :readonly="!state.isEditing"
              @input="v$.formData.capacity.$touch()"
          />
          <span class="error-message" v-if="v$.formData.capacity.$error" data-testid="error-capacity-validation">
            <span v-if="!v$.formData.capacity.required.$response">Capacity is required*</span>
            <span v-else-if="!v$.formData.capacity.numeric.$response">Capacity must be a number*</span>
            <span v-else-if="!v$.formData.capacity.maxValue.$response">Capacity must be less than or equal to 10*</span>
            <span v-else-if="!v$.formData.capacity.minValue.$response">Capacity must be more than 0*</span>
          </span>
          <span class="error-message" v-if="fieldError('Capacity','capacity')" data-testid="error-capacity-server">{{ fieldError('Capacity','capacity') }}</span>
        </div>

        <div class="input-form" data-testid="input-price">
          <label>Price: </label>
          <input
              :data-testid="state.isCreate ? 'price-input' : 'input-price'"
              v-model.number="state.formData.price"
              class="input"
              type="number"
              placeholder="Enter room price"
              :readonly="!state.isEditing"
              @input="v$.formData.price.$touch()"
          />
          <span class="error-message" v-if="v$.formData.price.$error" data-testid="error-price-validation">
            <span v-if="!v$.formData.price.required.$response">Price is required*</span>
            <span v-else-if="!v$.formData.price.numeric.$response">Price must be a number*</span>
            <span v-else-if="!v$.formData.price.maxValue.$response">Price must be less than or equal to 1,000,000*</span>
            <span v-else-if="!v$.formData.price.minValue.$response">Price must be more than 0*</span>
          </span>
          <span class="error-message" v-if="fieldError('Price','price')" data-testid="error-price-server">{{ fieldError('Price','price') }}</span>
        </div>

        <div class="input-form" data-testid="input-type">
          <label>Type: </label>
          <template v-if="!state.isEditing && !state.isCreate">
            <input class="input" type="text" :value="state.typeTitle" readonly />
          </template>
          <template v-else>
            <select
                :data-testid="state.isCreate ? 'type-select' : 'room-type-select'"
                v-model="state.formData.type"
                @change="v$.formData.type.$touch()"
            >
              <option disabled value="">Select type</option>
              <option
                  v-for="roomType in state.roomTypes"
                  :key="roomType.idType"
                  :value="String(roomType.idType)"
                  :data-testid="state.isCreate ? 'type-option' : 'room-type-option'"
              >
                {{ roomType.title }}
              </option>
            </select>
          </template>
          <span class="error-message" v-if="v$.formData.type.$error" data-testid="error-type-validation">
            <span v-if="!v$.formData.type.required.$response">Type is required*</span>
          </span>
          <span class="error-message" v-if="fieldError('Type','type')" data-testid="error-type-server">{{ fieldError('Type','type') }}</span>
        </div>

        <div class="input-form" data-testid="input-status">
          <label>Status: </label>
          <template v-if="!state.isEditing && !state.isCreate">
            <input class="input" type="text" :value="state.statusTitle" readonly />
          </template>
          <template v-else>
            <select
                :data-testid="state.isCreate ? 'status-select' : 'room-status-select'"
                v-model="state.formData.status"
                @change="v$.formData.status.$touch()"
            >
              <option disabled value="">Select status</option>
              <option
                  v-for="roomStatus in state.roomStatuses"
                  :key="roomStatus.idStatus"
                  :value="String(roomStatus.idStatus)"
                  :data-testid="state.isCreate ? 'status-option' : 'room-status-option'"
              >
                {{ roomStatus.title }}
              </option>
            </select>
          </template>
          <span class="error-message" v-if="v$.formData.status.$error" data-testid="error-status-validation">
            <span v-if="!v$.formData.status.required.$response">Status is required*</span>
          </span>
          <span class="error-message" v-if="fieldError('Status','status')" data-testid="error-status-server">{{ fieldError('Status','status') }}</span>
        </div>

        <div class="registration-class" data-testid="form-actions">
          <router-link
              class="registration-btn"
              to="/rooms"
              :data-testid="state.isCreate ? 'cancel-button' : 'btn-back'"
          >
            {{ state.isCreate ? 'Cancel' : 'Back' }}
          </router-link>

          <button
              v-if="state.isCreate"
              class="registration-btn"
              type="submit"
              :disabled="state.isSubmitting"
              data-testid="submit-button"
          >
            Confirm
          </button>

          <button
              v-else
              type="submit"
              class="registration-btn"
              :disabled="state.isSubmitting"
              data-testid="btn-submit"
          >
            {{ state.isEditing ? 'Save' : 'Edit' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { reactive, onMounted, watch } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, maxLength, maxValue, minValue } from '@vuelidate/validators';
import { useRouter, useRoute } from 'vue-router';
import { rooms } from '@/api';
import { notify } from '@kyvg/vue3-notification';

export default {
  name: 'RoomForm',
  props: {
    idRoom: { type: [Number, String], required: false, default: null },
  },
  setup(props) {
    const router = useRouter();
    const route = useRoute();

    const state = reactive({
      isCreate: true,
      isEditing: true,
      isSubmitting: false,
      isLoading: false,
      formData: {
        idRoom: 0,
        number: '',
        capacity: 0,
        price: 0,
        type: '',
        status: '',
      },
      typeTitle: '',
      statusTitle: '',
      roomTypes: [],
      roomStatuses: [],
      errors: {},
    });

    const rules = {
      formData: {
        number: { required, maxLength: maxLength(20) },
        capacity: { required, numeric, maxValue: maxValue(10), minValue: minValue(1) },
        price: { required, numeric, maxValue: maxValue(1000000), minValue: minValue(1) },
        type: { required },
        status: { required },
      },
    };

    const v$ = useVuelidate(rules, state);

    const toId = (val) => {
      const n = Number(val);
      return Number.isFinite(n) && n > 0 ? n : 0;
    };

    const fieldError = (...keys) => {
      for (const k of keys) {
        const arr = state.errors?.[k];
        if (Array.isArray(arr) && arr.length) return arr[0];
      }
      return '';
    };

    const computeTitles = () => {
      const t = state.roomTypes.find((x) => String(x.idType) === String(state.formData.type));
      state.typeTitle = t?.title ?? '';
      const s = state.roomStatuses.find((x) => String(x.idStatus) === String(state.formData.status));
      state.statusTitle = s?.title ?? '';
    };

    async function fetchDictionaries() {
      try {
        const [typesRes, statusesRes] = await Promise.all([rooms.types(), rooms.statuses()]);
        state.roomTypes = Array.isArray(typesRes?.data) ? typesRes.data : (typesRes || []);
        state.roomStatuses = Array.isArray(statusesRes?.data) ? statusesRes.data : (statusesRes || []);
        computeTitles();
      } catch (error) {
        notify({ title: 'Load failed', text: error?.message || 'Failed to load dictionaries', type: 'error' });
      }
    }

    async function fetchSpecificRoom(id) {
      if (!id) return;
      state.isLoading = true;
      try {
        const roomRes = await rooms.get(id);
        const room = roomRes?.data ?? roomRes;
        state.formData = {
          idRoom: room.idRoom ?? room.id ?? id,
          number: String(room.number ?? room.Number ?? ''),
          capacity: Number(room.capacity ?? room.Capacity ?? 0),
          price: Number(room.price ?? room.Price ?? 0),
          type: String(room.type ?? room.Type ?? ''),
          status: String(room.status ?? room.Status ?? ''),
        };
        computeTitles();
      } catch (error) {
        notify({ title: 'Load failed', text: error?.message || 'Error loading room', type: 'error' });
        window.history.length > 1 ? history.back() : (location.href = '/rooms');
      } finally {
        state.isLoading = false;
      }
    }

    async function createRoom() {
      state.errors = {};
      v$.value.$touch();
      if (v$.value.$error) return;

      try {
        state.isSubmitting = true;
        const payload = {
          number: String(state.formData.number ?? '').trim(),
          capacity: Number(state.formData.capacity),
          price: Number(state.formData.price),
          type: String(state.formData.type),
          status: String(state.formData.status),
        };

        if (!Number.isFinite(payload.capacity) || !Number.isFinite(payload.price)) {
          notify({ title: 'Validation', text: 'Capacity and Price must be valid numbers', type: 'error' });
          return;
        }

        const res = await rooms.create(payload);

        if (res?.httpStatusCode === 201 || res?.httpStatusCode === 200) {
          notify({ title: 'Room Created', text: 'Room has been created successfully.', type: 'success', duration: 3000 });
          await router.push({ path: '/rooms', query: { created: 'true' } });
          return;
        }

        const errors = res?.errors || res?.data?.errors;
        if (errors) state.errors = errors;
        notify({ title: 'Create failed', text: res?.message || res?.data?.message || 'Validation failed', type: 'error' });
      } catch (error) {
        const backendErrors = error?.response?.data?.errors || error?.details;
        if (backendErrors) state.errors = backendErrors;
        notify({ title: 'Create failed', text: error?.response?.data?.message || error?.message || 'Unexpected error', type: 'error' });
      } finally {
        state.isSubmitting = false;
      }
    }

    async function saveUpdate() {
      v$.value.$touch();
      if (v$.value.$error) return;

      try {
        state.isSubmitting = true;
        state.errors = {};
        const payload = {
          idRoom: state.formData.idRoom,
          number: String(state.formData.number ?? '').trim(),
          capacity: Number(state.formData.capacity),
          price: Number(state.formData.price),
          type: String(state.formData.type),
          status: String(state.formData.status),
        };

        const res = await rooms.update(payload);
        if (res?.httpStatusCode && res.httpStatusCode !== 200) {
          state.errors = res?.errors || {};
          notify({ title: 'Update failed', text: res?.message || 'Validation failed', type: 'error' });
          return;
        }

        notify({ title: 'Room Updated', text: 'Room has been successfully updated.', type: 'success', duration: 4000 });
        state.isEditing = false;
      } catch (err) {
        if (err?.details) state.errors = err.details;
        notify({ title: 'Update failed', text: err?.message || 'Unexpected error', type: 'error' });
      } finally {
        state.isSubmitting = false;
      }
    }

    function onSubmit() {
      if (state.isCreate) {
        createRoom();
      } else if (!state.isEditing) {
        state.isEditing = true;
      } else {
        saveUpdate();
      }
    }

    const init = () => {
      const idFromProps = toId(props.idRoom);
      const idFromRoute = toId(route.params?.idRoom);
      const resolvedId = idFromProps || idFromRoute;
      state.isCreate = !resolvedId;
      state.isEditing = state.isCreate;
      state.formData.idRoom = resolvedId;

      fetchDictionaries().then(() => {
        if (!state.isCreate) fetchSpecificRoom(resolvedId);
      });
    };

    onMounted(init);
    watch(() => [props.idRoom, route.params?.idRoom], init);

    watch(() => [state.formData.type, state.formData.status, state.roomTypes, state.roomStatuses], computeTitles, { deep: true });

    return { state, v$, onSubmit, fieldError };
  },
};
</script>

<style scoped>
.newRoom-component {
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
  margin: 5%;
}
.creating-form {
  width: 20%;
  max-width: 720px;
  background: #fff;
  border-radius: 8px;
  padding: 24px 20px;
  box-shadow: 0 6px 16px rgba(0,0,0,.08);
  display: flex;
  flex-direction: column;
  gap: 14px;
}
.registration-btn{
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-weight: bold;
  color: white;
  margin: 10px;
  cursor: pointer;
}
.registration-class{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}
.input-form {
  display: flex;
  flex-direction: column;
  margin: 5px;
}

.input[readonly] {
  background: #f3f3f3;
  color: #666;
}
.input-form label {
  margin-bottom: 5px;
  font-weight: bold;
  color: black;
}
.input-form input[type="text"],
.input-form input[type="number"] {
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-bottom: 10px;
}

h1 {
  display: flex;
  justify-content: center;
  color: black;
}

.input-form select {
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-bottom: 10px;
  background-color: white;
  color: black;
}
.error-message {
  color: red;
  margin: 10px 0;
}
</style>