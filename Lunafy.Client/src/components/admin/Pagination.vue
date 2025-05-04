<script setup lang="ts">
import { computed, ref, watch } from 'vue'

type PaginationItem = {
    label: string,
    value?: string,
    isActive: boolean,
    isEllipsis: boolean
}

// const emits = defineEmits(['changePageSize', 'changePage'])
const emits = defineEmits<{
    (e: 'changePageSize', pageSize: number): void,
    (e: 'changePage', page: number): void
}>()

const props = defineProps<{
    totalPages: number,
    currentPage: number,
    pageSize: number
}>()
const maxPaginationCount = 7

const truncateLeft = ref<boolean>(false)
const truncateRight = ref<boolean>(false)

const leftDotStart = ref<number>(0)
const leftDotEnd = ref<number>(0)

const rightDotStart = ref<number>(0)
const rightDotEnd = ref<number>(0)

const evaulatePager = () => {
    if (props.totalPages <= maxPaginationCount) return

    const hidden = props.totalPages - maxPaginationCount

    const start = 1
    const end = props.totalPages

    rightDotStart.value = props.currentPage + 1 < start + 3
        ? start + 3
        : props.currentPage + 2

    rightDotEnd.value = Math.min(rightDotStart.value + hidden, end - 1)

    truncateLeft.value = rightDotEnd.value + 1 >= end
    truncateRight.value = rightDotEnd.value >= rightDotStart.value

    const rightTruncateCount = Math.max(0, rightDotEnd.value - rightDotStart.value + 1)
    const leftTruncateCount = truncateLeft.value ? hidden + (truncateRight.value ? 2 : 1) - rightTruncateCount : 0

    leftDotEnd.value = props.currentPage - 1 > end - 3
        ? end - 3
        : props.currentPage - 2
    leftDotStart.value = leftDotEnd.value - leftTruncateCount + 1
}

watch(
    () => [props.totalPages, props.currentPage],
    evaulatePager,
    { immediate: true }
)

const pageItems = computed((): PaginationItem[] => {
    const pages: PaginationItem[] = []

    if (props.totalPages <= maxPaginationCount) {
        return Array.from({ length: props.totalPages }, (_, i): PaginationItem => ({
            label: (i + 1).toString(),
            value: (i + 1).toString(),
            isActive: false,
            isEllipsis: false
        }))
    }

    if (truncateLeft.value) {
        for (let i = 1; i < leftDotStart.value; i++) {
            pages.push({
                label: i.toString(),
                value: i.toString(),
                isActive: false,
                isEllipsis: false
            })
        }

        pages.push({
            label: '...',
            isActive: false,
            isEllipsis: true
        })

        for (let i = leftDotEnd.value + 1; i < props.currentPage; i++) {
            pages.push({
                label: i.toString(),
                value: i.toString(),
                isActive: false,
                isEllipsis: false
            })
        }
    } else {
        for (let i = 1; i < props.currentPage; i++) {
            pages.push({
                label: i.toString(),
                value: i.toString(),
                isActive: false,
                isEllipsis: false
            })
        }
    }

    pages.push({
        label: props.currentPage.toString(),
        value: props.currentPage.toString(),
        isActive: true,
        isEllipsis: false
    })

    if (truncateRight.value) {
        for (let i = props.currentPage + 1; i < rightDotStart.value; i++) {
            pages.push({
                label: i.toString(),
                value: i.toString(),
                isActive: false,
                isEllipsis: false
            })
        }

        pages.push({
            label: '...',
            isActive: false,
            isEllipsis: true
        })

        for (let i = rightDotEnd.value + 1; i <= props.totalPages; i++) {
            pages.push({
                label: i.toString(),
                value: i.toString(),
                isActive: false,
                isEllipsis: false
            })
        }
    } else {
        for (let i = props.currentPage + 1; i <= props.totalPages; i++) {
            pages.push({
                label: i.toString(),
                value: i.toString(),
                isActive: false,
                isEllipsis: false
            })
        }
    }

    return pages
})

</script>

<template>
    <div class="d-flex flex-column flex-lg-row justify-content-between">
        <div>
            <select :value="pageSize" class="form-select"
                @change="(e: Event) => emits('changePageSize', parseInt((e.target as HTMLSelectElement).value))">
                <option value="5">5</option>
                <option value="15">15</option>
                <option value="25">25</option>
                <option value="50">50</option>
            </select>
        </div>
        <nav>
            <ul class="pagination">
                <li class="page-item"><a class="page-link" href="#">Previous</a></li>

                <li class="page-item" v-for="pageItem in pageItems">
                    <button class="page-link" @click="emits('changePage', parseInt(pageItem.value || '1'))"
                        v-if="!pageItem.isEllipsis" :class="{ 'active': pageItem.isActive }">{{
                            pageItem.label }}</button>
                    <button class="page-link" v-else disabled>...</button>
                </li>

                <li class="page-item"><a class="page-link" href="#">Next</a></li>
            </ul>
        </nav>
    </div>
</template>

<style lang="css" scoped></style>