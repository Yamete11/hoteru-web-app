<template>
  <div class="service-form-component">
    <navbar />
    <sidebar />

    <div class="main">
      <h1>{{ state.isCreate ? 'New Service' : 'Service Details' }}</h1>

      <form @submit.prevent="onSubmit" class="creating-form">
        <div class="input-form">
          <label>Title<span v-if="state.isCreate">*</span>:</label>
          <input
              :data-testid="state.isCreate ? 'service-title' : 'input-title'"
              v-model="state.formData.title"
              class="input"
              type="text"
              placeholder="Enter service title"
              :readonly="!state.isEditing"
              @input="v$.formData.title.$touch()"
          />
          <span class="error-message" v-if="v$.formData.title.$error">
            <span v-if="!v$.formData.title.required.$response">Title is required*</span>
            <span v-else-if="!v$.formData.title.maxLength.$response">Title must be less than 20 characters*</span>
          </span>
          <span class="error-message" v-if="fieldError('Title','title')">{{ fieldError('Title','title') }}</span>
        </div>

        <div class="input-form">
          <label>Price<span v-if="state.isCreate">*</span>:</label>
          <input
              :data-testid="state.isCreate ? 'service-price' : 'input-price'"
              v-model.number="state.formData.sum"
              class="input"
              type="text"
              placeholder="Enter service price"
              :readonly="!state.isEditing"
              @input="v$.formData.sum.$touch()"
          />
          <span class="error-message" v-if="v$.formData.sum.$error">
            <span v-if="!v$.formData.sum.numeric.$response">Price must be a number*</span>
            <span v-else-if="!v$.formData.sum.required.$response">Price is required*</span>
            <span v-else-if="!v$.formData.sum.minValue.$response">Price must be at least 1*</span>
            <span v-else-if="!v$.formData.sum.maxValue.$response">Price must not exceed 1,000,000*</span>
          </span>
          <span class="error-message" v-if="fieldError('Sum','sum')">{{ fieldError('Sum','sum') }}</span>
        </div>

        <div class="input-form">
          <label>Description:</label>
          <input
              :data-testid="state.isCreate ? 'service-description' : 'input-description'"
              v-model="state.formData.description"
              class="input"
              type="text"
              placeholder="Enter service description"
              :readonly="!state.isEditing"
              @input="v$.formData.description.$touch()"
          />
          <span class="error-message" v-if="v$.formData.description.$error">Description can have only 50 symbols*</span>
          <span class="error-message" v-if="fieldError('Description','description')">{{ fieldError('Description','description') }}</span>
        </div>

        <div class="registration-class">
          <router-link class="registration-btn" to="/services" data-testid="button-back">
            {{ state.isCreate ? 'Cancel' : 'Back' }}
          </router-link>

          <button
              v-if="state.isCreate"
              class="registration-btn"
              type="submit"
              :disabled="state.isSubmitting"
              data-testid="service-confirm-button"
          >
            Confirm
          </button>

          <button
              v-else
              type="button"
              class="registration-btn"
              @click="toggleEdit"
              :disabled="state.isSubmitting"
              data-testid="button-edit-save"
          >
            {{ state.isEditing ? 'Save' : 'Edit' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { reactive, ref, onMounted, watch } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, maxLength, maxValue, minValue } from '@vuelidate/validators';
import { useRouter, useRoute } from 'vue-router';
import { notify } from '@kyvg/vue3-notification';
import { services } from '@/api';

export default {
  name: 'ServiceForm',
  props: {
    idService: { type: [Number, String], required: false, default: null },
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
        idService: 0,
        title: '',
        sum: 0,
        description: '',
      },
      errors: {},
    });

    const rules = {
      formData: {
        title: { required, maxLength: maxLength(20) },
        sum: { required, numeric, minValue: minValue(1), maxValue: maxValue(1000000) },
        description: { maxLength: maxLength(50) },
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

    const normalizeFromDto = (dto, fallbackId = 0) => ({
      idService: dto?.idService ?? dto?.id ?? fallbackId ?? 0,
      title: dto?.title ?? dto?.Title ?? '',
      sum: dto?.sum ?? dto?.Sum ?? 0,
      description: dto?.description ?? dto?.Description ?? '',
    });

    async function fetchSpecificService(id) {
      if (!id) return;
      state.isLoading = true;
      state.errors = {};
      try {
        const dto = await services.get(id);
        state.formData = normalizeFromDto(dto?.data, id);
      } catch (e) {
        notify({ title: 'Error', text: e?.message || 'Failed to fetch service details', type: 'error', duration: 3000 });
        window.history.length > 1 ? history.back() : (location.href = '/services');
      } finally {
        state.isLoading = false;
      }
    }

    async function createService() {
      state.errors = {};
      v$.value.$touch();
      if (v$.value.$error) return;

      try {
        state.isSubmitting = true;
        const payload = {
          title: state.formData.title.trim(),
          sum: Number(state.formData.sum),
          description: state.formData.description?.trim() || '',
        };
        const res = await services.create(payload);
        if (res?.httpStatusCode === 201) {
          notify({ title: 'Service Created', text: 'The service has been successfully created.', type: 'success', duration: 3000 });
          await router.push({ path: '/services', query: { created: 'true' } });
        } else {
          state.errors = res?.errors || {};
          notify({ title: 'Create failed', text: res?.message || 'Validation failed', type: 'error' });
        }
      } catch (err) {
        if (err?.details) state.errors = err.details;
        notify({ title: 'Create failed', text: err?.message || 'Unexpected error', type: 'error' });
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
          idService: state.formData.idService,
          title: state.formData.title.trim(),
          sum: Number(state.formData.sum),
          description: state.formData.description?.trim() || '',
        };
        const res = await services.update(payload);
        if (res?.httpStatusCode && res.httpStatusCode !== 200) {
          state.errors = res?.errors || {};
          notify({ title: 'Validation failed', text: res?.message || 'Fix validation errors', type: 'warn' });
          return;
        }
        notify({ title: 'Service Updated', text: 'The service has been successfully updated.', type: 'success', duration: 3000 });
        state.isEditing = false;
      } catch (e) {
        state.errors = e?.details || {};
        notify({ title: 'Update failed', text: e?.message || 'Unexpected error', type: 'error' });
      } finally {
        state.isSubmitting = false;
      }
    }

    function toggleEdit() {
      if (state.isEditing) {
        saveUpdate();
      } else {
        state.isEditing = true;
      }
    }

    function onSubmit() {
      if (state.isCreate) {
        createService();
      } else if (state.isEditing) {
        saveUpdate();
      }
    }

    const resolveAndInit = () => {
      const idFromProps = toId(props.idService);
      const idFromRoute = toId(route.params?.idService);
      const resolvedId = idFromProps || idFromRoute;
      state.isCreate = !resolvedId;
      state.isEditing = state.isCreate;
      state.formData.idService = resolvedId;
      if (!state.isCreate) fetchSpecificService(resolvedId);
    };

    onMounted(resolveAndInit);
    watch(() => [props.idService, route.params?.idService], resolveAndInit);

    return { state, v$, fieldError, toggleEdit, onSubmit };
  },
};
</script>

<style scoped>
.service-form-component {
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

.registration-btn {
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

.input[readonly] {
  background: #f3f3f3;
  color: #666;
}

.registration-class {
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

.input-form label {
  margin-bottom: 5px;
  font-weight: bold;
  color: black;
}

.input-form input[type='text'] {
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
