using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RearqBim.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionsController : ControllerBase
    {
        public class CreateTransactionRequest
        {
            public Guid MaterialId { get; set; }
            public Guid BuyerProjectId { get; set; }
            public decimal Quantity { get; set; }
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            var unitPrice = 220000m; // Precio base simulado
            var subtotal = unitPrice * request.Quantity;
            const decimal commissionPct = 7.0m;
            var fee = subtotal * (commissionPct / 100m);
            var total = subtotal + fee;

            var tonsDiverted = (28.5m * request.Quantity) / 1000m;
            var co2Avoided = 14.2m * request.Quantity;

            return Ok(new
            {
                transactionCode = $"TX-2026-{Random.Shared.Next(100000, 999999)}",
                quantity = request.Quantity,
                subtotalMaterialsCop = subtotal,
                commissionPercentage = commissionPct,
                platformFeeCop = fee,
                totalAmountCop = total,
                status = "ESCROW_FUNDED",
                environmentalCertificate = new
                {
                    certificateCode = $"CERT-RCD-2026-{Random.Shared.Next(1000, 9999)}",
                    rcdMassDivertedTons = tonsDiverted,
                    co2eAvoidedKg = co2Avoided,
                    resolutionCompliance = "Resolución 1257 de 2021 / PGI-RCD",
                    verificationUrl = $"https://app.rearqbim.com/verify/CERT-RCD-2026-{Random.Shared.Next(1000, 9999)}"
                }
            });
        }
    }
}
