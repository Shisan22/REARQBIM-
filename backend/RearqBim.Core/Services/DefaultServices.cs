using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RearqBim.Core.DTOs;
using RearqBim.Core.Interfaces;

namespace RearqBim.Core.Services
{
    public class MockSubscriptionService : ISubscriptionService
    {
        // Por defecto, las constructoras de prueba tienen suscripción activa
        public Task<bool> IsCompanySubscriptionActiveAsync(Guid companyId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> HasBimDownloadAccessAsync(Guid companyId)
        {
            return Task.FromResult(true);
        }
    }

    public class MockBimStorageService : IBimStorageService
    {
        public Task<BimDownloadResult?> GetBimDownloadStreamAsync(Guid materialId)
        {
            var sampleContent = Encoding.UTF8.GetBytes(
                $"# REARQ BIM - Revit Family / IFC Metadata Stub\n" +
                $"# Material ID: {materialId}\n" +
                $"# Standard: IFC4 / LOD 300\n" +
                $"# Compliance: Resolucion 1257 de 2021 (MinAmbiente Colombia)\n"
            );

            var result = new BimDownloadResult
            {
                FileStream = new MemoryStream(sampleContent),
                ContentType = "application/octet-stream",
                FileName = $"REARQ_BIM_{materialId:N}_LOD300.rfa"
            };

            return Task.FromResult<BimDownloadResult?>(result);
        }

        public Task<string> UploadBimAssetAsync(Stream stream, string fileName, string contentType)
        {
            var simulatedUrl = $"https://storage.rearqbim.com/bim-assets/{Guid.NewGuid()}_{fileName}";
            return Task.FromResult(simulatedUrl);
        }
    }

    public class MockMaterialService : IMaterialService
    {
        private static readonly ConcurrentDictionary<Guid, MaterialResponseDto> _materials = new();
        private static int _consecutive = 115;

        static MockMaterialService()
        {
            // Seed 1: Puerta en Roble
            var id1 = Guid.Parse("77777777-7777-7777-7777-777777777777");
            _materials[id1] = new MaterialResponseDto
            {
                Id = id1,
                Sku = "MAT-2026-089",
                Title = "Puerta Entamborada en Roble Reclaimed + Marco Estructural",
                Category = "CARPINTERIA_Y_MADERAS",
                Condition = "EXCELENTE",
                TechnicalDescription = "Puertas macizas recuperadas en desmonte de obra institucional. Incluye marco y bisagras inox 304.",
                LengthMm = 2100,
                WidthMm = 900,
                HeightMm = 45,
                UnitOfMeasure = "UNIDAD",
                WeightKgPerUnit = 28.5m,
                AvailableQuantity = 42,
                UnitPriceCop = 220000,
                OriginalRetailPriceCop = 450000,
                SavingsPercentage = 51.11m,
                EmbodiedCarbonKgCo2ePerUnit = 14.2m,
                OriginProjectName = "Torre Sostenible Calle 93",
                City = "Bogotá D.C.",
                Department = "Cundinamarca",
                HasBimAsset = true,
                BimAsset = new BimAssetSummaryDto
                {
                    Id = Guid.NewGuid(),
                    FileFormat = "RFA",
                    BimSoftwareVersion = "Autodesk Revit 2024 / IFC4",
                    LodLevel = "LOD_300",
                    FileSizeBytes = 12582912,
                    DownloadCount = 14,
                    DownloadUrl = $"/api/v1/materials/{id1}/bim-download"
                },
                ImageUrls = new List<string> { "https://images.unsplash.com/photo-1513694203232-719a280e022f?auto=format&fit=crop&w=600&q=80" },
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            };

            // Seed 2: Bloques Estructurales
            var id2 = Guid.Parse("88888888-8888-8888-8888-888888888888");
            _materials[id2] = new MaterialResponseDto
            {
                Id = id2,
                Sku = "MAT-2026-112",
                Title = "Bloque Estructural de Arcilla Cocida 12x19x39 - Lote Sobrante",
                Category = "MAMPOSTERIA_Y_BLOQUES",
                Condition = "NUEVO_EXCEDENTE",
                TechnicalDescription = "Paletas completas embaladas en plástico termoencogible. Excedente de cerramiento de fachada Torre 2.",
                LengthMm = 390,
                WidthMm = 120,
                HeightMm = 190,
                UnitOfMeasure = "UNIDAD",
                WeightKgPerUnit = 8.2m,
                AvailableQuantity = 1500,
                UnitPriceCop = 1850,
                OriginalRetailPriceCop = 3200,
                SavingsPercentage = 42.19m,
                EmbodiedCarbonKgCo2ePerUnit = 1.8m,
                OriginProjectName = "Torre Sostenible Calle 93",
                City = "Bogotá D.C.",
                Department = "Cundinamarca",
                HasBimAsset = true,
                BimAsset = new BimAssetSummaryDto
                {
                    Id = Guid.NewGuid(),
                    FileFormat = "IFC",
                    BimSoftwareVersion = "IFC4",
                    LodLevel = "LOD_300",
                    FileSizeBytes = 4194304,
                    DownloadCount = 5,
                    DownloadUrl = $"/api/v1/materials/{id2}/bim-download"
                },
                ImageUrls = new List<string> { "https://images.unsplash.com/photo-1590069261209-f8e9b8642343?auto=format&fit=crop&w=600&q=80" },
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };
        }

        public Task<MaterialResponseDto> CreateMaterialWithAssetsAsync(CreateMaterialRequestDto request, Guid userId, Guid companyId)
        {
            var id = Guid.NewGuid();
            var sku = $"MAT-2026-{++_consecutive}";

            decimal savings = 0;
            if (request.OriginalRetailPriceCop.HasValue && request.OriginalRetailPriceCop.Value > request.UnitPriceCop)
            {
                savings = Math.Round(((request.OriginalRetailPriceCop.Value - request.UnitPriceCop) / request.OriginalRetailPriceCop.Value) * 100, 2);
            }

            var hasBim = request.BimAssetFile != null;
            BimAssetSummaryDto? bimAsset = null;

            if (hasBim)
            {
                var ext = Path.GetExtension(request.BimAssetFile!.FileName).TrimStart('.').ToUpperInvariant();
                bimAsset = new BimAssetSummaryDto
                {
                    Id = Guid.NewGuid(),
                    FileFormat = ext,
                    BimSoftwareVersion = request.BimSoftwareVersion ?? "Revit 2024 / IFC4",
                    LodLevel = request.BimLodLevel ?? "LOD_300",
                    FileSizeBytes = request.BimAssetFile.Length,
                    DownloadCount = 0,
                    DownloadUrl = $"/api/v1/materials/{id}/bim-download"
                };
            }

            var material = new MaterialResponseDto
            {
                Id = id,
                Sku = sku,
                Title = request.Title,
                Category = request.Category,
                Condition = request.Condition,
                TechnicalDescription = request.TechnicalDescription,
                LengthMm = request.LengthMm,
                WidthMm = request.WidthMm,
                HeightMm = request.HeightMm,
                UnitOfMeasure = request.UnitOfMeasure,
                WeightKgPerUnit = request.WeightKgPerUnit,
                AvailableQuantity = request.AvailableQuantity,
                UnitPriceCop = request.UnitPriceCop,
                OriginalRetailPriceCop = request.OriginalRetailPriceCop,
                SavingsPercentage = savings,
                EmbodiedCarbonKgCo2ePerUnit = request.EmbodiedCarbonKgCo2ePerUnit ?? 0,
                OriginProjectName = "Obra Registrada (" + request.OriginProjectId.ToString()[..8] + ")",
                City = "Bogotá D.C.",
                Department = "Cundinamarca",
                HasBimAsset = hasBim,
                BimAsset = bimAsset,
                ImageUrls = new List<string> { "https://images.unsplash.com/photo-1504307651254-35680f356dfd?auto=format&fit=crop&w=600&q=80" },
                CreatedAt = DateTime.UtcNow
            };

            _materials[id] = material;
            return Task.FromResult(material);
        }

        public Task<MaterialResponseDto?> GetMaterialByIdAsync(Guid id)
        {
            _materials.TryGetValue(id, out var material);
            return Task.FromResult(material);
        }

        public Task<IEnumerable<MaterialResponseDto>> SearchMaterialsAsync(MaterialFilterDto filter)
        {
            IEnumerable<MaterialResponseDto> query = _materials.Values;

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(m => m.Title.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         m.TechnicalDescription.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                query = query.Where(m => m.Category.Equals(filter.Category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.City))
            {
                query = query.Where(m => m.City.Contains(filter.City, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Condition))
            {
                query = query.Where(m => m.Condition.Equals(filter.Condition, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.OnlyWithBim)
            {
                query = query.Where(m => m.HasBimAsset);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(m => m.UnitPriceCop >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(m => m.UnitPriceCop <= filter.MaxPrice.Value);
            }

            var results = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return Task.FromResult<IEnumerable<MaterialResponseDto>>(results);
        }

        public Task IncrementBimDownloadCounterAsync(Guid materialId)
        {
            if (_materials.TryGetValue(materialId, out var material) && material.BimAsset != null)
            {
                material.BimAsset.DownloadCount++;
            }
            return Task.CompletedTask;
        }
    }
}
