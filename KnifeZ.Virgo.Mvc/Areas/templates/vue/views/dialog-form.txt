<template>
	<v-form-dialog :use-dialog="true">
		<v-form layout="inline" :fields="fields" :events="events" @reSearch="reSearch"></v-form>
	</v-form-dialog>
</template>

<script>
import VFormDialog from '@/components/page/v-form-dialog'
import VForm from '@/components/page/v-form'
import API from '../api/index'
export default {
	name: '$name$-dialog',
	components: {
		VForm,
		VFormDialog,
	},
	data() {
		return {
			events: API,
		}
	},
	props: {
		fields: {
			type: Array,
		},
	},
	methods: {
		reSearch() {
			this.$emit('reSearch')
		},
	},
}
</script>
