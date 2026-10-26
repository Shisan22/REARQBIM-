<script setup>
import { computed } from 'vue'

const props = defineProps({
  material: {
    type: Object,
    required: true,
    default: () => ({
      id: '',
      sku: 'MAT-2026-089',
      title: 'Puerta Entamborada en Roble Reclaimed + Marco Estructural',
      category: 'CARPINTERIA_Y_MADERAS',
      condition: 'EXCELENTE',
      lengthMm: 2100,
      widthMm: 900,
      heightMm: 45,
      unitOfMeasure: 'UNIDAD',
      weightKgPerUnit: 28.5,
      availableQuantity: 42,
      unitPriceCop: 220000,
      originalRetailPriceCop: 450000,
      savingsPercentage: 51.11,
      embodiedCarbonKgCo2ePerUnit: 14.2,
      originProjectName: 'Torre Sostenible Calle 93',
      city: 'Bogotá D.C.',
      imageUrl: 'https://images.unsplash.com/photo-1513694203232-719a280e022f?auto=format&fit=crop&w=600&q=80',
      hasBimAsset: true,
      bimAsset: {
        fileFormat: 'RFA',
        lodLevel: 'LOD 300',
        fileSizeBytes: 12582912, // 12 MB
        downloadCount: 38
      }
    })
  },
  hasActiveSubscription: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['download-bim', 'request-quote', 'view-details'])

// Formateadores de moneda y peso
const formattedPrice = computed(() => {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0
  }).format(props.material.unitPriceCop)
})

const formattedOriginalPrice = computed(() => {
  if (!props.material.originalRetailPriceCop) return null
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0
  }).format(props.material.originalRetailPriceCop)
})

const formattedDimensions = computed(() => {
  const { lengthMm, widthMm, heightMm } = props.material
  if (lengthMm && widthMm && heightMm) {
    return `${lengthMm} × ${widthMm} × ${heightMm} mm`
  }
  return 'Dimensiones en ficha técnica'
})

// Cálculo de impacto ambiental acumulado según Res. 1257 / PGI-RCD
const estimatedDivertedTons = computed(() => {
  const totalKg = props.material.weightKgPerUnit * props.material.availableQuantity
  return (totalKg / 1000).toFixed(2)
})

const conditionBadgeClass = computed(() => {
  switch (props.material.condition) {
    case 'NUEVO_EXCEDENTE':
      return 'badge-condition--nuevo'
    case 'EXCELENTE':
      return 'badge-condition--excelente'
    case 'BUENO_REUTILIZABLE':
      return 'badge-condition--bueno'
    default:
      return 'badge-condition--reciclaje'
  }
})

const conditionLabel = computed(() => {
  const map = {
    NUEVO_EXCEDENTE: 'Nuevo / Remanente',
    EXCELENTE: 'Excelente Estado',
    BUENO_REUTILIZABLE: 'Bueno Reutilizable',
    APTO_RECICLAJE: 'Apto Transformación'
  }
  return map[props.material.condition] || props.material.condition
})

// Acciones
const handleBimDownload = () => {
  emit('download-bim', {
    materialId: props.material.id,
    sku: props.material.sku,
    bimFormat: props.material.bimAsset?.fileFormat
  })
}

const handleQuoteRequest = () => {
  emit('request-quote', props.material)
}

const handleCardClick = () => {
  emit('view-details', props.material.id)
}
</script>

<template>
  <article class="rearq-card" @click="handleCardClick">
    <!-- Encabezado Visual y Badges Técnicos -->
    <div class="card-media">
      <img 
        :src="material.imageUrl || '/assets/placeholder-material.webp'" 
        :alt="material.title" 
        class="card-image"
        loading="lazy"
      />
      
      <!-- Overlay de Sostenibilidad / Ahorro -->
      <div class="card-media-badges">
        <span class="badge-condition" :class="conditionBadgeClass">
          {{ conditionLabel }}
        </span>
        <span v-if="material.savingsPercentage > 0" class="badge-savings">
          -{{ Math.round(material.savingsPercentage) }}% Ahorro
        </span>
      </div>

      <!-- Badge Flotante BIM si tiene archivo 3D/Revit -->
      <div v-if="material.hasBimAsset" class="badge-bim-indicator">
        <svg class="icon-cube" viewBox="0 0 24 24" fill="none" stroke="currentColor">
          <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          <polyline points="3.27 6.96 12 12.01 20.73 6.96" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          <line x1="12" y1="22.08" x2="12" y2="12" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        <span>BIM {{ material.bimAsset?.fileFormat }} · {{ material.bimAsset?.lodLevel }}</span>
      </div>
    </div>

    <!-- Contenido y Especificaciones Técnicas -->
    <div class="card-body">
      <!-- Metadatos de Obra y Ubicación -->
      <div class="card-meta-header">
        <span class="sku-code">{{ material.sku }}</span>
        <span class="location-tag">
          <svg class="icon-pin" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z" stroke-width="2"/>
            <circle cx="12" cy="10" r="3" stroke-width="2"/>
          </svg>
          {{ material.city }}
        </span>
      </div>

      <h3 class="material-title" :title="material.title">
        {{ material.title }}
      </h3>
      <p class="origin-project">Obra de Origen: <strong>{{ material.originProjectName }}</strong></p>

      <!-- Ficha de Dimensiones Técnicas Exactas (Blueprint Specs) -->
      <div class="technical-specs-panel">
        <div class="spec-row">
          <span class="spec-label">
            <svg class="icon-ruler" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <path d="M3 6l18 0M3 12l18 0M3 18l18 0" stroke-width="2" stroke-linecap="round"/>
            </svg>
            Dimensiones Exactas:
          </span>
          <span class="spec-value-dim">{{ formattedDimensions }}</span>
        </div>

        <div class="specs-grid-mini">
          <div class="mini-spec">
            <span class="mini-label">Disponible:</span>
            <span class="mini-value">{{ material.availableQuantity }} {{ material.unitOfMeasure }}s</span>
          </div>
          <div class="mini-spec">
            <span class="mini-label">Masa RCD Evitada:</span>
            <span class="mini-value highlight-green">{{ estimatedDivertedTons }} Ton</span>
          </div>
        </div>
      </div>

      <!-- Métricas Financieras B2B -->
      <div class="pricing-container">
        <div class="price-main">
          <span class="currency-tag">COP</span>
          <span class="price-value">{{ formattedPrice }}</span>
          <span class="price-unit">/ {{ material.unitOfMeasure }}</span>
        </div>
        <div v-if="formattedOriginalPrice" class="price-reference">
          Ref. Mercado: <del>{{ formattedOriginalPrice }}</del>
        </div>
      </div>

      <!-- Trazabilidad Res. 1257 / PGI-RCD -->
      <div class="environmental-footprint">
        <div class="footprint-pill" title="Cumple parámetros de economía circular para el Plan de Gestión Integral de RCD">
          <svg class="icon-leaf" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M11 20A7 7 0 0 1 9.8 6.1C15.5 5 17 4.48 19 2c1 2 2 4.18 2 8 0 5.5-4.78 10-10 10Z" stroke-width="2"/>
            <path d="M2 21c0-3 1.85-5.36 5.08-6C9.5 14.52 12 13 13 12" stroke-width="2"/>
          </svg>
          <span>PGI-RCD Certificable (-{{ material.embodiedCarbonKgCo2ePerUnit }} kg CO₂e)</span>
        </div>
      </div>
    </div>

    <!-- Botones de Acción B2B -->
    <div class="card-actions" @click.stop>
      <!-- Botón Primario Técnico: Descarga de Objeto BIM -->
      <button 
        v-if="material.hasBimAsset"
        type="button" 
        class="btn-bim-download"
        :class="{ 'btn-bim-locked': !hasActiveSubscription }"
        @click="handleBimDownload"
      >
        <svg class="icon-bim-btn" viewBox="0 0 24 24" fill="none" stroke="currentColor">
          <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          <polyline points="7 10 12 15 17 10" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          <line x1="12" y1="15" x2="12" y2="3" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        <span>
          Descargar Objeto BIM
          <small class="bim-badge-sub">.{{ material.bimAsset?.fileFormat }}</small>
        </span>
      </button>

      <!-- Botón de Transacción Comercial -->
      <button 
        type="button" 
        class="btn-request-transaction"
        @click="handleQuoteRequest"
      >
        Solicitar Material
      </button>
    </div>
  </article>
</template>

<style scoped>
/* ==========================================================================
   REARQ BIM - Design System Tokens & Component Styles
   Paleta: Slate (#0f172a / #1e293b), Emerald Eco (#059669 / #10b981),
   BIM Cyan (#0284c7 / #0ea5e9).
   ========================================================================== */

.rearq-card {
  display: flex;
  flex-direction: column;
  background-color: #ffffff;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 4px 6px -1px rgba(15, 23, 42, 0.05), 0 2px 4px -2px rgba(15, 23, 42, 0.05);
  overflow: hidden;
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
  color: #1e293b;
  position: relative;
  cursor: pointer;
}

.rearq-card:hover {
  transform: translateY(-4px);
  border-color: #94a3b8;
  box-shadow: 0 16px 24px -4px rgba(15, 23, 42, 0.12), 0 6px 12px -4px rgba(15, 23, 42, 0.06);
}

/* Área Multimedia */
.card-media {
  position: relative;
  width: 100%;
  height: 210px;
  background-color: #f1f5f9;
  overflow: hidden;
}

.card-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.rearq-card:hover .card-image {
  transform: scale(1.04);
}

.card-media-badges {
  position: absolute;
  top: 12px;
  left: 12px;
  display: flex;
  gap: 8px;
  z-index: 2;
}

.badge-condition {
  font-size: 0.72rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  padding: 4px 10px;
  border-radius: 6px;
  backdrop-filter: blur(8px);
}

.badge-condition--nuevo {
  background-color: rgba(16, 185, 129, 0.95);
  color: #ffffff;
}

.badge-condition--excelente {
  background-color: rgba(13, 148, 136, 0.95);
  color: #ffffff;
}

.badge-condition--bueno {
  background-color: rgba(2, 132, 199, 0.95);
  color: #ffffff;
}

.badge-condition--reciclaje {
  background-color: rgba(217, 119, 6, 0.95);
  color: #ffffff;
}

.badge-savings {
  background-color: rgba(225, 29, 72, 0.95);
  color: #ffffff;
  font-size: 0.72rem;
  font-weight: 800;
  padding: 4px 8px;
  border-radius: 6px;
}

.badge-bim-indicator {
  position: absolute;
  bottom: 12px;
  right: 12px;
  background-color: rgba(15, 23, 42, 0.88);
  color: #38bdf8;
  font-size: 0.75rem;
  font-weight: 700;
  padding: 5px 10px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  gap: 6px;
  border: 1px solid rgba(56, 189, 248, 0.3);
  backdrop-filter: blur(4px);
}

.icon-cube {
  width: 14px;
  height: 14px;
}

/* Cuerpo de la Tarjeta */
.card-body {
  padding: 16px 18px 12px 18px;
  display: flex;
  flex-direction: column;
  flex: 1;
}

.card-meta-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
  font-size: 0.75rem;
}

.sku-code {
  font-family: 'JetBrains Mono', 'Courier New', monospace;
  font-weight: 600;
  color: #64748b;
  letter-spacing: 0.02em;
}

.location-tag {
  display: flex;
  align-items: center;
  gap: 4px;
  color: #475569;
  font-weight: 500;
}

.icon-pin {
  width: 13px;
  height: 13px;
  color: #0d9488;
}

.material-title {
  font-size: 1.05rem;
  font-weight: 700;
  line-height: 1.35;
  color: #0f172a;
  margin: 0 0 4px 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  height: 2.7em;
}

.origin-project {
  font-size: 0.78rem;
  color: #64748b;
  margin: 0 0 12px 0;
}

.origin-project strong {
  color: #334155;
}

/* Panel Técnico de Dimensiones (Estilo Plano Arquitectónico) */
.technical-specs-panel {
  background-color: #f8fafc;
  border: 1px solid #e2e8f0;
  border-left: 3px solid #0284c7;
  border-radius: 6px;
  padding: 10px 12px;
  margin-bottom: 12px;
}

.spec-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
}

.spec-label {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 0.75rem;
  font-weight: 600;
  color: #475569;
  text-transform: uppercase;
  letter-spacing: 0.02em;
}

.icon-ruler {
  width: 14px;
  height: 14px;
  color: #0284c7;
}

.spec-value-dim {
  font-family: 'JetBrains Mono', monospace;
  font-size: 0.82rem;
  font-weight: 700;
  color: #0369a1;
  background-color: #e0f2fe;
  padding: 2px 7px;
  border-radius: 4px;
}

.specs-grid-mini {
  display: flex;
  justify-content: space-between;
  padding-top: 6px;
  border-top: 1px dashed #cbd5e1;
  font-size: 0.75rem;
}

.mini-spec {
  display: flex;
  gap: 5px;
}

.mini-label {
  color: #64748b;
}

.mini-value {
  font-weight: 600;
  color: #1e293b;
}

.highlight-green {
  color: #059669;
}

/* Precios */
.pricing-container {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  margin-bottom: 10px;
}

.price-main {
  display: flex;
  align-items: baseline;
  gap: 4px;
}

.currency-tag {
  font-size: 0.75rem;
  font-weight: 700;
  color: #64748b;
}

.price-value {
  font-size: 1.35rem;
  font-weight: 800;
  color: #0f172a;
  letter-spacing: -0.02em;
}

.price-unit {
  font-size: 0.75rem;
  color: #64748b;
  font-weight: 500;
}

.price-reference {
  font-size: 0.75rem;
  color: #94a3b8;
}

/* Huella Ambiental / Trazabilidad */
.environmental-footprint {
  margin-bottom: 14px;
}

.footprint-pill {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background-color: #ecfdf5;
  color: #047857;
  border: 1px solid #a7f3d0;
  padding: 4px 8px;
  border-radius: 6px;
  font-size: 0.72rem;
  font-weight: 600;
}

.icon-leaf {
  width: 13px;
  height: 13px;
  stroke: #059669;
}

/* Acciones */
.card-actions {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
  padding: 12px 18px 16px 18px;
  background-color: #f8fafc;
  border-top: 1px solid #f1f5f9;
}

button {
  cursor: pointer;
  border: none;
  border-radius: 7px;
  font-size: 0.82rem;
  font-weight: 600;
  padding: 9px 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  transition: all 0.2s ease;
}

.btn-bim-download {
  background: linear-gradient(135deg, #0284c7 0%, #0369a1 100%);
  color: #ffffff;
  box-shadow: 0 2px 4px rgba(2, 132, 199, 0.25);
}

.btn-bim-download:hover {
  background: linear-gradient(135deg, #0369a1 0%, #075985 100%);
  transform: translateY(-1px);
}

.bim-badge-sub {
  background-color: rgba(255, 255, 255, 0.25);
  padding: 1px 4px;
  border-radius: 3px;
  font-size: 0.65rem;
  margin-left: 4px;
  text-transform: uppercase;
}

.icon-bim-btn {
  width: 15px;
  height: 15px;
}

.btn-request-transaction {
  background-color: #0f172a;
  color: #ffffff;
}

.btn-request-transaction:hover {
  background-color: #1e293b;
  transform: translateY(-1px);
}

.btn-bim-locked {
  background: #64748b;
  opacity: 0.8;
}
</style>
