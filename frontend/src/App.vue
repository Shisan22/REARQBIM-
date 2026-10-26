<script setup>
import { ref } from 'vue'
import { currentCompany, materialsList, transactionsList } from './stores/materialsStore.js'
import CatalogView from './views/CatalogView.vue'
import PublishMaterialView from './views/PublishMaterialView.vue'
import EnvironmentalAuditView from './views/EnvironmentalAuditView.vue'
import TransactionsView from './views/TransactionsView.vue'
import SubscriptionsView from './views/SubscriptionsView.vue'

// Vista activa actual en la navegación
const currentView = ref('catalog') // 'catalog' | 'publish' | 'audit' | 'transactions' | 'subscriptions'

// Notificaciones Toast Globales
const notification = ref(null)

const triggerNotification = (title, message, type = 'success') => {
  notification.value = { title, message, type }
  setTimeout(() => {
    notification.value = null
  }, 4500)
}

const navigateTo = (viewName) => {
  currentView.value = viewName
}
</script>

<template>
  <div class="rearq-app">
    <!-- Barra Superior Corporativa B2B -->
    <header class="app-header">
      <div class="header-container">
        <!-- Logo e Identidad -->
        <div class="brand" @click="navigateTo('catalog')">
          <div class="brand-logo">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"/>
              <polyline points="3.27 6.96 12 12.01 20.73 6.96" stroke-width="2.2"/>
              <line x1="12" y1="22.08" x2="12" y2="12" stroke-width="2.2"/>
            </svg>
          </div>
          <div>
            <span class="brand-title">REARQ <span class="highlight-cyan">BIM</span></span>
            <span class="brand-subtitle">Economía Circular para la Construcción · Res. 1257</span>
          </div>
        </div>

        <!-- Navegación Modular Principal (5 Módulos del Sistema) -->
        <nav class="app-nav">
          <button 
            class="nav-tab" 
            :class="{ active: currentView === 'catalog' }" 
            @click="navigateTo('catalog')"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" class="tab-icon">
              <rect x="3" y="3" width="7" height="7" stroke-width="2"/>
              <rect x="14" y="3" width="7" height="7" stroke-width="2"/>
              <rect x="14" y="14" width="7" height="7" stroke-width="2"/>
              <rect x="3" y="14" width="7" height="7" stroke-width="2"/>
            </svg>
            <span>Catálogo Técnico</span>
            <span class="tab-badge">{{ materialsList.length }}</span>
          </button>

          <button 
            class="nav-tab" 
            :class="{ active: currentView === 'publish' }" 
            @click="navigateTo('publish')"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" class="tab-icon">
              <circle cx="12" cy="12" r="10" stroke-width="2"/>
              <line x1="12" y1="8" x2="12" y2="16" stroke-width="2"/>
              <line x1="8" y1="12" x2="16" y2="12" stroke-width="2"/>
            </svg>
            <span>Publicar Sobrante</span>
          </button>

          <button 
            class="nav-tab" 
            :class="{ active: currentView === 'audit' }" 
            @click="navigateTo('audit')"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" class="tab-icon">
              <path d="M11 20A7 7 0 0 1 9.8 6.1C15.5 5 17 4.48 19 2c1 2 2 4.18 2 8 0 5.5-4.78 10-10 10Z" stroke-width="2"/>
              <path d="M2 21c0-3 1.85-5.36 5.08-6C9.5 14.52 12 13 13 12" stroke-width="2"/>
            </svg>
            <span>Trazabilidad & Res. 1257</span>
          </button>

          <button 
            class="nav-tab" 
            :class="{ active: currentView === 'transactions' }" 
            @click="navigateTo('transactions')"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" class="tab-icon">
              <path d="M17 1l4 4-4 4" stroke-width="2"/>
              <path d="M3 11V9a4 4 0 0 1 4-4h14" stroke-width="2"/>
              <path d="M7 23l-4-4 4-4" stroke-width="2"/>
              <path d="M21 13v2a4 4 0 0 1-4 4H3" stroke-width="2"/>
            </svg>
            <span>Motor Transaccional</span>
            <span class="tab-badge badge-orange">{{ transactionsList.length }}</span>
          </button>

          <button 
            class="nav-tab" 
            :class="{ active: currentView === 'subscriptions' }" 
            @click="navigateTo('subscriptions')"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" class="tab-icon">
              <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2" stroke-width="2"/>
            </svg>
            <span>Suscripciones BIM</span>
          </button>
        </nav>

        <!-- Identidad de la Constructora en Sesión -->
        <div class="tenant-badge" @click="navigateTo('subscriptions')">
          <div class="tenant-info">
            <span class="tenant-name">{{ currentCompany.name }}</span>
            <span class="tenant-nit">NIT: {{ currentCompany.nit }}</span>
          </div>
          <span class="subscription-pill">{{ currentCompany.plan }}</span>
        </div>
      </div>
    </header>

    <!-- Notificación Toast Flotante -->
    <transition name="fade">
      <div v-if="notification" class="notification-toast" :class="`toast-${notification.type}`">
        <strong>{{ notification.title }}:</strong> {{ notification.message }}
      </div>
    </transition>

    <!-- Contenedor Dinámico de la Vista Activa -->
    <main class="main-container">
      <CatalogView 
        v-if="currentView === 'catalog'" 
        :on-notify="triggerNotification" 
        @navigate="navigateTo"
      />

      <PublishMaterialView 
        v-else-if="currentView === 'publish'" 
        :on-notify="triggerNotification" 
        @navigate="navigateTo"
      />

      <EnvironmentalAuditView 
        v-else-if="currentView === 'audit'" 
        :on-notify="triggerNotification" 
      />

      <TransactionsView 
        v-else-if="currentView === 'transactions'" 
        :on-notify="triggerNotification" 
      />

      <SubscriptionsView 
        v-else-if="currentView === 'subscriptions'" 
        :on-notify="triggerNotification" 
      />
    </main>

    <!-- Footer Corporativo -->
    <footer class="app-footer">
      <div class="footer-content">
        <p>
          <strong>REARQ BIM © 2026</strong> — Solución de Economía Circular B2B para el Sector de la Construcción en Colombia.
        </p>
        <p class="footer-sub">
          Diseñado para cumplir con el <strong>PGI-RCD</strong> y la <strong>Resolución 1257 de 2021</strong> del Ministerio de Ambiente y Desarrollo Sostenible.
        </p>
      </div>
    </footer>
  </div>
</template>

<style>
/* Reset y Estilos Globales */
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
  background-color: #f8fafc;
  color: #0f172a;
  -webkit-font-smoothing: antialiased;
}

.rearq-app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Header */
.app-header {
  background-color: #0f172a;
  color: #ffffff;
  padding: 10px 24px;
  border-bottom: 2px solid #0284c7;
  position: sticky;
  top: 0;
  z-index: 50;
}

.header-container {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
}

.brand {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
}

.brand-logo {
  width: 38px;
  height: 38px;
  background: linear-gradient(135deg, #0284c7, #0d9488);
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #ffffff;
}

.brand-logo svg {
  width: 22px;
  height: 22px;
}

.brand-title {
  font-size: 1.15rem;
  font-weight: 800;
  letter-spacing: -0.02em;
}

.highlight-cyan {
  color: #38bdf8;
}

.brand-subtitle {
  display: block;
  font-size: 0.68rem;
  color: #94a3b8;
  font-weight: 500;
}

/* Navegación Modular */
.app-nav {
  display: flex;
  gap: 4px;
  align-items: center;
}

.nav-tab {
  background: transparent;
  border: none;
  color: #cbd5e1;
  padding: 8px 14px;
  border-radius: 6px;
  font-size: 0.8rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 7px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.nav-tab:hover {
  background-color: rgba(255, 255, 255, 0.08);
  color: #ffffff;
}

.nav-tab.active {
  background-color: #1e293b;
  color: #38bdf8;
  box-shadow: 0 1px 3px rgba(0,0,0,0.3);
}

.tab-icon {
  width: 16px;
  height: 16px;
}

.tab-badge {
  background-color: #0284c7;
  color: #ffffff;
  font-size: 0.65rem;
  font-weight: 800;
  padding: 1px 6px;
  border-radius: 10px;
}

.badge-orange {
  background-color: #ea580c;
}

/* Tenant Badge */
.tenant-badge {
  display: flex;
  align-items: center;
  gap: 12px;
  background-color: rgba(255, 255, 255, 0.06);
  padding: 6px 12px;
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  cursor: pointer;
  transition: background 0.2s;
}

.tenant-badge:hover {
  background-color: rgba(255, 255, 255, 0.1);
}

.tenant-info {
  display: flex;
  flex-direction: column;
  text-align: right;
}

.tenant-name {
  font-size: 0.78rem;
  font-weight: 700;
}

.tenant-nit {
  font-size: 0.68rem;
  color: #94a3b8;
}

.subscription-pill {
  background-color: #059669;
  color: #ffffff;
  font-size: 0.68rem;
  font-weight: 800;
  padding: 3px 7px;
  border-radius: 4px;
}

/* Main Container */
.main-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 24px;
  flex: 1;
  width: 100%;
}

/* Notificación Toast */
.notification-toast {
  position: fixed;
  bottom: 24px;
  right: 24px;
  padding: 14px 20px;
  border-radius: 8px;
  color: #ffffff;
  font-size: 0.85rem;
  z-index: 1000;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.2);
}

.toast-success { background-color: #059669; }
.toast-info { background-color: #0284c7; }
.toast-error { background-color: #e11d48; }

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

/* Footer */
.app-footer {
  background-color: #ffffff;
  border-top: 1px solid #e2e8f0;
  padding: 18px 24px;
  text-align: center;
  font-size: 0.78rem;
  color: #64748b;
}

.footer-sub {
  font-size: 0.72rem;
  color: #94a3b8;
  margin-top: 4px;
}
</style>
