<template>
  <div class="mx-auto mb-10 max-w-4.5xl space-y-8 h-full flex flex-col">
    <div class="">
      <h1 class="text-xl font-medium">Account</h1>
      <div class="flex space-x-6">
        <div v-if="!editMode" class="mt-2 flex space-x-5">
          <div class="rounded-2xl border border-neutral-300 p-5 pr-7">
            <div class="flex space-x-4 ">
              <img :src="userDefaultImg" class="h-16 w-16 rounded-full border border-gray-300"/>
              <div>
                <h1 class="whitespace-nowrap text-lg font-medium leading-5">
                  {{ user.firstName }} {{ user.lastName }}
                </h1>
                <h2 class="text-sm text-neutral-500">{{ user.email }}</h2>
                <h3 class="text-sm text-neutral-500">
                  {{ user.residence }}
                </h3>
              </div>
            </div>
            <div class="mt-3.5 flex space-x-2">
              <Button
                @click="beginUpdate()"
                class="flex space-x-1.5 border border-neutral-300 !px-4 !py-1.5 transition hover:bg-neutral-50"
              >
                <span>Update</span>
                <EditIcon class="h-[18px] w-[18px]" />
              </Button>
              <Button
                class="border border-neutral-300 !py-1.5 px-5 hover:bg-neutral-50 transition"
                type="button"
                @click="passwordModalOpen = true"
              >
                Change password
              </Button>
            </div>
          </div>
        </div>
        <div v-else>
          <p class="text-sm font-medium mb-0.5 mt-1">Update your personal account information</p>
          <p class="text-sm text-neutral-500">Please note that this information is publicly visible</p>
          <form @submit.prevent="updateUser()" class="ml-[22rem] -mt-11 flex flex-col space-y-3">
            <div class="flex space-x-2 w-fit">
              <Input
                required
                v-model="user.firstName"
                placeholder="First Name"
                class="h-12"
                name="firstName"
                label="First Name"
              />
              <Input
                required
                v-model="user.lastName"
                placeholder="Last Name"
                class="h-12"
                name="lastName"
                label="Last Name"
              />
            </div>
            <Input
              disabled
              required
              v-model="user.email"
              placeholder="Email address"
              class="h-12 w-[24.5rem]"
              name="emailAddress"
              type="tel"
              label="Email address"
              title="Your email address is permanent &#10;and cannot be changed"
            />
            <Input
              required
              v-model="user.residence"
              class="h-12 w-[24.5rem]"
              name="street"
              placeholder="Location of Residence"
              label="Residence"
            />
            <div class="flex space-x-2 pt-2">
              <Button @click="cancelUpdate()" type="button" class="px-10 border border-neutral-300 !py-2 hover:bg-neutral-50">
                Cancel
              </Button>
              <Button
                :disabled="loading"
                type="submit"
                class="group flex items-center space-x-2 !rounded-full bg-black !px-6 !py-2 text-white"
              >
                <div v-if="loading" class="flex items-center">
                  <Loader class="mr-3" />
                  Saving...
                </div>
                <div v-else>Save Changes</div>
              </Button>
            </div>
          </form>
        </div>
        <div v-if="!editMode" class="mt-4 pt-0.5">
          <h3 class="font-medium">Delete account</h3>
          <p class="text-sm text-neutral-500 w-[23rem]">
            Once you delete your account there is no going back. Please be certain.
            You will be asked to confirm your decision once more after clicking the button.
          </p>
          <Button
            @click="deletionModalOpen = true"
            class="mt-2.5 border hover:border-red-600 border-red-600 text-red-600 !px-5 !py-1.5 hover:text-white transition hover:bg-red-600"
          >
            Delete account
          </Button>
        </div>
      </div>
      <Modal
        light
        :isOpen="deletionModalOpen"
        @modalClosed="deletionModalOpen = false"
        class="-mt-10 max-w-sm bg-white text-sm p-7"
      >
        <form
          @submit.prevent="confirmDeleteAccount()"
          class=""
        >
          <div class="w-full space-y-5 pb-3">
            <p class="text-base text-left">
              <span class="font-medium">Please tell us why you want to delete your account<br /></span>
              <span class="text-sm text-neutral-500 leading-loose">
                E.g. "I don't find tourister.com profitable enough"
              </span>
            </p>
            <textarea
              required
              rows="5"
              placeholder="Enter a message here.."
              v-model="deletionReason"
              class="w-full resize-none rounded-lg border border-gray-300 text-sm focus:border-gray-300 focus:ring-0"
            >
            </textarea>
            <div v-if="deletionError" class="text-[15px] text-red-500">{{ deletionError }}</div>
          </div>
          <button
            type="submit"
            class="py-2 w-full font-medium rounded-full border hover:border-red-700 border-red-600 text-red-600 transition hover:text-white hover:bg-red-700"
          >
            Delete my account
          </button>
        </form>
      </Modal>
    </div>
    <div class="flex-1">
      <div class="flex justify-between items-center">
        <div>
          <div class="flex items-center space-x-1">
            <h1 class="text-lg font-medium">Notifications</h1>
            <div
                v-if="unreadNotifications"
                class="rounded-full leading-none h-4 w-4 bg-red-500 flex items-center justify-center text-xs font-bold text-white"
            >
              {{unreadNotifications}}
            </div>
          </div>
          <h2 class="text-neutral-500 text-sm">
            You can change your notifcation subscription settings on the right
          </h2>
        </div>
          <Button
            class="border border-neutral-300 -mb-2 !py-1.5 px-5 hover:bg-neutral-50 transition"
            type="button"
            @click="subscriptionsOpen = true"
          >
            Subscription settings
          </Button>
      </div>
      <div 
        v-if="notifications.length" 
        class="mt-5 divide-y divide-neutral-300 border border-neutral-300 rounded-xl"
      >
        <div 
          v-for="notification of notifications" 
          :key="notification.id" 
          class="flex items-start justify-between px-6 py-5"
        >
          <div>
            <h2 class="text-sm font-medium">{{notification.title}}</h2>
            <p class="text-sm text-neutral-500 line-clamp-1 pt-0.5 pr-14" :title="notification.message">
              {{notification.message}}
            </p>
            <p 
              class="pt-2 text-[13px] text-neutral-500"
              :title="format(parseJSON(notification.timestamp), 'EEE, dd MMM yyyy')"
            >
              {{formatDistanceToNow(parseJSON(notification.timestamp))}} ago
            </p>
          </div>
          <button 
            @click="deleteNotification(notification.id)" 
            class="group hover:bg-black ring-neutral-400 hover:ring-black rounded-full p-0.5 transition"
          >
            <XIcon class="h-3.5 w-3.5 group-hover:text-white stroke-[3] transition" />
          </button>
        </div>
      </div>
      <div v-else class="mt-5 flex justify-center py-32">
        <p class="text-sm text-neutral-500">You have no notifications</p>
      </div>
    </div>
    <ChangePasswordModal
      :is-open="passwordModalOpen"
      @closed="passwordModalOpen = false"
    />
    <SubscriptionDialog :is-open="subscriptionsOpen" @closed="subscriptionsOpen = false" />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import {
  EditIcon,
  XIcon
} from 'vue-tabler-icons'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Loader from '@/components/ui/Loader.vue'
import Modal from '@/components/ui/Modal.vue'
import ChangePasswordModal from '@/components/registration/ChangePasswordModal.vue'
import api from '@/api/api'
import { setUser, notifications, unreadNotifications } from '@/stores/userStore'
import userDefaultImg from '@/assets/images/user-default.png'
import {format, formatDistanceToNow, parseJSON} from "date-fns";
import SubscriptionDialog from "@/components/user/SubscriptionDialog.vue";

const editMode = ref(false)
const loading = ref(false)
const deletionModalOpen = ref(false)
const passwordModalOpen = ref(false)
const subscriptionsOpen = ref(false)

const user = ref()
const deletionError = ref('')
const deletionReason = ref('')

const beginUpdate = () => {
  user.value.copy = JSON.parse(JSON.stringify(user.value))
  editMode.value = true
}

const cancelUpdate = () => {
  user.value = user.value.copy
  editMode.value = false
}

const updateUser = async () => {
  loading.value = true
  const [, error] = await api.users.update(user.value)
  if (!error) {
    loading.value = editMode.value = false
    delete user.value.copy
    setUser(user.value)
  }
}

const confirmDeleteAccount = async () => {
  deletionError.value = ''
  const [, error] = await api.users.deleteProfile()
  if (error) {
    deletionError.value = error.detail
  }
  else {
    deletionModalOpen.value = false
    await api.auth.signOut()
  }
}

const deleteNotification = async (notificationId) => {
  const [, error] = await api.reservations.deleteNotification(notificationId)
  if (!error) {
    notifications.value = notifications.value.filter(notification => notification.id !== notificationId)
    unreadNotifications.value = notifications.value.length
  }
}

const [profileData, profileError] = await api.users.getProfile()
if (!profileError) {
  user.value = profileData
}

</script>
