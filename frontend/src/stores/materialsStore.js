import { ref, computed } from 'vue'

// Estado compartido de la constructora en sesión
export const currentCompany = ref({
  id: '11111111-1111-1111-1111-111111111111',
  name: 'Constructora Bolívar S.A.',
  nit: '900.123.456-7',
  plan: 'PRO PROJECT ($1.5M/mes)',
  planTier: 'PRO_PROJECT',
  hasActiveSubscription: true,
  activeProjects: [
    { id: '33333333-3333-3333-3333-333333333333', name: 'Torre Sostenible Calle 93', city: 'Bogotá D.C.' },
    { id: '55555555-5555-5555-5555-555555555555', name: 'Residencial Reserva de la Colina', city: 'Bogotá D.C.' }
  ]
})

// Catálogo compartido de materiales técnicos
export const materialsList = ref([
  {
    id: '77777777-7777-7777-7777-777777777777',
    sku: 'MAT-2026-089',
    title: 'Puerta Entamborada en Roble Reclaimed + Marco Estructural',
    category: 'CARPINTERIA_Y_MADERAS',
    condition: 'EXCELENTE',
    technicalDescription: 'Puertas macizas recuperadas en desmonte institucional. Incluye marco en madera estructural y bisagras inox 304.',
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
    rcdNormativeCode: 'RCD-MAD-01',
    originProjectName: 'Torre Sostenible Calle 93',
    city: 'Bogotá D.C.',
    imageUrl: 'https://images.unsplash.com/photo-1513694203232-719a280e022f?auto=format&fit=crop&w=600&q=80',
    hasBimAsset: true,
    bimAsset: {
      fileFormat: 'RFA',
      lodLevel: 'LOD 300',
      bimSoftwareVersion: 'Autodesk Revit 2024 / IFC4',
      fileSizeBytes: 12582912,
      downloadCount: 14
    }
  },
  {
    id: '88888888-8888-8888-8888-888888888888',
    sku: 'MAT-2026-112',
    title: 'Bloque Estructural de Arcilla Cocida 12x19x39 - Lote Sobrante',
    category: 'MAMPOSTERIA_Y_BLOQUES',
    condition: 'NUEVO_EXCEDENTE',
    technicalDescription: 'Paletas completas embaladas en plástico termoencogible. Excedente de cerramiento Torre 2. Resistencia 10.5 MPa según NTC 4205.',
    lengthMm: 390,
    widthMm: 120,
    heightMm: 190,
    unitOfMeasure: 'UNIDAD',
    weightKgPerUnit: 8.2,
    availableQuantity: 1500,
    unitPriceCop: 1850,
    originalRetailPriceCop: 3200,
    savingsPercentage: 42.19,
    embodiedCarbonKgCo2ePerUnit: 1.8,
    rcdNormativeCode: 'RCD-MAMP-02',
    originProjectName: 'Complejo Residencial El Poblado',
    city: 'Medellín',
    imageUrl: 'https://images.unsplash.com/photo-1590069261209-f8e9b8642343?auto=format&fit=crop&w=600&q=80',
    hasBimAsset: true,
    bimAsset: {
      fileFormat: 'IFC',
      lodLevel: 'LOD 300',
      bimSoftwareVersion: 'IFC4 OpenBIM',
      fileSizeBytes: 4194304,
      downloadCount: 5
    }
  },
  {
    id: '99999999-9999-9999-9999-999999999999',
    sku: 'MAT-2026-144',
    title: 'Viga IPE 200 Acero Estructural A572 Grado 50 (Tramos 6.0m)',
    category: 'ESTRUCTURAS_Y_CONCRETO',
    condition: 'NUEVO_EXCEDENTE',
    technicalDescription: 'Tramos de 6m sin perforar con imprimación anticorrosiva gris. Certificado de calidad de colada de acería disponible.',
    lengthMm: 6000,
    widthMm: 100,
    heightMm: 200,
    unitOfMeasure: 'UNIDAD',
    weightKgPerUnit: 134.4,
    availableQuantity: 18,
    unitPriceCop: 490000,
    originalRetailPriceCop: 850000,
    savingsPercentage: 42.35,
    embodiedCarbonKgCo2ePerUnit: 180.5,
    rcdNormativeCode: 'RCD-MET-01',
    originProjectName: 'Parque Industrial del Valle',
    city: 'Cali',
    imageUrl: 'https://images.unsplash.com/photo-1504307651254-35680f356dfd?auto=format&fit=crop&w=600&q=80',
    hasBimAsset: true,
    bimAsset: {
      fileFormat: 'RFA',
      lodLevel: 'LOD 350',
      bimSoftwareVersion: 'Autodesk Revit 2024 / IFC4',
      fileSizeBytes: 8388608,
      downloadCount: 22
    }
  }
])

// Historial de Transacciones B2B con Liquidación de Comisión (5% - 8%)
export const transactionsList = ref([
  {
    id: 'tx-001',
    code: 'TX-2026-000142',
    materialTitle: 'Puerta Entamborada en Roble Reclaimed + Marco Estructural',
    originProject: 'Torre Sostenible Calle 93 (Bogotá)',
    buyerProject: 'Complejo Residencial El Poblado (Medellín)',
    quantity: 12,
    unitPriceCop: 220000,
    subtotalMaterialsCop: 2640000,
    commissionPct: 7.0,
    platformFeeCop: 184800,
    totalCop: 2824800,
    status: 'ESCROW_FUNDED', // ESCROW_FUNDED, DISPATCHED, DELIVERED_RECEIVED
    date: '2026-10-18',
    rcdMassDivertedTons: 0.342,
    co2eAvoidedKg: 170.4,
    certificateCode: 'CERT-RCD-2026-9812'
  },
  {
    id: 'tx-002',
    code: 'TX-2026-000098',
    materialTitle: 'Bloque Estructural de Arcilla Cocida 12x19x39',
    originProject: 'Torre Sostenible Calle 93 (Bogotá)',
    buyerProject: 'Residencial Reserva de la Colina (Bogotá)',
    quantity: 600,
    unitPriceCop: 1850,
    subtotalMaterialsCop: 1110000,
    commissionPct: 6.5,
    platformFeeCop: 72150,
    totalCop: 1182150,
    status: 'DELIVERED_RECEIVED',
    date: '2026-10-10',
    rcdMassDivertedTons: 4.92,
    co2eAvoidedKg: 1080.0,
    certificateCode: 'CERT-RCD-2026-8941'
  }
])

// Certificados Ambientales Oficiales (PGI-RCD / Res. 1257 de 2021)
export const certificatesList = ref([
  {
    code: 'CERT-RCD-2026-9812',
    txCode: 'TX-2026-000142',
    project: 'Complejo Residencial El Poblado',
    material: 'Puerta Entamborada en Roble Reclaimed',
    tonsDiverted: 0.342,
    co2eAvoidedKg: 170.4,
    issuedDate: '2026-10-18',
    authority: 'Secretaría Distrital de Ambiente / Corantioquia',
    complianceNorm: 'Resolución 1257 de 2021'
  },
  {
    code: 'CERT-RCD-2026-8941',
    txCode: 'TX-2026-000098',
    project: 'Residencial Reserva de la Colina',
    material: 'Bloque Estructural de Arcilla Cocida',
    tonsDiverted: 4.92,
    co2eAvoidedKg: 1080.0,
    issuedDate: '2026-10-10',
    authority: 'Secretaría Distrital de Ambiente (SDA Bogotá)',
    complianceNorm: 'Resolución 1257 de 2021 / Res. 0472'
  }
])

// Funciones para agregar datos dinámicamente
export function addMaterial(newMat) {
  materialsList.value.unshift(newMat)
}

export function addTransaction(newTx) {
  transactionsList.value.unshift(newTx)
  certificatesList.value.unshift({
    code: newTx.certificateCode,
    txCode: newTx.code,
    project: newTx.buyerProject,
    material: newTx.materialTitle,
    tonsDiverted: newTx.rcdMassDivertedTons,
    co2eAvoidedKg: newTx.co2eAvoidedKg,
    issuedDate: new Date().toISOString().split('T')[0],
    authority: 'CAR / Autoridad Ambiental Regional',
    complianceNorm: 'Resolución 1257 de 2021'
  })
}
