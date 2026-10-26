using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RearqBim.Core.DTOs
{
    /// <summary>
    /// DTO para la publicación técnica de un material sobrante de obra
    /// </summary>
    public class CreateMaterialRequestDto
    {
        [Required(ErrorMessage = "El ID del proyecto de origen es obligatorio.")]
        public Guid OriginProjectId { get; set; }

        [Required(ErrorMessage = "El título o denominación técnica del material es requerido.")]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría de material es requerida.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado técnico de conservación es requerido.")]
        public string Condition { get; set; } = string.Empty; // NUEVO_EXCEDENTE, EXCELENTE, BUENO_REUTILIZABLE, APTO_RECICLAJE

        [Required(ErrorMessage = "La descripción técnica y especificaciones son obligatorias.")]
        public string TechnicalDescription { get; set; } = string.Empty;

        // Atributos Dimensionales Técnicos Exactos (en mm para interoperabilidad BIM)
        [Range(0.1, 100000.0, ErrorMessage = "Largo inválido.")]
        public decimal? LengthMm { get; set; }

        [Range(0.1, 100000.0, ErrorMessage = "Ancho inválido.")]
        public decimal? WidthMm { get; set; }

        [Range(0.1, 100000.0, ErrorMessage = "Alto/Espesor inválido.")]
        public decimal? HeightMm { get; set; }

        [Required]
        public string UnitOfMeasure { get; set; } = "UNIDAD"; // UNIDAD, m2, m3, ml, kg

        [Range(0.01, 1000000.0, ErrorMessage = "Peso unitario necesario para cálculo de masa RCD (Res. 1257).")]
        public decimal WeightKgPerUnit { get; set; }

        // Inventario
        [Range(0.01, 1000000.0, ErrorMessage = "Cantidad disponible debe ser mayor a 0.")]
        public decimal AvailableQuantity { get; set; }

        public decimal MinimumOrderQuantity { get; set; } = 1.0m;

        // Precios (COP)
        public decimal? OriginalRetailPriceCop { get; set; }

        [Required]
        [Range(100.0, 1000000000.0, ErrorMessage = "Precio de oferta en COP requerido.")]
        public decimal UnitPriceCop { get; set; }

        // Atributos de Sostenibilidad y Trazabilidad Ambiental
        public decimal RecycledContentPercentage { get; set; } = 0.0m;
        public decimal? EmbodiedCarbonKgCo2ePerUnit { get; set; }
        public string? RcdNormativeCode { get; set; }

        // Archivos adjuntos
        public List<IFormFile> Photographs { get; set; } = new();
        public IFormFile? BimAssetFile { get; set; } // Archivo .RFA, .RVT o .IFC
        public string? BimLodLevel { get; set; } = "LOD_300";
        public string? BimSoftwareVersion { get; set; } = "Revit 2024 / IFC4";
    }

    /// <summary>
    /// DTO de respuesta para fichas técnicas de materiales en catálogo
    /// </summary>
    public class MaterialResponseDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string TechnicalDescription { get; set; } = string.Empty;
        
        // Dimensiones
        public decimal? LengthMm { get; set; }
        public decimal? WidthMm { get; set; }
        public decimal? HeightMm { get; set; }
        public string DimensionsFormatted => $"{LengthMm ?? 0} x {WidthMm ?? 0} x {HeightMm ?? 0} mm";
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal WeightKgPerUnit { get; set; }

        // Cantidad y Precio
        public decimal AvailableQuantity { get; set; }
        public decimal UnitPriceCop { get; set; }
        public decimal? OriginalRetailPriceCop { get; set; }
        public decimal SavingsPercentage { get; set; }

        // Sostenibilidad
        public decimal? EmbodiedCarbonKgCo2ePerUnit { get; set; }
        public decimal PotentialRcdDivertedTons => (WeightKgPerUnit * AvailableQuantity) / 1000.0m;

        // Ubicación de la Obra de Origen
        public string OriginProjectName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        // Archivo BIM
        public bool HasBimAsset { get; set; }
        public BimAssetSummaryDto? BimAsset { get; set; }
        public List<string> ImageUrls { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }

    public class BimAssetSummaryDto
    {
        public Guid Id { get; set; }
        public string FileFormat { get; set; } = string.Empty; // RFA, RVT, IFC
        public string BimSoftwareVersion { get; set; } = string.Empty;
        public string LodLevel { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public int DownloadCount { get; set; }
    }
}
