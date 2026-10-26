<script setup>
import { computed } from 'vue'
import { certificatesList, transactionsList, currentCompany } from '../stores/materialsStore.js'

const props = defineProps({
  onNotify: {
    type: Function,
    default: () => {}
  }
})

// Métricas de Impacto Acumulado
const totalDivertedTons = computed(() => {
  return certificatesList.value.reduce((acc, c) => acc + c.tonsDiverted, 0).toFixed(2)
})

const totalCo2Avoided = computed(() => {
  return certificatesList.value.reduce((acc, c) => acc + c.co2eAvoidedKg, 0).toFixed(1)
})

const totalEconomicSavings = computed(() => {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0
  }).format(18400000)
})

// Meta Resolución 1257 de 2021 (30% de aprovechamiento en grandes generadores)
const currentCompliancePercentage = computed(() => {
  const goal = 15.0 // Meta en toneladas para el proyecto de ejemplo
  const actual = parseFloat(totalDivertedTons.value)
  return Math.min(100, Math.round((actual / goal) * 100))
})

const downloadCertificatePdf = (cert) => {
  props.onNotify(
    'Certificado PGI-RCD Generado',
    `Descargando certificación oficial ${cert.code} para ${cert.project}. Válido para radicar ante la Secretaría Distrital de Ambiente / CAR.`,
    'success'
  )
}
</script>

<template>
  <div class="view-audit">
    <!-- Header del Dashboard -->
    <div class="audit-header">
      <div>
        <h2>Dashboard de Trazabilidad Ambiental & PGI-RCD</h2>
        <p>Certificación auditable para el cumplimiento de la <strong>Resolución 1257 de 2021</strong> y <strong>Resolución 0472 de 2017</strong> (MinAmbiente Colombia).</p>
      </div>
      <div class="authority-badge">
        <span class="auth-dot"></span>
        Entidad Receptora: CAR / SDA Bogotá / Corantioquia
      </div>
    </div>

    <!-- Tarjetas de Métricas Ambientales Clave -->
    <div class="kpi-grid">
      <div class="kpi-card">
        <div class="kpi-icon-wrap icon-green">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M11 20A7 7 0 0 1 9.8 6.1C15.5 5 17 4.48 19 2c1 2 2 4.18 2 8 0 5.5-4.78 10-10 10Z" stroke-width="2"/>
            <path d="M2 21c0-3 1.85-5.36 5.08-6C9.5 14.52 12 13 13 12" stroke-width="2"/>
          </svg>
        </div>
        <div class="kpi-info">
          <span class="kpi-label">Masa RCD Desviada de Vertederos</span>
          <span class="kpi-number">{{ totalDivertedTons }} <small>Ton</small></span>
          <span class="kpi-sub">Sobrantes reincorporados a nuevas obras</span>
        </div>
      </div>

      <div class="kpi-card">
        <div class="kpi-icon-wrap icon-cyan">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <circle cx="12" cy="12" r="10" stroke-width="2"/>
            <path d="M12 6v6l4 2" stroke-width="2"/>
          </svg>
        </div>
        <div class="kpi-info">
          <span class="kpi-label">Huella CO₂ Incorporada Evitada</span>
          <span class="kpi-number">{{ totalCo2Avoided }} <small>kg CO₂e</small></span>
          <span class="kpi-sub">Reducción en fabricación de materiales vírgenes</span>
        </div>
      </div>

      <div class="kpi-card">
        <div class="kpi-icon-wrap icon-blue">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" stroke-width="2"/>
            <polyline points="22 4 12 14.01 9 11.01" stroke-width="2"/>
          </svg>
        </div>
        <div class="kpi-info">
          <span class="kpi-label">Cumplimiento Meta Res. 1257</span>
          <span class="kpi-number">{{ currentCompliancePercentage }}%</span>
          <div class="progress-bar-bg">
            <div class="progress-bar-fill" :style="{ width: `${currentCompliancePercentage}%` }"></div>
          </div>
          <span class="kpi-sub">Meta obligatoria del 30% en proceso</span>
        </div>
      </div>

      <div class="kpi-card">
        <div class="kpi-icon-wrap icon-emerald">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <line x1="12" y1="1" x2="12" y2="23" stroke-width="2"/>
            <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6" stroke-width="2"/>
          </svg>
        </div>
        <div class="kpi-info">
          <span class="kpi-label">Ahorro Económico en Obra</span>
          <span class="kpi-number-sm">{{ totalEconomicSavings }}</span>
          <span class="kpi-sub">Ahorro en compras y costo de botadero</span>
        </div>
      </div>
    </div>

    <!-- Desglose por Tipología de RCD -->
    <div class="chart-section">
      <div class="chart-card">
        <h3>Distribución de Materiales Circulares Reutilizados</h3>
        <p class="chart-sub">Desglose de residuos reincorporados según categorización oficial</p>
        
        <div class="category-bars">
          <div class="cat-bar-item">
            <div class="cat-bar-header">
              <span>Mampostería y Bloques de Arcilla (NTC 4205)</span>
              <strong>5.2 Ton (68%)</strong>
            </div>
            <div class="cat-bar-track"><div class="cat-bar-fill bar-orange" style="width: 68%"></div></div>
          </div>

          <div class="cat-bar-item">
            <div class="cat-bar-header">
              <span>Carpintería y Maderas Reclaimed</span>
              <strong>1.8 Ton (22%)</strong>
            </div>
            <div class="cat-bar-track"><div class="cat-bar-fill bar-amber" style="width: 22%"></div></div>
          </div>

          <div class="cat-bar-item">
            <div class="cat-bar-header">
              <span>Acero Estructural y Metales</span>
              <strong>0.8 Ton (10%)</strong>
            </div>
            <div class="cat-bar-track"><div class="cat-bar-fill bar-blue" style="width: 10%"></div></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Tabla de Certificaciones Emitidas -->
    <div class="certificates-section">
      <div class="table-header-row">
        <div>
          <h3>Certificados Oficiales Emitidos para Radicación</h3>
          <p class="chart-sub">Documentos digitales con código criptográfico y sello de trazabilidad</p>
        </div>
      </div>

      <div class="table-responsive">
        <table class="cert-table">
          <thead>
            <tr>
              <th>Código Certificado</th>
              <th>Transacción</th>
              <th>Obra Beneficiaria</th>
              <th>Material Reutilizado</th>
              <th>Masa RCD</th>
              <th>CO₂ Evitado</th>
              <th>Fecha Emisión</th>
              <th>Acción</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="cert in certificatesList" :key="cert.code">
              <td class="font-mono"><strong>{{ cert.code }}</strong></td>
              <td class="font-mono">{{ cert.txCode }}</td>
              <td>{{ cert.project }}</td>
              <td>{{ cert.material }}</td>
              <td class="text-green"><strong>{{ cert.tonsDiverted }} Ton</strong></td>
              <td>{{ cert.co2eAvoidedKg }} kg</td>
              <td>{{ cert.issuedDate }}</td>
              <td>
                <button class="btn-download-pdf" @click="downloadCertificatePdf(cert)">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" class="icon-download">
                    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" stroke-width="2"/>
                    <polyline points="7 10 12 15 17 10" stroke-width="2"/>
                    <line x1="12" y1="15" x2="12" y2="3" stroke-width="2"/>
                  </svg>
                  PDF PGI-RCD
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<style scoped>
.view-audit {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.audit-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.audit-header h2 {
  font-size: 1.35rem;
  font-weight: 800;
  color: #0f172a;
}

.audit-header p {
  font-size: 0.85rem;
  color: #64748b;
  margin-top: 4px;
}

.authority-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  background-color: #f1f5f9;
  border: 1px solid #cbd5e1;
  padding: 6px 14px;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  color: #334155;
}

.auth-dot {
  width: 8px;
  height: 8px;
  background-color: #10b981;
  border-radius: 50%;
}

/* KPI Cards */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 18px;
}

.kpi-card {
  background-color: #ffffff;
  padding: 20px;
  border-radius: 10px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(15, 23, 42, 0.05);
  display: flex;
  gap: 16px;
  align-items: flex-start;
}

.kpi-icon-wrap {
  width: 44px;
  height: 44px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.kpi-icon-wrap svg {
  width: 22px;
  height: 22px;
}

.icon-green { background: #dcfce7; color: #15803d; }
.icon-cyan { background: #e0f2fe; color: #0369a1; }
.icon-blue { background: #ede9fe; color: #6d28d9; }
.icon-emerald { background: #ccfbf1; color: #0f766e; }

.kpi-info {
  display: flex;
  flex-direction: column;
  flex: 1;
}

.kpi-label {
  font-size: 0.75rem;
  font-weight: 600;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.02em;
}

.kpi-number {
  font-size: 1.6rem;
  font-weight: 800;
  color: #0f172a;
  letter-spacing: -0.02em;
  margin: 4px 0 2px 0;
}

.kpi-number small {
  font-size: 0.9rem;
  color: #64748b;
}

.kpi-number-sm {
  font-size: 1.25rem;
  font-weight: 800;
  color: #0f172a;
  margin: 4px 0 2px 0;
}

.kpi-sub {
  font-size: 0.72rem;
  color: #94a3b8;
}

.progress-bar-bg {
  width: 100%;
  height: 6px;
  background-color: #e2e8f0;
  border-radius: 3px;
  margin: 6px 0;
  overflow: hidden;
}

.progress-bar-fill {
  height: 100%;
  background: linear-gradient(90deg, #0284c7, #0d9488);
  border-radius: 3px;
}

/* Chart */
.chart-card {
  background-color: #ffffff;
  padding: 22px;
  border-radius: 10px;
  border: 1px solid #e2e8f0;
}

.chart-card h3 {
  font-size: 1.1rem;
  font-weight: 700;
  color: #1e293b;
}

.chart-sub {
  font-size: 0.8rem;
  color: #64748b;
  margin-bottom: 18px;
}

.category-bars {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.cat-bar-header {
  display: flex;
  justify-content: space-between;
  font-size: 0.82rem;
  color: #334155;
  margin-bottom: 6px;
}

.cat-bar-track {
  width: 100%;
  height: 10px;
  background-color: #f1f5f9;
  border-radius: 5px;
  overflow: hidden;
}

.cat-bar-fill { height: 100%; border-radius: 5px; }
.bar-orange { background-color: #f97316; }
.bar-amber { background-color: #f59e0b; }
.bar-blue { background-color: #0284c7; }

/* Table */
.certificates-section {
  background-color: #ffffff;
  padding: 22px;
  border-radius: 10px;
  border: 1px solid #e2e8f0;
}

.table-responsive {
  overflow-x: auto;
}

.cert-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.82rem;
  text-align: left;
}

.cert-table th {
  background-color: #f8fafc;
  padding: 10px 14px;
  color: #475569;
  font-weight: 700;
  border-bottom: 1px solid #e2e8f0;
  text-transform: uppercase;
  font-size: 0.72rem;
}

.cert-table td {
  padding: 12px 14px;
  border-bottom: 1px solid #f1f5f9;
  color: #334155;
}

.font-mono {
  font-family: 'JetBrains Mono', monospace;
}

.text-green {
  color: #059669;
}

.btn-download-pdf {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background-color: #0f172a;
  color: #ffffff;
  border: none;
  padding: 6px 12px;
  border-radius: 5px;
  font-size: 0.75rem;
  font-weight: 600;
  cursor: pointer;
}

.icon-download {
  width: 13px;
  height: 13px;
}
</style>
