using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RearqBim.Core.DTOs;

namespace RearqBim.Core.Interfaces
{
    public interface IMaterialService
    {
        Task<MaterialResponseDto> CreateMaterialWithAssetsAsync(CreateMaterialRequestDto request, Guid userId, Guid companyId);
        Task<MaterialResponseDto?> GetMaterialByIdAsync(Guid id);
        Task<IEnumerable<MaterialResponseDto>> SearchMaterialsAsync(MaterialFilterDto filter);
        Task IncrementBimDownloadCounterAsync(Guid materialId);
    }

    public interface IBimStorageService
    {
        Task<BimDownloadResult?> GetBimDownloadStreamAsync(Guid materialId);
        Task<string> UploadBimAssetAsync(Stream stream, string fileName, string contentType);
    }

    public interface ISubscriptionService
    {
        Task<bool> IsCompanySubscriptionActiveAsync(Guid companyId);
        Task<bool> HasBimDownloadAccessAsync(Guid companyId);
    }

    public class MaterialFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? City { get; set; }
        public string? Condition { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool OnlyWithBim { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class BimDownloadResult
    {
        public Stream FileStream { get; set; } = Stream.Null;
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = string.Empty;
    }
}
