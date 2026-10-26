<script setup>
import { computed } from 'vue'
import { transactionsList } from '../stores/materialsStore.js'

const props = defineProps({
  onNotify: {
    type: Function,
    default: () => {}
  }
})

const formatCurrency = (val) => {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0
  }).format(val)
}

const totalCommissionsEarned = computed(() => {
  return transactionsList.value.reduce((acc, t) => acc + t.platformFeeCop, 0)
})

const totalGmv = computed(() => {
  return transactionsList.value.reduce((acc, t) => acc + t.totalCop, 0)
})

const handleReleaseEscrow = (tx) => {
  tx.status = 'DELIVERED_RECEIVED'
  props.onNotify(
    'Fondos Liberados (Escrow)',
    `Inspección aprobada para transacción ${tx.code}. Comisión de ${formatCurrency(tx.platformFeeCop)} liquidada a favor de REARQ BIM.`,
    'success'
  )
}

const handleDownloadRemision = (tx) => {
  props.onNotify(
    'Acta de Despacho',
    `Generando acta oficial de entrega y cadena de custodia RCD para la orden ${tx.code}.`,
    'info'
  )
}
</script>

<template>
  <div class="view-transactions">
    <div class="transactions-header">
      <div>
        <h2>Motor Transaccional & Coordinación de Intercambios</h2>
        <p>Control de compras, custodia financiera (Escrow) y liquidación de comisión de plataforma (5% al 8%).</p>
      </div>

      <!-- Resumen de Métricas Financieras Transaccionales -->
      <div class="tx-metrics-bar">
        <div class="tx-metric-pill">
          <span>GMV Transaccionado:</span>
          <strong>{{ formatCurrency(totalGmv) }}</strong>
        </div>
        <div class="tx-metric-pill highlight-cyan">
          <span>Comisiones de Plataforma (7%):</span>
          <strong>{{ formatCurrency(totalCommissionsEarned) }}</strong>
        </div>
      </div>
    </div>

    <!-- Lista de Órdenes Transaccionales -->
    <div class="tx-cards-container">
      <div v-for="tx in transactionsList" :key="tx.id" class="tx-card">
        <!-- Encabezado de la Orden -->
        <div class="tx-card-top">
          <div>
            <span class="tx-code">{{ tx.code }}</span>
            <span class="tx-date">Fecha: {{ tx.date }}</span>
          </div>

          <span 
            class="status-badge"
            :class="{
              'status-escrow': tx.status === 'ESCROW_FUNDED',
              'status-delivered': tx.status === 'DELIVERED_RECEIVED'
            }"
          >
            {{ tx.status === 'ESCROW_FUNDED' ? 'Custodia Asegurada (Escrow)' : 'Recibido y Verificado' }}
          </span>
        </div>

        <!-- Detalle de Material y Obra -->
        <div class="tx-card-body">
          <h3 class="tx-material-title">{{ tx.materialTitle }}</h3>
          
          <div class="logistics-route">
            <div class="route-point">
              <span class="route-label">Obra Origen:</span>
              <span class="route-val">{{ tx.originProject }}</span>
            </div>
            <div class="route-arrow">➔</div>
            <div class="route-point">
              <span class="route-label">Obra Destino:</span>
              <span class="route-val">{{ tx.buyerProject }}</span>
            </div>
          </div>

          <!-- Desglose Financiero B2B con Comisión -->
          <div class="financial-breakdown">
            <div class="fin-row">
              <span>Cantidad Transferida:</span>
              <strong>{{ tx.quantity }} unidades</strong>
            </div>
            <div class="fin-row">
              <span>Subtotal Materiales:</span>
              <span>{{ formatCurrency(tx.subtotalMaterialsCop) }}</span>
            </div>
            <div class="fin-row highlight-fee">
              <span>Tarifa de Intermediación REARQ BIM ({{ tx.commissionPct }}%):</span>
              <span>+ {{ formatCurrency(tx.platformFeeCop) }}</span>
            </div>
            <div class="fin-row total-row">
              <span>Total Liquidado:</span>
              <span class="total-amount">{{ formatCurrency(tx.totalCop) }}</span>
            </div>
          </div>

          <!-- Impacto RCD de la transacción -->
          <div class="tx-rcd-footer">
            <span>Impacto Ambiental: <strong>{{ tx.rcdMassDivertedTons }} Ton RCD</strong> desviadas · Certificado {{ tx.certificateCode }}</span>
          </div>
        </div>

        <!-- Acciones Transaccionales -->
        <div class="tx-card-actions">
          <button class="btn-tx-secondary" @click="handleDownloadRemision(tx)">
            Descargar Acta de Entrega
          </button>
          <button 
            v-if="tx.status === 'ESCROW_FUNDED'" 
            class="btn-tx-primary" 
            @click="handleReleaseEscrow(tx)"
          >
            Confirmar Inspección y Liberar Pago
          </button>
          <button v-else class="btn-tx-disabled" disabled>
            ✓ Transacción Concluida
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.view-transactions {
  display: flex;
  flex-direction: column;
  gap: 22px;
}

.transactions-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.transactions-header h2 {
  font-size: 1.35rem;
  font-weight: 800;
  color: #0f172a;
}

.transactions-header p {
  font-size: 0.85rem;
  color: #64748b;
  margin-top: 4px;
}

.tx-metrics-bar {
  display: flex;
  gap: 12px;
}

.tx-metric-pill {
  background-color: #ffffff;
  border: 1px solid #cbd5e1;
  padding: 8px 16px;
  border-radius: 8px;
  font-size: 0.82rem;
  display: flex;
  flex-direction: column;
}

.tx-metric-pill strong {
  font-size: 1.05rem;
  color: #0f172a;
  margin-top: 2px;
}

.highlight-cyan {
  border-color: #38bdf8;
  background-color: #f0f9ff;
}

.highlight-cyan strong {
  color: #0284c7;
}

/* Cards */
.tx-cards-container {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.tx-card {
  background-color: #ffffff;
  border-radius: 10px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(15, 23, 42, 0.05);
  overflow: hidden;
}

.tx-card-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 14px 20px;
  background-color: #f8fafc;
  border-bottom: 1px solid #e2e8f0;
}

.tx-code {
  font-family: 'JetBrains Mono', monospace;
  font-weight: 700;
  color: #0f172a;
  font-size: 0.95rem;
  margin-right: 14px;
}

.tx-date {
  font-size: 0.78rem;
  color: #64748b;
}

.status-badge {
  font-size: 0.75rem;
  font-weight: 700;
  padding: 4px 12px;
  border-radius: 20px;
}

.status-escrow {
  background-color: #fef3c7;
  color: #b45309;
}

.status-delivered {
  background-color: #d1fae5;
  color: #047857;
}

.tx-card-body {
  padding: 20px;
}

.tx-material-title {
  font-size: 1.15rem;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 14px;
}

.logistics-route {
  display: flex;
  align-items: center;
  gap: 14px;
  background-color: #f8fafc;
  padding: 12px 16px;
  border-radius: 6px;
  border: 1px dashed #cbd5e1;
  margin-bottom: 18px;
}

.route-point {
  display: flex;
  flex-direction: column;
}

.route-label {
  font-size: 0.7rem;
  color: #64748b;
  text-transform: uppercase;
  font-weight: 600;
}

.route-val {
  font-size: 0.85rem;
  font-weight: 700;
  color: #334155;
}

.route-arrow {
  color: #0284c7;
  font-size: 1.1rem;
}

/* Financial Breakdown */
.financial-breakdown {
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 14px 18px;
  font-size: 0.85rem;
  margin-bottom: 14px;
}

.fin-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 6px;
  color: #475569;
}

.highlight-fee {
  color: #0284c7;
  font-weight: 600;
}

.total-row {
  border-top: 1px solid #e2e8f0;
  padding-top: 8px;
  margin-top: 8px;
  font-size: 0.95rem;
  color: #0f172a;
  font-weight: 700;
}

.total-amount {
  font-size: 1.15rem;
  font-weight: 800;
  color: #0f172a;
}

.tx-rcd-footer {
  font-size: 0.78rem;
  color: #059669;
}

.tx-card-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 12px 20px;
  background-color: #f8fafc;
  border-top: 1px solid #f1f5f9;
}

.btn-tx-secondary {
  background-color: #ffffff;
  border: 1px solid #cbd5e1;
  color: #334155;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 0.82rem;
  font-weight: 600;
  cursor: pointer;
}

.btn-tx-primary {
  background: linear-gradient(135deg, #059669 0%, #0d9488 100%);
  color: #ffffff;
  border: none;
  padding: 8px 18px;
  border-radius: 6px;
  font-size: 0.82rem;
  font-weight: 700;
  cursor: pointer;
}

.btn-tx-disabled {
  background-color: #f1f5f9;
  border: 1px solid #e2e8f0;
  color: #059669;
  font-weight: 700;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 0.82rem;
}
</style>
