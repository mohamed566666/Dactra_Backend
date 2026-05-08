using Dactra.DTOs.FavoritesDTOs;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IPatientService _patientService;

        public FavoritesController(IFavoriteService favoriteService, IPatientService patientService)
        {
            _favoriteService = favoriteService;
            _patientService = patientService;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var patientId = await GetPatientIdAsync();
            if (patientId == 0) return Unauthorized();

            await _favoriteService.ToggleFavoriteAsync(patientId, id);
            return Ok("Favorite Toggled Successfuly");
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites([FromQuery] GetFavoritesQueryDTO query)
        {
            var patientId = await GetPatientIdAsync();
            if (patientId == 0) return Unauthorized();

            var result = await _favoriteService.GetFavoritesAsync(patientId, query);
            return Ok(result);
        }

        private async Task<int> GetPatientIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return 0;

            var profile = await _patientService.GetProfileByUserID(userId);
            return profile?.Id ?? 0;
        }
    }
}