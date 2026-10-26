<script setup>
import { ref, computed } from 'vue'
import { currentCompany, addMaterial } from '../stores/materialsStore.js'

const props = defineProps({
  onNotify: {
    type: Function,
    default: () => {}
  }
})

const emit = defineEmits(['navigate'])

// Formulario reactivo
const originProjectId = ref(currentCompany.value.activeProjects[0].id)
const title = ref('')
const category = ref('CARPINTERIA_Y_MADERAS')
const condition = ref('EXCELENTE')
const technicalDescription = ref('')

const lengthMm = ref(2100)
const widthMm = ref(900)
const heightMm = ref(45)
const unitOfMeasure = ref('UNIDAD')
const weightKgPerUnit = ref(25)

const availableQuantity = ref(20)
const minimumOrder = ref(1)
const unitPriceCop = ref(180000)
const originalRetailPriceCop = ref(380000)

const hasBim = ref(true)
const bimFormat = ref('RFA')
const bimLod = ref('LOD_300')
const bimVersion = ref('Autodesk Revit 2024 / IFC4')

// Cálculos dinámicos en tiempo real (Ingeniería de Sostenibilidad)
const savingsPct = computed(() => {
  if (originalRetailPriceCop.value > 0 && originalRetailPriceCop.value > unitPriceCop.value) {
    return Math.round(((originalRetailPriceCop.value - unitPriceCop.value) / originalRetailPriceCop.value) * 100)
  }
  return 0
})

const totalRcdDivertedTons = computed(() => {
  const kg = (weightKgPerUnit.value || 0) * (availableQuantity.value || 0)
  return (kg / 1000).toFixed(3)
})

const totalCo2Avoided = computed(() => {
  // Factor de emisión evitado estimado (0.5 kg CO2e por kg de material circular reutilizado)
  const kg = (weightKgPerUnit.value || 0) * (availableQuantity.value || 0)
  return (kg * 0.48).toFixed(1)
})

// Envío del formulario
const submitForm = () => {
  if (!title.value.trim()) {
    props.onNotify('Campo Requerido', 'Por favor ingresa el título o denominación técnica del material.', 'error')
    return
  }

  const selectedProj = currentCompany.value.activeProjects.find(p => p.id === originProjectId.value)

  const newMaterial = {
    id: `mat-${Date.now()}`,
    sku: `MAT-2026-${Math.floor(200 + Math.random() * 800)}`,
    title: title.value,
    category: category.value,
    condition: condition.value,
    technicalDescription: technicalDescription.value || 'Especificación técnica verificada por residente de obra.',
    lengthMm: lengthMm.value,
    widthMm: widthMm.value,
    heightMm: heightMm.value,
    unitOfMeasure: unitOfMeasure.value,
    weightKgPerUnit: weightKgPerUnit.value,
    availableQuantity: availableQuantity.value,
    unitPriceCop: unitPriceCop.value,
    originalRetailPriceCop: originalRetailPriceCop.value,
    savingsPercentage: savingsPct.value,
    embodiedCarbonKgCo2ePerUnit: (totalCo2Avoided.value / availableQuantity.value).toFixed(1),
    rcdNormativeCode: 'RCD-GEN-2026',
    originProjectName: selectedProj ? selectedProj.name : 'Obra Registrada',
    city: selectedProj ? selectedProj.city : 'Bogotá D.C.',
    imageUrl: 'https://images.unsplash.com/photo-1504307651254-35680f356dfd?auto=format&fit=crop&w=600&q=80',
    hasBimAsset: hasBim.value,
    bimAsset: hasBim.value ? {
      fileFormat: bimFormat.value,
      lodLevel: bimLod.value,
      bimSoftwareVersion: bimVersion.value,
      fileSizeBytes: 6291456,
      downloadCount: 0
    } : null
  }

  addMaterial(newMaterial)

  props.onNotify(
    'Publicación Exitosa',
    `Material "${newMaterial.title}" publicado en REARQ BIM con SKU ${newMaterial.sku}. Objeto BIM y ficha técnica disponibles.`,
    'success'
  )

  emit('navigate', 'catalog')
}
</script>

<template>
  <div class="view-publish">
    <div class="publish-header">
      <div>
        <h2>Publicar Sobrante de Obra (Economía Circular)</h2>
        <p>Registra excedentes o materiales recuperados con especificaciones técnicas exactas para su reutilización en nuevas obras.</p>
      </div>
      <button class="btn-cancel" @click="emit('navigate', 'catalog')">Volver al Catálogo</button>
    </div>

    <form class="publish-form" @submit.prevent="submitForm">
      <!-- 1. Identificación y Obra de Origen -->
      <section class="form-section">
        <h3 class="section-title">1. Obra de Origen y Clasificación</h3>
        <div class="form-grid-2">
          <div class="form-group">
            <label>Obra / Proyecto de Origen *</label>
            <select v-model="originProjectId" class="form-control" required>
              <option v-for="proj in currentCompany.activeProjects" :key="proj.id" :value="proj.id">
                {{ proj.name }} ({{ proj.city }})
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Categoría del Material *</label>
            <select v-model="category" class="form-control" required>
              <option value="CARPINTERIA_Y_MADERAS">Carpintería y Maderas</option>
              <option value="MAMPOSTERIA_Y_BLOQUES">Mampostería y Bloques</option>
              <option value="ESTRUCTURAS_Y_CONCRETO">Estructuras y Acero</option>
              <option value="ACABADOS_Y_CERAMICA">Acabados y Cerámica</option>
              <option value="HIDROSANITARIO_Y_GRIFERIA">Hidrosanitario y Tuberías</option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label>Título o Nombre Técnico del Sobrante *</label>
          <input 
            v-model="title" 
            type="text" 
            placeholder="Ej. Puertas Entamboradas en Cedro 2.10x0.90m con Marcos"
            class="form-control" 
            required 
          />
        </div>

        <div class="form-grid-2">
          <div class="form-group">
            <label>Estado Técnico de Conservación *</label>
            <select v-model="condition" class="form-control" required>
              <option value="NUEVO_EXCEDENTE">Nuevo / Excedente de Compra (En empaque original)</option>
              <option value="EXCELENTE">Excelente Estado (Recuperado sin defectos estructurales)</option>
              <option value="BUENO_REUTILIZABLE">Bueno Reutilizable (Defectos estéticos menores)</option>
              <option value="APTO_RECICLAJE">Apto para Transformación / Reciclaje de Áridos</option>
            </select>
          </div>

          <div class="form-group">
            <label>Código PGI-RCD de la Obra</label>
            <input type="text" value="PGI-RCD-BOG-0421" class="form-control" readonly />
          </div>
        </div>

        <div class="form-group">
          <label>Descripción Técnica y Criterios de Calidad</label>
          <textarea 
            v-model="technicalDescription" 
            rows="3" 
            placeholder="Indica condiciones de almacenamiento, tratamientos aplicados, ensayos o certificaciones de laboratorio disponibles..."
            class="form-control"
          ></textarea>
        </div>
      </section>

      <!-- 2. Tolerancias Dimensionales Exactas (Blueprint Specs) -->
      <section class="form-section blueprint-section">
        <div class="blueprint-badge">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" class="icon-ruler">
            <path d="M3 6l18 0M3 12l18 0M3 18l18 0" stroke-width="2" stroke-linecap="round"/>
          </svg>
          Especificaciones Dimensionales Exactas (Estándar BIM en Milímetros)
        </div>

        <div class="form-grid-3">
          <div class="form-group">
            <label>Largo (mm) *</label>
            <input v-model.number="lengthMm" type="number" step="0.1" class="form-control code-input" required />
          </div>

          <div class="form-group">
            <label>Ancho (mm) *</label>
            <input v-model.number="widthMm" type="number" step="0.1" class="form-control code-input" required />
          </div>

          <div class="form-group">
            <label>Alto / Espesor (mm) *</label>
            <input v-model.number="heightMm" type="number" step="0.1" class="form-control code-input" required />
          </div>
        </div>

        <div class="form-grid-2">
          <div class="form-group">
            <label>Unidad de Medida *</label>
            <select v-model="unitOfMeasure" class="form-control">
              <option value="UNIDAD">UNIDAD</option>
              <option value="m2">Metros Cuadrados (m²)</option>
              <option value="m3">Metros Cúbicos (m³)</option>
              <option value="ml">Metros Lineales (ml)</option>
              <option value="kg">Kilogramos (kg)</option>
            </select>
          </div>

          <div class="form-group">
            <label>Peso Unitario Estimado (kg) *</label>
            <input v-model.number="weightKgPerUnit" type="number" step="0.01" class="form-control code-input" required />
            <small class="helper-text">Requerido para el cálculo oficial de masa RCD según Resolución 1257.</small>
          </div>
        </div>
      </section>

      <!-- 3. Integración BIM (Revit & IFC) -->
      <section class="form-section">
        <h3 class="section-title">3. Integración con Modelos BIM (Revit / IFC)</h3>
        
        <div class="form-check-bim">
          <label class="custom-toggle">
            <input v-model="hasBim" type="checkbox" />
            <span class="toggle-slider"></span>
          </label>
          <div>
            <strong>Asociar Objeto / Familia BIM Descargable</strong>
            <p class="toggle-desc">Permite a los arquitectos e ingenieros descargar el objeto para especificarlo directamente en el diseño.</p>
          </div>
        </div>

        <div v-if="hasBim" class="form-grid-3 bim-box">
          <div class="form-group">
            <label>Formato BIM *</label>
            <select v-model="bimFormat" class="form-control">
              <option value="RFA">Autodesk Revit (.RFA)</option>
              <option value="IFC">OpenBIM IFC (.IFC)</option>
              <option value="RVT">Proyecto Revit (.RVT)</option>
            </select>
          </div>

          <div class="form-group">
            <label>Nivel de Detalle (LOD) *</label>
            <select v-model="bimLod" class="form-control">
              <option value="LOD_300">LOD 300 (Geometría y dimensiones exactas)</option>
              <option value="LOD_350">LOD 350 (Conexiones y anclajes técnicos)</option>
              <option value="LOD_200">LOD 200 (Genérico aproximado)</option>
            </select>
          </div>

          <div class="form-group">
            <label>Versión Compatible</label>
            <input v-model="bimVersion" type="text" class="form-control" />
          </div>
        </div>
      </section>

      <!-- 4. Inventario, Precios y Métricas Ambientales -->
      <section class="form-section">
        <h3 class="section-title">4. Cantidades, Precios y Métricas Ambientales</h3>
        
        <div class="form-grid-3">
          <div class="form-group">
            <label>Cantidad Disponible *</label>
            <input v-model.number="availableQuantity" type="number" min="1" class="form-control" required />
          </div>

          <div class="form-group">
            <label>Precio Unitario de Venta (COP) *</label>
            <input v-model.number="unitPriceCop" type="number" step="100" class="form-control" required />
          </div>

          <div class="form-group">
            <label>Precio Comercial Referencia Nuevo (COP)</label>
            <input v-model.number="originalRetailPriceCop" type="number" step="100" class="form-control" />
          </div>
        </div>

        <!-- Panel de Impacto Calculado -->
        <div class="impact-calculator-preview">
          <div class="calc-card">
            <span class="calc-label">Ahorro para Obra Compradora:</span>
            <span class="calc-val highlight-rose">-{{ savingsPct }}%</span>
          </div>
          <div class="calc-card">
            <span class="calc-label">Masa RCD Desviada de Vertedero:</span>
            <span class="calc-val highlight-green">{{ totalRcdDivertedTons }} Toneladas</span>
          </div>
          <div class="calc-card">
            <span class="calc-label">Huella CO₂ Evitada (Res. 1257):</span>
            <span class="calc-val highlight-cyan">{{ totalCo2Avoided }} kg CO₂e</span>
          </div>
        </div>
      </section>

      <!-- Botón de Envío -->
      <div class="form-actions">
        <button type="button" class="btn-secondary" @click="emit('navigate', 'catalog')">Cancelar</button>
        <button type="submit" class="btn-primary-publish">
          Publicar Material en Catálogo B2B
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.view-publish {
  max-width: 900px;
  margin: 0 auto;
}

.publish-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.publish-header h2 {
  font-size: 1.4rem;
  font-weight: 800;
  color: #0f172a;
}

.publish-header p {
  font-size: 0.85rem;
  color: #64748b;
  margin-top: 4px;
}

.btn-cancel {
  background-color: #ffffff;
  border: 1px solid #cbd5e1;
  color: #334155;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 0.82rem;
  font-weight: 600;
  cursor: pointer;
}

.publish-form {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.form-section {
  background-color: #ffffff;
  padding: 22px 26px;
  border-radius: 10px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(15, 23, 42, 0.05);
}

.section-title {
  font-size: 1.05rem;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 16px;
  padding-bottom: 8px;
  border-bottom: 1px solid #f1f5f9;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-bottom: 14px;
}

.form-group label {
  font-size: 0.8rem;
  font-weight: 600;
  color: #475569;
}

.form-control {
  padding: 9px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 0.88rem;
  color: #0f172a;
  outline: none;
  transition: border-color 0.2s;
}

.form-control:focus {
  border-color: #0284c7;
}

.code-input {
  font-family: 'JetBrains Mono', monospace;
  font-weight: 600;
}

.helper-text {
  font-size: 0.72rem;
  color: #64748b;
}

.form-grid-2 {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-grid-3 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 16px;
}

/* Blueprint Section */
.blueprint-section {
  background-color: #f8fafc;
  border-left: 4px solid #0284c7;
}

.blueprint-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.82rem;
  font-weight: 700;
  color: #0284c7;
  text-transform: uppercase;
  margin-bottom: 16px;
}

.icon-ruler {
  width: 18px;
  height: 18px;
}

/* Toggle BIM */
.form-check-bim {
  display: flex;
  align-items: center;
  gap: 14px;
  background-color: #f0fdf4;
  border: 1px solid #bbf7d0;
  padding: 12px 16px;
  border-radius: 8px;
  margin-bottom: 16px;
}

.toggle-desc {
  font-size: 0.75rem;
  color: #166534;
}

.bim-box {
  background-color: #f8fafc;
  border: 1px dashed #cbd5e1;
  padding: 14px;
  border-radius: 8px;
}

/* Impact Calculator */
.impact-calculator-preview {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 14px;
  margin-top: 14px;
  background: #0f172a;
  padding: 14px 18px;
  border-radius: 8px;
  color: #ffffff;
}

.calc-card {
  display: flex;
  flex-direction: column;
}

.calc-label {
  font-size: 0.7rem;
  color: #94a3b8;
}

.calc-val {
  font-size: 1.15rem;
  font-weight: 800;
  margin-top: 4px;
}

.highlight-rose {
  color: #fb7185;
}

.highlight-green {
  color: #34d399;
}

.highlight-cyan {
  color: #38bdf8;
}

/* Actions */
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 14px;
  padding: 16px 0;
}

.btn-secondary {
  background-color: #ffffff;
  border: 1px solid #cbd5e1;
  padding: 10px 20px;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
}

.btn-primary-publish {
  background: linear-gradient(135deg, #0284c7 0%, #0d9488 100%);
  color: #ffffff;
  border: none;
  padding: 10px 24px;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 700;
  cursor: pointer;
  box-shadow: 0 4px 6px -1px rgba(2, 132, 199, 0.3);
}

.btn-primary-publish:hover {
  opacity: 0.95;
}
</style>
