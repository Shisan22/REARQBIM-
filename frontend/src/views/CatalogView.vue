<script setup>
import { ref, computed } from 'vue'
import MaterialCard from '../components/MaterialCard.vue'
import { materialsList, currentCompany, addTransaction } from '../stores/materialsStore.js'

const props = defineProps({
  onNotify: {
    type: Function,
    default: () => {}
  }
})

const emit = defineEmits(['navigate'])

const searchQuery = ref('')
const selectedCategory = ref('')
const selectedCity = ref('')
const onlyBim = ref(false)

const filteredMaterials = computed(() => {
  return materialsList.value.filter(item => {
    const matchesSearch = !searchQuery.value || 
      item.title.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      item.sku.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      (item.technicalDescription && item.technicalDescription.toLowerCase().includes(searchQuery.value.toLowerCase()))
    
    const matchesCategory = !selectedCategory.value || item.category === selectedCategory.value
    const matchesCity = !selectedCity.value || item.city.toLowerCase().includes(selectedCity.value.toLowerCase())
    const matchesBim = !onlyBim.value || item.hasBimAsset

    return matchesSearch && matchesCategory && matchesCity && matchesBim
  })
})

const handleBimDownload = (payload) => {
  if (!currentCompany.value.hasActiveSubscription) {
    props.onNotify('Acceso Restringido', 'Requiere una suscripción activa (Plan Pro u Enterprise) para descargar familias BIM.', 'error')
    return
  }
  props.onNotify(
    'Descarga BIM Completada',
    `Archivo ${payload.bimFormat} (${payload.sku}) descargado. Incorporado exitosamente a su biblioteca Revit / IFC.`,
    'success'
  )
}

const handleQuoteRequest = (material) => {
  // Simular creación de transacción transaccional
  const qty = 5
  const subtotal = material.unitPriceCop * qty
  const comm = subtotal * 0.07
  const total = subtotal + comm
  const tons = ((material.weightKgPerUnit * qty) / 1000).toFixed(3)
  const co2 = (material.embodiedCarbonKgCo2ePerUnit * qty).toFixed(1)

  const newTx = {
    id: `tx-${Date.now()}`,
    code: `TX-2026-${Math.floor(100000 + Math.random() * 900000)}`,
    materialTitle: material.title,
    originProject: material.originProjectName,
    buyerProject: currentCompany.value.activeProjects[0].name,
    quantity: qty,
    unitPriceCop: material.unitPriceCop,
    subtotalMaterialsCop: subtotal,
    commissionPct: 7.0,
    platformFeeCop: comm,
    totalCop: total,
    status: 'ESCROW_FUNDED',
    date: new Date().toISOString().split('T')[0],
    rcdMassDivertedTons: parseFloat(tons),
    co2eAvoidedKg: parseFloat(co2),
    certificateCode: `CERT-RCD-2026-${Math.floor(1000 + Math.random() * 9000)}`
  }

  addTransaction(newTx)

  props.onNotify(
    'Intercambio Coordinado',
    `Se reservaron ${qty} unidades de "${material.title}". Comisión de plataforma: 7%. Fondos asegurados en Escrow.`,
    'info'
  )
}
</script>

<template>
  <div class="view-catalog">
    <!-- Panel de Búsqueda y Filtros Técnicos Paramétricos -->
    <div class="search-panel">
      <div class="search-input-wrapper">
        <svg class="search-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
          <circle cx="11" cy="11" r="8" stroke-width="2"/>
          <path d="m21 21-4.3-4.3" stroke-width="2"/>
        </svg>
        <input 
          v-model="searchQuery" 
          type="text" 
          placeholder="Buscar por título, SKU, material o especificación arquitectónica..."
          class="input-search"
        />
      </div>

      <div class="filter-controls">
        <select v-model="selectedCategory" class="select-filter">
          <option value="">Todas las Categorías</option>
          <option value="CARPINTERIA_Y_MADERAS">Carpintería y Maderas</option>
          <option value="MAMPOSTERIA_Y_BLOQUES">Mampostería y Bloques</option>
          <option value="ESTRUCTURAS_Y_CONCRETO">Estructuras y Acero</option>
        </select>

        <select v-model="selectedCity" class="select-filter">
          <option value="">Todas las Ciudades</option>
          <option value="Bogotá">Bogotá D.C.</option>
          <option value="Medellín">Medellín</option>
          <option value="Cali">Cali</option>
        </select>

        <label class="checkbox-filter">
          <input v-model="onlyBim" type="checkbox" />
          <span>Solo con Objeto BIM (.RFA / .IFC)</span>
        </label>

        <button class="btn-publish-shortcut" @click="emit('navigate', 'publish')">
          + Publicar Sobrante de Obra
        </button>
      </div>
    </div>

    <!-- Resultados -->
    <div class="results-bar">
      <div>
        <h2>Sobrantes Técnicos Disponibles en Obra ({{ filteredMaterials.length }})</h2>
        <p class="results-subtitle">Materiales recuperados y remanentes con tolerancia milimétrica verificada</p>
      </div>
      <div class="filter-badge-summary">
        <span class="active-tag">Disponibilidad Inmediata</span>
        <span class="active-tag">Cumple Res. 1257</span>
      </div>
    </div>

    <!-- Grid de Fichas -->
    <div class="catalog-grid">
      <MaterialCard
        v-for="material in filteredMaterials"
        :key="material.id"
        :material="material"
        :has-active-subscription="currentCompany.hasActiveSubscription"
        @download-bim="handleBimDownload"
        @request-quote="handleQuoteRequest"
      />
    </div>
  </div>
</template>

<style scoped>
.view-catalog {
  display: flex;
  flex-direction: column;
}

.search-panel {
  background-color: #ffffff;
  padding: 18px 22px;
  border-radius: 10px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 2px 4px rgba(15, 23, 42, 0.04);
  margin-bottom: 24px;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.search-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 14px;
  width: 18px;
  height: 18px;
  color: #94a3b8;
}

.input-search {
  width: 100%;
  padding: 12px 14px 12px 42px;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 0.92rem;
  color: #0f172a;
  outline: none;
  transition: border-color 0.2s;
}

.input-search:focus {
  border-color: #0284c7;
}

.filter-controls {
  display: flex;
  gap: 14px;
  flex-wrap: wrap;
  align-items: center;
}

.select-filter {
  padding: 8px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 0.85rem;
  color: #334155;
  background-color: #ffffff;
}

.checkbox-filter {
  display: flex;
  align-items: center;
  gap: 7px;
  font-size: 0.85rem;
  font-weight: 600;
  color: #0284c7;
  cursor: pointer;
}

.btn-publish-shortcut {
  margin-left: auto;
  background-color: #0d9488;
  color: #ffffff;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 0.82rem;
  font-weight: 700;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-publish-shortcut:hover {
  background-color: #0f766e;
}

.results-bar {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 20px;
}

.results-bar h2 {
  font-size: 1.25rem;
  font-weight: 800;
  color: #0f172a;
}

.results-subtitle {
  font-size: 0.82rem;
  color: #64748b;
  margin-top: 2px;
}

.filter-badge-summary {
  display: flex;
  gap: 8px;
}

.active-tag {
  background-color: #ecfdf5;
  color: #047857;
  border: 1px solid #a7f3d0;
  font-size: 0.72rem;
  font-weight: 700;
  padding: 4px 10px;
  border-radius: 20px;
}

.catalog-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
  gap: 24px;
}
</style>
