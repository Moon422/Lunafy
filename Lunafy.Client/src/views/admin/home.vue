<script setup lang="ts">
import TotalUsersStat from '@/components/admin/home/TotalUsersStat.vue'
import TotalTracksStat from '@/components/admin/home/TotalTracksStat.vue'
import MonthlyRevenueStat from '@/components/admin/home/MonthlyRevenueStat.vue'
import StorageUsedStat from '@/components/admin/home/StorageUsedStat.vue'
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend,
    type ChartData,
    type ChartOptions
} from 'chart.js'
import { Line } from 'vue-chartjs'

const CHART_COLORS = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
}

const options: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    interaction: {
        intersect: false
    },
    scales: {
        x: {
            display: true,
            title: {
                display: true,
                text: 'Months'
            }
        },
        y: {
            display: true,
            title: {
                display: true,
                text: 'Value'
            },
            suggestedMin: -10,
            suggestedMax: 200
        }
    }
}

const DATA_COUNT = 12
const labels = []
for (let i = 0; i < DATA_COUNT; ++i) {
    labels.push(i.toString())
}
const datapoints = [0, 20, 20, 60, 60, 120, NaN, 180, 120, 125, 105, 110, 170]

const data: ChartData<'line'> = {
    labels,
    datasets: [
        {
            label: 'Cubic interpolation',
            data: datapoints,
            borderColor: CHART_COLORS.blue,
            fill: false,
            tension: 0.4
        }
    ]
}

ChartJS.register(
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend
)
</script>

<template>
    <!-- Header -->
    <div class="container">
        <div class="d-flex justify-content-between align-items-center">
            <h3>Dashboard</h3>
            <button class="btn btn-primary">
                <i class="bi bi-plus"></i>
                New Report
            </button>
        </div>
    </div>

    <!-- Stats -->
    <div class="container mt-3">
        <div class="row g-3">
            <div class="col-12 col-lg-6">
                <TotalUsersStat />
            </div>
            <div class="col-12 col-lg-6">
                <TotalTracksStat />
            </div>
            <div class="col-12 col-lg-6">
                <MonthlyRevenueStat />
            </div>
            <div class="col-12 col-lg-6">
                <StorageUsedStat />
            </div>
        </div>
    </div>

    <!-- Charts -->
    <div class="container mt-3">
        <div class="row g-3">
            <div class="col-12 col-lg-6">
                <div class="card h-100">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center">
                            <h5>Recent Activity</h5>
                            <div>
                                <select class="form-select" aria-label="Recent Activity">
                                    <option value="7">Last 7 Days</option>
                                    <option value="30">Last 30 Days</option>
                                    <option value="180">Last 3 Months</option>
                                </select>
                            </div>
                        </div>
                        <div style="height: 25rem;">
                            <Line :data="data" :options="options" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-lg-6">
                <div class="card h-100">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center">
                            <h5>Listening Stats</h5>
                            <div>
                                <select class="form-select" aria-label="Recent Activity">
                                    <option value="genre">By Genre</option>
                                    <option value="artist">By Artist</option>
                                    <option value="album">By Album</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.grid-container {
    display: grid;
    grid-template-columns: 1fr;
    gap: 1rem;
}

/* Large devices (laptops, 992px and up) */
@media (min-width: 768px) {
    .grid-container {
        grid-template-columns: repeat(2, 1fr);
    }
}
</style>