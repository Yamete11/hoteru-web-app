<template>
  <div class="item-div">
    <span class="item name" data-testid="guest-name">{{ guest.name }}</span>
    <span class="item surname" data-testid="guest-surname">{{ guest.surname }}</span>
    <span class="item telNumber" data-testid="guest-telNumber">{{ guest.telNumber }}</span>
    <span class="item email" data-testid="guest-email">{{ guest.email }}</span>
    <div class="item-btns">
      <button class="btn" type="button" @click="viewGuestDetails(guest.idPerson)">
        Details
      </button>
      <button class="btn" type="button" data-testid="delete-guest-button" @click="deleteGuest(guest.idPerson)">
        Delete
      </button>
    </div>
  </div>
</template>

<script>
import { guests } from "@/api";

export default {
  name: "GuestItem",
  props: {
    guest: { type: Object, required: true },
  },
  emits: ["deleteGuest", "notificationDeleteAttempt"],
  data() {
    return { busy: false };
  },
  methods: {
    viewGuestDetails(idPerson) {
      this.$router.push({ name: "GuestDetails", params: { idPerson } });
    },
    async deleteGuest(idPerson) {
      if (this.busy) return;
      this.busy = true;
      try {
        await guests.remove(idPerson);
        this.$emit("deleteGuest", idPerson);
        this.$emit("notificationDeleteAttempt");
      } catch (err) {
        console.error("Delete guest failed:", err);
      } finally {
        this.busy = false;
      }
    },
  },
};
</script>

<style scoped>
.item-div{
  display: grid;
  grid-template-columns: 1.2fr 1.2fr 160px 1.8fr 240px;
  align-items: center;
  gap: 10px;

  padding: 10px 14px;
  margin: 8px 16px;
  background-color:#C8B6A6;
  border-radius: 8px;
  font-weight: 600;
  font-size: 16px;
}

.item {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  min-width: 0;
}
.item.telNumber {
  justify-content: center;
}
.item.email {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.item-btns {
  justify-self: end;
  display: grid;
  grid-template-columns: 120px 120px;
  gap: 10px;
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
.btn:hover  {
  background-color: #3a3a3a;
}
.btn:active {
  transform: translateY(1px)
; }
.btn:disabled {
  background-color: #9a9a9a;
  border-color: #bdbdbd;
  color: #ececec;
  cursor: not-allowed;
  opacity: 0.85;
}
</style>
