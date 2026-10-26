<script setup>
import { currentCompany } from '../stores/materialsStore.js'

const props = defineProps({
  onNotify: {
    type: Function,
    default: () => {}
  }
})

const handleSelectPlan = (tier, name, price) => {
  currentCompany.value.plan = `${name} (${price})`
  currentCompany.value.planTier = tier
  props.onNotify(
    'Plan Actualizado',
    `Su empresa ahora cuenta con el ${name}. Funciones BIM y cupos de obra actualizados en tiempo real.`,
    'success'
  )
}
</script>

<template>
  <div class="view-subscriptions">
    <div class="subs-header">
      <div>
        <h2>Suscripciones SaaS & Módulo de Integración BIM</h2>
        <p>Habilita la descarga de familias paramétricas de Revit, modelos IFC y reportes certificados PGI-RCD para sus proyectos.</p>
      </div>
      <div class="current-plan-card">
        <span class="plan-label">Plan Actual de la Constructora:</span>
        <strong class="plan-val">{{ currentCompany.plan }}</strong>
        <span class="active-indicator">● Suscripción Vigente</span>
      </div>
    </div>

    <!-- Gestión de Obras Asignadas -->
    <div class="projects-assigned-card">
      <div class="assigned-header">
        <div>
          <h3>Obras Activas en Suscripción ({{ currentCompany.activeProjects.length }} de 3 Cupos Usados)</h3>
          <p>Obras autorizadas para registrar sobrantes de materiales y descargar familias BIM.</p>
        </div>
        <button class="btn-add-work" @click="props.onNotify('Nueva Obra', 'Apertura de formulario de registro de licencia y código PGI-RCD.', 'info')">
          + Agregar Nueva Obra
        </button>
      </div>

      <div class="projects-chips-grid">
        <div v-for="proj in currentCompany.activeProjects" :key="proj.id" class="project-chip">
          <div class="chip-icon">🏢</div>
          <div>
            <strong>{{ proj.name }}</strong>
            <span class="chip-city">{{ proj.city }} · Licencia Curaduría Verificada</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Comparativa de Planes B2B -->
    <div class="plans-grid">
      <!-- Plan Básico -->
      <div class="pricing-tier" :class="{ 'tier-selected': currentCompany.planTier === 'BASIC_FREE' }">
        <div class="tier-header">
          <span class="tier-badge">Plan Explorador</span>
          <h4>Básico Free</h4>
          <div class="tier-price">
            <span class="price-val">$0</span>
            <span class="price-period">COP / mes</span>
          </div>
          <p class="tier-desc">Para consulta general del catálogo de excedentes en obra sin requerimientos BIM.</p>
        </div>

        <ul class="tier-features">
          <li>✓ Visualización de inventario disponible</li>
          <li>✓ Filtros por ubicación y categoría</li>
          <li>✕ Sin descarga de objetos BIM (.RFA / .IFC)</li>
          <li>✕ Sin certificados auditables PGI-RCD</li>
          <li>✕ 1 sola obra registrada</li>
        </ul>

        <button 
          class="btn-tier-action"
          :disabled="currentCompany.planTier === 'BASIC_FREE'"
          @click="handleSelectPlan('BASIC_FREE', 'Plan Básico', '$0/mes')"
        >
          {{ currentCompany.planTier === 'BASIC_FREE' ? 'Plan Activo' : 'Seleccionar Básico' }}
        </button>
      </div>

      <!-- Plan Pro Project (El recomendado para obras) -->
      <div class="pricing-tier tier-featured" :class="{ 'tier-selected': currentCompany.planTier === 'PRO_PROJECT' }">
        <div class="popular-ribbon">Recomendado para Obras</div>
        <div class="tier-header">
          <span class="tier-badge badge-pro">Plan Pro Obra</span>
          <h4>Pro Project</h4>
          <div class="tier-price">
            <span class="price-val">$1.500.000</span>
            <span class="price-period">COP / mes por proyecto</span>
          </div>
          <p class="tier-desc">Integración completa con la fase de diseño arquitectónico y normatividad ambiental.</p>
        </div>

        <ul class="tier-features">
          <li>✓ <strong>Descargas ilimitadas de Familias Revit (.RFA)</strong></li>
          <li>✓ <strong>Descargas de modelos OpenBIM (.IFC) LOD 300+</strong></li>
          <li>✓ Generación automatizada de Certificados PGI-RCD</li>
          <li>✓ Hasta 3 obras activas simultáneas</li>
          <li>✓ Comisión preferencial reducida del 6%</li>
          <li>✓ Cumplimiento Resolución 1257 de 2021</li>
        </ul>

        <button 
          class="btn-tier-action btn-pro-action"
          :disabled="currentCompany.planTier === 'PRO_PROJECT'"
          @click="handleSelectPlan('PRO_PROJECT', 'Plan Pro Project', '$1.5M/mes')"
        >
          {{ currentCompany.planTier === 'PRO_PROJECT' ? 'Plan Actualmente Activo' : 'Mejorar a Pro Project' }}
        </button>
      </div>

      <!-- Plan Enterprise Corp -->
      <div class="pricing-tier" :class="{ 'tier-selected': currentCompany.planTier === 'ENTERPRISE_CORP' }">
        <div class="tier-header">
          <span class="tier-badge badge-ent">Constructoras Multi-Obra</span>
          <h4>Enterprise Corp</h4>
          <div class="tier-price">
            <span class="price-val">$3.000.000</span>
            <span class="price-period">COP / mes corporativo</span>
          </div>
          <p class="tier-desc">Para grandes firmas constructoras y promotoras con múltiples frentes de trabajo.</p>
        </div>

        <ul class="tier-features">
          <li>✓ <strong>Obras activas ilimitadas a nivel nacional</strong></li>
          <li>✓ API REST de integración directa con Autodesk Construction Cloud / BIM 360</li>
          <li>✓ Acompañamiento y auditoría técnica para CAR / SDA</li>
          <li>✓ Tarifa de comisión mínima del 5%</li>
          <li>✓ Exportación de trazabilidad en CSV y JSON</li>
          <li>✓ Gestor de cuenta dedicado</li>
        </ul>

        <button 
          class="btn-tier-action"
          :disabled="currentCompany.planTier === 'ENTERPRISE_CORP'"
          @click="handleSelectPlan('ENTERPRISE_CORP', 'Plan Enterprise Corp', '$3.0M/mes')"
        >
          {{ currentCompany.planTier === 'ENTERPRISE_CORP' ? 'Plan Activo' : 'Mejorar a Enterprise' }}
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.view-subscriptions {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.subs-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.subs-header h2 {
  font-size: 1.35rem;
  font-weight: 800;
  color: #0f172a;
}

.subs-header p {
  font-size: 0.85rem;
  color: #64748b;
  margin-top: 4px;
}

.current-plan-card {
  background-color: #0f172a;
  color: #ffffff;
  padding: 12px 18px;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  border: 1px solid #1e293b;
}

.plan-label {
  font-size: 0.7rem;
  color: #94a3b8;
  text-transform: uppercase;
}

.plan-val {
  font-size: 1rem;
  font-weight: 800;
  color: #38bdf8;
  margin: 2px 0;
}

.active-indicator {
  font-size: 0.72rem;
  color: #34d399;
  font-weight: 600;
}

/* Obras Asignadas */
.projects-assigned-card {
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  padding: 20px;
  border-radius: 10px;
}

.assigned-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.assigned-header h3 {
  font-size: 1.05rem;
  font-weight: 700;
  color: #0f172a;
}

.assigned-header p {
  font-size: 0.8rem;
  color: #64748b;
}

.btn-add-work {
  background-color: #f1f5f9;
  border: 1px solid #cbd5e1;
  color: #0f172a;
  padding: 7px 14px;
  border-radius: 6px;
  font-size: 0.78rem;
  font-weight: 700;
  cursor: pointer;
}

.projects-chips-grid {
  display: flex;
  gap: 14px;
  flex-wrap: wrap;
}

.project-chip {
  display: flex;
  align-items: center;
  gap: 10px;
  background-color: #f8fafc;
  border: 1px solid #cbd5e1;
  padding: 10px 14px;
  border-radius: 8px;
}

.chip-icon {
  font-size: 1.3rem;
}

.chip-city {
  display: block;
  font-size: 0.72rem;
  color: #64748b;
}

/* Pricing Grid */
.plans-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 22px;
}

.pricing-tier {
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 26px;
  display: flex;
  flex-direction: column;
  position: relative;
  transition: transform 0.2s;
}

.pricing-tier:hover {
  transform: translateY(-3px);
}

.tier-featured {
  border: 2px solid #0284c7;
  box-shadow: 0 10px 25px -5px rgba(2, 132, 199, 0.15);
}

.popular-ribbon {
  position: absolute;
  top: -12px;
  left: 50%;
  transform: translateX(-50%);
  background-color: #0284c7;
  color: #ffffff;
  font-size: 0.7rem;
  font-weight: 800;
  padding: 3px 12px;
  border-radius: 12px;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.tier-badge {
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  color: #64748b;
  background: #f1f5f9;
  padding: 3px 8px;
  border-radius: 4px;
}

.badge-pro { background: #e0f2fe; color: #0369a1; }
.badge-ent { background: #ede9fe; color: #6d28d9; }

.tier-header h4 {
  font-size: 1.3rem;
  font-weight: 800;
  color: #0f172a;
  margin: 10px 0 6px 0;
}

.tier-price {
  display: flex;
  align-items: baseline;
  gap: 6px;
  margin-bottom: 8px;
}

.price-val {
  font-size: 1.8rem;
  font-weight: 800;
  color: #0f172a;
}

.price-period {
  font-size: 0.78rem;
  color: #64748b;
}

.tier-desc {
  font-size: 0.8rem;
  color: #64748b;
  line-height: 1.4;
  margin-bottom: 20px;
}

.tier-features {
  list-style: none;
  margin-bottom: 24px;
  display: flex;
  flex-direction: column;
  gap: 10px;
  font-size: 0.82rem;
  color: #334155;
  flex: 1;
}

.tier-features li {
  line-height: 1.35;
}

.btn-tier-action {
  width: 100%;
  padding: 10px;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 700;
  border: 1px solid #cbd5e1;
  background-color: #ffffff;
  color: #0f172a;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-pro-action {
  background: linear-gradient(135deg, #0284c7 0%, #0369a1 100%);
  color: #ffffff;
  border: none;
}

.btn-tier-action:disabled {
  background-color: #f1f5f9;
  color: #94a3b8;
  cursor: default;
}
</style>
