using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RearqBim.Core.DTOs;
using RearqBim.Core.Interfaces;

namespace RearqBim.Api.Controllers
{
    /// <summary>
    /// Controlador principal para el Catálogo Técnico de Materiales Circulares y Familias BIM
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        private readonly IBimStorageService _bimStorageService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<MaterialsController> _logger;

        public MaterialsController(
            IMaterialService materialService,
            IBimStorageService bimStorageService,
            ISubscriptionService subscriptionService,
            ILogger<MaterialsController> logger)
        {
            _materialService = materialService ?? throw new ArgumentNullException(nameof(materialService));
            _bimStorageService = bimStorageService ?? throw new ArgumentNullException(nameof(bimStorageService));
            _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Publica un nuevo lote de material sobrante desde una obra de origen, incluyendo metadatos técnicos y objeto BIM.
        /// </summary>
        /// <param name="request">Datos técnicos del material, dimensiones milimétricas, fotos y archivo BIM (.rfa, .rvt, .ifc)</param>
        /// <response code="201">Material publicado satisfactoriamente con SKU asignado y ficha técnica disponible.</response>
        /// <response code="400">Atributos técnicos incompletos o archivo BIM con formato no soportado.</response>
        /// <response code="403">La constructora no cuenta con suscripción activa o superó el cupo de proyectos.</response>
        [HttpPost]
        [Authorize(Roles = "PROJECT_RESIDENT,BIM_MODELER,COMPANY_ADMIN")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(MaterialResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PublishMaterial([FromForm] CreateMaterialRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var companyId = GetCurrentCompanyId();

            _logger.LogInformation("Iniciando publicación de material por Usuario {UserId} para Empresa {CompanyId}", userId, companyId);

            // 1. Verificación de reglas de negocio B2B: Validación de suscripción y límites de obra
            var hasActiveSubscription = await _subscriptionService.IsCompanySubscriptionActiveAsync(companyId);
            if (!hasActiveSubscription)
            {
                _logger.LogWarning("Empresa {CompanyId} intentó publicar sin suscripción activa a REARQ BIM.", companyId);
                return StatusCode(StatusCodes.Status403Forbidden, new { 
                    message = "Su empresa requiere una suscripción activa (Plan Pro u Enterprise) para registrar inventario y asociar objetos BIM." 
                });
            }

            // 2. Validación técnica del activo BIM (si fue adjuntado)
            if (request.BimAssetFile != null)
            {
                var allowedBimExtensions = new[] { ".rfa", ".rvt", ".ifc", ".dwg" };
                var extension = Path.GetExtension(request.BimAssetFile.FileName).ToLowerInvariant();

                if (!allowedBimExtensions.Contains(extension))
                {
                    return BadRequest(new { 
                        error = $"El formato de archivo BIM '{extension}' no es válido. Formatos soportados: .RFA (Revit Family), .RVT (Revit Project), .IFC (Industry Foundation Classes)." 
                    });
                }

                const long maxFileSize = 100 * 1024 * 1024; // 100 MB límite
                if (request.BimAssetFile.Length > maxFileSize)
                {
                    return BadRequest(new { error = "El archivo BIM supera el límite máximo permitido de 100MB." });
                }
            }

            // 3. Procesamiento y persistencia a través del servicio de aplicación
            try
            {
                var createdMaterial = await _materialService.CreateMaterialWithAssetsAsync(request, userId, companyId);

                _logger.LogInformation("Material publicado con éxito. ID: {MaterialId}, SKU: {Sku}", createdMaterial.Id, createdMaterial.Sku);

                return CreatedAtAction(nameof(GetMaterialById), new { id = createdMaterial.Id }, createdMaterial);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error de validación al publicar material");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error inesperado al publicar material técnico.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Ocurrió un error interno al procesar el material." });
            }
        }

        /// <summary>
        /// Obtiene la ficha técnica completa de un material por su Identificador Único
        /// </summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MaterialResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMaterialById(Guid id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material == null)
            {
                return NotFound(new { error = $"Material con ID '{id}' no encontrado en el catálogo." });
            }

            return Ok(material);
        }

        /// <summary>
        /// Buscador Técnico Avanzado de Materiales con Filtros Dimensionales y de Ubicación
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<MaterialResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchMaterials(
            [FromQuery] string? query,
            [FromQuery] string? category,
            [FromQuery] string? city,
            [FromQuery] string? condition,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? onlyWithBim,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            var results = await _materialService.SearchMaterialsAsync(new MaterialFilterDto
            {
                SearchTerm = query,
                Category = category,
                City = city,
                Condition = condition,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                OnlyWithBim = onlyWithBim ?? false,
                Page = page,
                PageSize = pageSize
            });

            return Ok(results);
        }

        /// <summary>
        /// Descarga autorizada del objeto BIM (.RFA / .IFC) para ser incorporado en Revit o software de modelado
        /// </summary>
        [HttpGet("{id:guid}/bim-download")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadBimAsset(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            var hasBimAccess = await _subscriptionService.HasBimDownloadAccessAsync(companyId);

            if (!hasBimAccess)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new {
                    message = "La descarga de familias y modelos BIM requiere una suscripción mensual activa (Planes Pro o Enterprise)."
                });
            }

            var downloadResult = await _bimStorageService.GetBimDownloadStreamAsync(id);
            if (downloadResult == null)
            {
                return NotFound(new { message = "El material no posee un modelo BIM asociado disponible para descarga." });
            }

            await _materialService.IncrementBimDownloadCounterAsync(id);

            return File(downloadResult.FileStream, downloadResult.ContentType, downloadResult.FileName);
        }

        #region Métodos Auxiliares de Claims y Contexto B2B
        private Guid GetCurrentUserId()
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claimValue, out var guid) ? guid : Guid.Empty;
        }

        private Guid GetCurrentCompanyId()
        {
            var claimValue = User.FindFirst("CompanyId")?.Value;
            return Guid.TryParse(claimValue, out var guid) ? guid : Guid.Empty;
        }
        #endregion
    }
}
