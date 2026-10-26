-- ==============================================================================
-- REARQ BIM - Sistema de Economía Circular y Marketplace Técnico B2B
-- Esquema de Base de Datos Relacional para PostgreSQL 15+
-- Soporta: Normativa Ambiental Colombiana (Res. 1257/2021, Res. 0472/2017, PGI-RCD),
--          Integración de Fichas Técnicas BIM (IFC, RVT, RFA),
--          Suscripciones SaaS para Constructoras y Motor Transaccional (5% - 8% Fee).
-- ==============================================================================

-- 0. Extensiones requeridas
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- 1. Tipos ENUM del Dominio de Construcción y BIM
CREATE TYPE user_role AS ENUM (
    'SUPER_ADMIN',
    'COMPANY_ADMIN',
    'PROJECT_RESIDENT',      -- Ingeniero residente / Director de obra
    'BIM_MODELER',           -- Arquitecto modelador / Diseñador
    'ENVIRONMENTAL_AUDITOR'  -- Auditor de sostenibilidad / PGI-RCD
);

CREATE TYPE company_type AS ENUM (
    'CONSTRUCTORA',
    'PROMOTORA_INMOBILIARIA',
    'CONTRATISTA_ESPECIALIZADO',
    'DEMOLEDORA',
    'DEPOSITO_RECICLAJE_AUTORIZADO'
);

CREATE TYPE subscription_tier AS ENUM (
    'BASIC_FREE',            -- Visualización básica de catálogo
    'PRO_PROJECT',           -- 1.5M COP/mes: Descargas BIM, 1 Obra activa
    'ENTERPRISE_CORP'        -- 3.0M COP/mes: Multi-obra, API BIM, Certificados PGI-RCD ilimitados
);

CREATE TYPE subscription_status AS ENUM (
    'TRIALING',
    'ACTIVE',
    'PAST_DUE',
    'CANCELED',
    'SUSPENDED'
);

CREATE TYPE material_condition AS ENUM (
    'NUEVO_EXCEDENTE',       -- 100% óptimo, empaque original o remanente sin instalar
    'EXCELENTE',             -- Recuperado intacto, sin defectos estructurales
    'BUENO_REUTILIZABLE',    -- Defectos estéticos menores, óptimo para uso secundario
    'APTO_RECICLAJE'         -- Para trituración/transformación (bloque, concreto, mampostería)
);

CREATE TYPE material_category AS ENUM (
    'ESTRUCTURAS_Y_CONCRETO',
    'MAMPOSTERIA_Y_BLOQUES',
    'ACABADOS_Y_CERAMICA',
    'CARPINTERIA_Y_MADERAS',
    'HIDROSANITARIO_Y_GRIFERIA',
    'ELECTRICO_E_ILUMINACION',
    'CUBIERTAS_Y_AISLAMIENTOS',
    'CARPINTERIA_METALICA_Y_VIDRIOS',
    'EXCAVACION_Y_ARIDOS'
);

CREATE TYPE bim_lod_level AS ENUM (
    'LOD_100',
    'LOD_200',
    'LOD_300',               -- Detalle geométrico y dimensional exacto
    'LOD_350',               -- Detalle de conexiones y fijaciones
    'LOD_400'
);

CREATE TYPE transaction_status AS ENUM (
    'DRAFT',
    'RESERVATION_PENDING',
    'ESCROW_FUNDED',
    'SITE_INSPECTION_APPROVED',
    'DISPATCHED',
    'DELIVERED_RECEIVED',
    'COMPLETED',
    'DISPUTED',
    'CANCELED'
);

-- 2. Entidad: Empresas / Constructoras (Tenants B2B)
CREATE TABLE companies (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    nit VARCHAR(20) NOT NULL UNIQUE,
    business_name VARCHAR(255) NOT NULL,
    trade_name VARCHAR(255),
    company_type company_type NOT NULL DEFAULT 'CONSTRUCTORA',
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(50),
    legal_representative VARCHAR(255),
    verified_status BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_companies_nit ON companies(nit);

-- 3. Entidad: Suscripciones SaaS (Acceso a BIM y Trazabilidad Ambiental)
CREATE TABLE subscriptions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    company_id UUID NOT NULL REFERENCES companies(id) ON DELETE CASCADE,
    tier subscription_tier NOT NULL DEFAULT 'BASIC_FREE',
    status subscription_status NOT NULL DEFAULT 'ACTIVE',
    price_cop NUMERIC(14, 2) NOT NULL DEFAULT 0.00,  -- Ej. 1,500,000.00 o 3,000,000.00 COP
    billing_cycle_months INT NOT NULL DEFAULT 1,
    max_active_projects INT NOT NULL DEFAULT 1,
    current_period_start TIMESTAMP WITH TIME ZONE NOT NULL,
    current_period_end TIMESTAMP WITH TIME ZONE NOT NULL,
    cancel_at_period_end BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_subscriptions_company ON subscriptions(company_id);
CREATE INDEX idx_subscriptions_status ON subscriptions(status);

-- 4. Entidad: Obras / Proyectos de Construcción (Origen o Destino de Materiales)
CREATE TABLE construction_projects (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    company_id UUID NOT NULL REFERENCES companies(id) ON DELETE RESTRICT,
    code_license VARCHAR(100),                      -- Curaduría / Licencia de construcción
    name VARCHAR(255) NOT NULL,
    city VARCHAR(100) NOT NULL,                     -- Bogotá, Medellín, Cali, etc.
    department VARCHAR(100) NOT NULL,
    address TEXT NOT NULL,
    latitude DECIMAL(10, 8),
    longitude DECIMAL(11, 8),
    estimated_end_date DATE,
    pgi_rcd_code VARCHAR(100),                      -- Código de Plan de Gestión Integral RCD
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_projects_city ON construction_projects(city);
CREATE INDEX idx_projects_company ON construction_projects(company_id);

-- 5. Entidad: Usuarios
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    company_id UUID REFERENCES companies(id) ON DELETE SET NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    professional_card_number VARCHAR(100),          -- Matrícula profesional COPNIA/CPNAA
    phone VARCHAR(50),
    role user_role NOT NULL DEFAULT 'PROJECT_RESIDENT',
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    last_login_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);

-- 6. Entidad: Catálogo de Materiales Técnicos (Sobrantes de Obra)
CREATE TABLE materials (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    origin_project_id UUID NOT NULL REFERENCES construction_projects(id) ON DELETE RESTRICT,
    published_by_user_id UUID NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    
    sku VARCHAR(50) UNIQUE,
    title VARCHAR(255) NOT NULL,
    category material_category NOT NULL,
    condition material_condition NOT NULL,
    technical_description TEXT NOT NULL,
    
    -- Métricas Dimensionales Técnicas
    length_mm DECIMAL(10, 2),                       -- Largo en milímetros (estándar BIM)
    width_mm DECIMAL(10, 2),                        -- Ancho en milímetros
    height_mm DECIMAL(10, 2),                       -- Alto / Espesor en milímetros
    unit_of_measure VARCHAR(30) NOT NULL DEFAULT 'UNIDAD', -- m2, m3, ml, UNIDAD, kg
    weight_kg_per_unit DECIMAL(10, 2),              -- Clave para cálculo RCD (Toneladas)
    
    -- Inventario y Disponibilidad
    available_quantity DECIMAL(12, 2) NOT NULL,
    reserved_quantity DECIMAL(12, 2) NOT NULL DEFAULT 0.00,
    minimum_order_quantity DECIMAL(12, 2) DEFAULT 1.00,
    
    -- Precios (Moneda: COP)
    original_retail_price_cop NUMERIC(14, 2),      -- Precio comercial nuevo de referencia
    unit_price_cop NUMERIC(14, 2) NOT NULL,        -- Precio de venta en REARQ
    savings_percentage DECIMAL(5, 2) GENERATED ALWAYS AS (
        CASE WHEN original_retail_price_cop > 0 AND original_retail_price_cop > unit_price_cop
        THEN ROUND(((original_retail_price_cop - unit_price_cop) / original_retail_price_cop) * 100, 2)
        ELSE 0.00 END
    ) STORED,

    -- Indicadores de Sostenibilidad y Economía Circular
    recycled_content_percentage DECIMAL(5, 2) DEFAULT 0.00,
    embodied_carbon_kg_co2e_per_unit DECIMAL(10, 2), -- Huella de carbono incorporada evitada
    rcd_normative_code VARCHAR(50),                 -- Código según Res. 0472 / 1257
    
    -- Estado de la publicación
    is_published BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_materials_category ON materials(category);
CREATE INDEX idx_materials_condition ON materials(condition);
CREATE INDEX idx_materials_origin_project ON materials(origin_project_id);
CREATE INDEX idx_materials_price ON materials(unit_price_cop);

-- 7. Entidad: Archivos e Integración BIM (Revit, IFC, CAD)
CREATE TABLE material_bim_assets (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    material_id UUID NOT NULL REFERENCES materials(id) ON DELETE CASCADE,
    file_format VARCHAR(20) NOT NULL,               -- 'RFA', 'RVT', 'IFC', 'DWG'
    bim_software_version VARCHAR(50),               -- Ej. 'Revit 2024', 'IFC4', 'IFC2x3'
    lod_level bim_lod_level NOT NULL DEFAULT 'LOD_300',
    file_url VARCHAR(500) NOT NULL,
    file_size_bytes BIGINT NOT NULL,
    checksum_sha256 VARCHAR(64),
    parameters_json JSONB,                          -- Metadatos BIM expuestos: OmniClass, UniFormat, transmitancia térmica, etc.
    download_count INT NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_bim_assets_material ON material_bim_assets(material_id);
CREATE INDEX idx_bim_assets_format ON material_bim_assets(file_format);

-- 8. Entidad: Fotografías y Evidencias Técnicas
CREATE TABLE material_media (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    material_id UUID NOT NULL REFERENCES materials(id) ON DELETE CASCADE,
    media_url VARCHAR(500) NOT NULL,
    is_cover BOOLEAN NOT NULL DEFAULT FALSE,
    caption VARCHAR(255),
    display_order INT DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_material_media_material ON material_media(material_id);

-- 9. Entidad: Motor Transaccional B2B (Compras, Coordinación y Comisión 5%-8%)
CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    transaction_code VARCHAR(30) UNIQUE NOT NULL,    -- Ej. 'TX-2026-000142'
    material_id UUID NOT NULL REFERENCES materials(id) ON DELETE RESTRICT,
    buyer_project_id UUID NOT NULL REFERENCES construction_projects(id) ON DELETE RESTRICT,
    buyer_user_id UUID NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    
    quantity DECIMAL(12, 2) NOT NULL,
    subtotal_materials_cop NUMERIC(14, 2) NOT NULL,
    
    -- Comisión de la plataforma REARQ BIM (5% al 8%)
    commission_percentage DECIMAL(5, 2) NOT NULL DEFAULT 7.00,
    platform_fee_cop NUMERIC(14, 2) NOT NULL,
    logistics_fee_cop NUMERIC(14, 2) DEFAULT 0.00,
    total_amount_cop NUMERIC(14, 2) NOT NULL,
    
    status transaction_status NOT NULL DEFAULT 'DRAFT',
    dispatch_date TIMESTAMP WITH TIME ZONE,
    reception_date TIMESTAMP WITH TIME ZONE,
    
    inspection_notes TEXT,
    buyer_digital_signature VARCHAR(255),
    seller_digital_signature VARCHAR(255),
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_transactions_material ON transactions(material_id);
CREATE INDEX idx_transactions_status ON transactions(status);
CREATE INDEX idx_transactions_buyer_project ON transactions(buyer_project_id);

-- 10. Entidad: Certificados de Trazabilidad y Cumplimiento Ambiental (PGI-RCD / Res. 1257)
CREATE TABLE environmental_certificates (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    certificate_code VARCHAR(50) UNIQUE NOT NULL,   -- Ej. 'CERT-RCD-2026-9812'
    transaction_id UUID NOT NULL REFERENCES transactions(id) ON DELETE RESTRICT,
    origin_project_id UUID NOT NULL REFERENCES construction_projects(id),
    destination_project_id UUID NOT NULL REFERENCES construction_projects(id),
    
    rcd_mass_diverted_tons DECIMAL(10, 3) NOT NULL, -- Toneladas métricas desviadas del vertedero
    co2e_avoided_kg DECIMAL(12, 2) NOT NULL,        -- Huella de carbono evitada
    resolution_compliance VARCHAR(100) DEFAULT 'Resolución 1257 de 2021 / PGI-RCD',
    
    qr_verification_url VARCHAR(500),
    pdf_report_url VARCHAR(500),
    issued_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_env_certificates_code ON environmental_certificates(certificate_code);
CREATE INDEX idx_env_certificates_transaction ON environmental_certificates(transaction_id);
