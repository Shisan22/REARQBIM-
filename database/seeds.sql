-- ==============================================================================
-- REARQ BIM - Datos Semilla de Prueba (Seed Data)
-- Entorno: Desarrollo / Testing End-to-End
-- ==============================================================================

-- 1. Empresas Constructoras de Prueba
INSERT INTO companies (id, nit, business_name, trade_name, company_type, email, phone, legal_representative, verified_status)
VALUES 
('11111111-1111-1111-1111-111111111111', '900.123.456-7', 'Constructora Bolívar S.A.', 'Constructora Bolívar', 'CONSTRUCTORA', 'proyectos@bolivar.com.co', '+57 (601) 345-6789', 'Carlos Andrés Restrepo', true),
('22222222-2222-2222-2222-222222222222', '901.987.654-3', 'Arquitectura & Concretos Medellín S.A.S.', 'ArqConcretos', 'CONSTRUCTORA', 'compras@arqconcretos.com', '+57 (604) 444-1234', 'Isabella Gómez', true)
ON CONFLICT (nit) DO NOTHING;

-- 2. Suscripciones Activas
-- Constructora Bolívar tiene Plan Pro ($1.5M COP/mes)
INSERT INTO subscriptions (id, company_id, tier, status, price_cop, billing_cycle_months, max_active_projects, current_period_start, current_period_end)
VALUES 
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111', 'PRO_PROJECT', 'ACTIVE', 1500000.00, 1, 3, NOW() - INTERVAL '5 days', NOW() + INTERVAL '25 days'),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '22222222-2222-2222-2222-222222222222', 'ENTERPRISE_CORP', 'ACTIVE', 3000000.00, 1, 10, NOW() - INTERVAL '10 days', NOW() + INTERVAL '20 days')
ON CONFLICT (id) DO NOTHING;

-- 3. Obras de Construcción (Origen y Destino)
INSERT INTO construction_projects (id, company_id, code_license, name, city, department, address, latitude, longitude, pgi_rcd_code, is_active)
VALUES 
('33333333-3333-3333-3333-333333333333', '11111111-1111-1111-1111-111111111111', 'CUR-BOG-2025-104', 'Torre Sostenible Calle 93', 'Bogotá D.C.', 'Cundinamarca', 'Calle 93 # 14-20', 4.6782, -74.0538, 'PGI-RCD-BOG-0421', true),
('44444444-4444-4444-4444-444444444444', '22222222-2222-2222-2222-222222222222', 'CUR-MED-2025-089', 'Complejo Residencial El Poblado', 'Medellín', 'Antioquia', 'Cra 43A # 12Sur-30', 6.2085, -75.5711, 'PGI-RCD-MED-0199', true)
ON CONFLICT (id) DO NOTHING;

-- 4. Usuarios de Prueba (Password genérica hasheada: Password123!)
INSERT INTO users (id, company_id, email, password_hash, first_name, last_name, professional_card_number, role, is_active)
VALUES 
('55555555-5555-5555-5555-555555555555', '11111111-1111-1111-1111-111111111111', 'esteban.quiceno@bolivar.com.co', '$2a$12$eA8bI.H9V/c3q5gN1p5fcegM4sA1qQ.F7J9kY0d2zH4rZ3e1jW7kK', 'Esteban', 'Quiceno', 'COPNIA-251094', 'PROJECT_RESIDENT', true),
('66666666-6666-6666-6666-666666666666', '22222222-2222-2222-2222-222222222222', 'isabella.gomez@arqconcretos.com', '$2a$12$eA8bI.H9V/c3q5gN1p5fcegM4sA1qQ.F7J9kY0d2zH4rZ3e1jW7kK', 'Isabella', 'Gómez', 'CPNAA-A18492', 'BIM_MODELER', true)
ON CONFLICT (email) DO NOTHING;

-- 5. Materiales Técnicos Sobrantes de Obra
INSERT INTO materials (
    id, origin_project_id, published_by_user_id, sku, title, category, condition, technical_description,
    length_mm, width_mm, height_mm, unit_of_measure, weight_kg_per_unit, available_quantity, minimum_order_quantity,
    original_retail_price_cop, unit_price_cop, recycled_content_percentage, embodied_carbon_kg_co2e_per_unit, rcd_normative_code, is_published
)
VALUES 
(
    '77777777-7777-7777-7777-777777777777',
    '33333333-3333-3333-3333-333333333333',
    '55555555-5555-5555-5555-555555555555',
    'MAT-2026-089',
    'Puerta Entamborada en Roble Reclaimed + Marco Estructural',
    'CARPINTERIA_Y_MADERAS',
    'EXCELENTE',
    'Puertas macizas recuperadas en desmonte de obra institucional. Incluye marco en madera estructural y bisagras inox 304. Sin perforaciones de cerradura adicionales.',
    2100.00, 900.00, 45.00, 'UNIDAD', 28.50, 42.00, 5.00,
    450000.00, 220000.00, 85.00, 14.20, 'RCD-MAD-01', true
),
(
    '88888888-8888-8888-8888-888888888888',
    '33333333-3333-3333-3333-333333333333',
    '55555555-5555-5555-5555-555555555555',
    'MAT-2026-112',
    'Bloque Estructural de Arcilla Cocida 12x19x39 - Lote Sobrante',
    'MAMPOSTERIA_Y_BLOQUES',
    'NUEVO_EXCEDENTE',
    'Paletas completas embaladas en plástico termoencogible. Excedente de cerramiento de fachada Torre 2. Resistencia compresión 10.5 MPa según NTC 4205.',
    390.00, 120.00, 190.00, 'UNIDAD', 8.20, 1500.00, 100.00,
    3200.00, 1850.00, 10.00, 1.80, 'RCD-MAMP-02', true
)
ON CONFLICT (sku) DO NOTHING;

-- 6. Archivo BIM Asociado (Familia Revit .RFA y archivo IFC)
INSERT INTO material_bim_assets (id, material_id, file_format, bim_software_version, lod_level, file_url, file_size_bytes, download_count)
VALUES 
('99999999-9999-9999-9999-999999999999', '77777777-7777-7777-7777-777777777777', 'RFA', 'Autodesk Revit 2024 / IFC4', 'LOD_300', 'https://storage.rearqbim.com/bim-assets/MAT-2026-089_Puerta_Roble_LOD300.rfa', 12582912, 14)
ON CONFLICT (id) DO NOTHING;

-- 7. Fotografías de Muestra
INSERT INTO material_media (id, material_id, media_url, is_cover, caption)
VALUES 
(uuid_generate_v4(), '77777777-7777-7777-7777-777777777777', 'https://images.unsplash.com/photo-1513694203232-719a280e022f?auto=format&fit=crop&w=600&q=80', true, 'Detalle de carpintería y acabado en roble')
ON CONFLICT DO NOTHING;
