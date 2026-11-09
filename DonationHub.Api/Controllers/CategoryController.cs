using DonationHub.Application.Features.Categories.CreateCategory;
using DonationHub.Application.Features.Categories.DeleteCategory;
using DonationHub.Application.Features.Categories.GetAllCategories;
using DonationHub.Application.Features.Categories.GetCategoryById;
using DonationHub.Application.Features.Categories.UpdateCategory;
using DonationHub.Application.Features.Categories.SetCategoryDescription;
using DonationHub.Application.Features.Categories.RemoveCategoryDescription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly GetAllCategoriesService _getAllCategoriesService;
        private readonly GetCategoryByIdService _getCategoryByIdService;
        private readonly CreateCategoryService _createCategoryService;
        private readonly UpdateCategoryService _updateCategoryService;
        private readonly DeleteCategoryService _deleteCategoryService;
        private readonly SetCategoryDescriptionService _setCategoryDescriptionService;
        private readonly RemoveCategoryDescriptionService _removeCategoryDescriptionService;

        public CategoryController(
            GetAllCategoriesService getAllCategoriesService,
            GetCategoryByIdService getCategoryByIdService,
            CreateCategoryService createCategoryService,
            UpdateCategoryService updateCategoryService,
            DeleteCategoryService deleteCategoryService,
            SetCategoryDescriptionService setCategoryDescriptionService,
            RemoveCategoryDescriptionService removeCategoryDescriptionService)
        {
            _getAllCategoriesService = getAllCategoriesService;
            _getCategoryByIdService = getCategoryByIdService;
            _createCategoryService = createCategoryService;
            _updateCategoryService = updateCategoryService;
            _deleteCategoryService = deleteCategoryService;
            _setCategoryDescriptionService = setCategoryDescriptionService;
            _removeCategoryDescriptionService = removeCategoryDescriptionService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyList<GetAllCategoriesResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var categories = await _getAllCategoriesService.GetAllAsync(cancellationToken);
            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<GetAllCategoriesResponse>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var request = new GetCategoryByIdRequest
            {
                Id = id
            };
            var category = await _getCategoryByIdService.GetByIdAsync(
                request,
                cancellationToken);
            if (category is null)
                return NotFound(new { Error = "Category.NotFound" });
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCategoryRequest request,CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.NameAr) ||
                string.IsNullOrWhiteSpace(request.NameEn) ||
                request.NameAr.Trim().Length > 100 ||
                request.NameEn.Trim().Length > 100)
            {
                return BadRequest(new { Error = "Category.InvalidData" });
            }
            var errorCode = await _createCategoryService.CreateAsync(
                request,
                cancellationToken);
            if (errorCode == "Category.NameArAlreadyExists" ||
                errorCode == "Category.NameEnAlreadyExists")
            {
                return Conflict(new { Error = errorCode });
            }
            if (errorCode is not null)
                return BadRequest(new { Error = errorCode });
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id,UpdateCategoryRequest request,CancellationToken cancellationToken)
        {
            if (request.NameAr is not null &&
                (string.IsNullOrWhiteSpace(request.NameAr) ||
                 request.NameAr.Trim().Length > 100))
            {
                return BadRequest(new { Error = "Category.InvalidNameAr" });
            }
            if (request.NameEn is not null &&
                (string.IsNullOrWhiteSpace(request.NameEn) ||
                 request.NameEn.Trim().Length > 100))
            {
                return BadRequest(new { Error = "Category.InvalidNameEn" });
            }
            var errorCode = await _updateCategoryService.UpdateAsync(id,request,cancellationToken);
            if (errorCode == "Category.NotFound")
                return NotFound(new { Error = errorCode });
            if (errorCode == "Category.NameArAlreadyExists" ||
                errorCode == "Category.NameEnAlreadyExists")
            {
                return Conflict(new { Error = errorCode });
            }
            if (errorCode == "Category.NoChanges")
                return BadRequest(new { Error = errorCode });
            if (errorCode is not null)
                return BadRequest(new { Error = errorCode });
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
        {
            var errorCode = await _deleteCategoryService.DeleteAsync(id,cancellationToken);
            if (errorCode == "Category.NotFound")
                return NotFound(new { Error = errorCode });
            if (errorCode == "Category.HasCampaigns")
                return Conflict(new { Error = errorCode });
            if (errorCode is not null)
                return BadRequest(new { Error = errorCode });
            return NoContent();
        }

        [HttpPut("{id:guid}/description")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetDescription(
            Guid id,
            SetCategoryDescriptionRequest request,
            CancellationToken cancellationToken)
        {
            var errorCode = await _setCategoryDescriptionService.SetAsync(
                id,
                request,
                cancellationToken);
            if (errorCode == "Category.NotFound")
                return NotFound(new { Error = errorCode });
            if (errorCode == "Category.InvalidLanguage")
                return BadRequest(new { Error = errorCode });
            if (errorCode is not null)
                return BadRequest(new { Error = errorCode });
            return NoContent();
        }

        [HttpDelete("{id:guid}/description")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveDescription(
            Guid id,
            RemoveCategoryDescriptionRequest request,
            CancellationToken cancellationToken)
        {
            var errorCode = await _removeCategoryDescriptionService.RemoveAsync(
                id,
                request,
                cancellationToken);
            if (errorCode == "Category.NotFound")
                return NotFound(new { Error = errorCode });
            if (errorCode == "Category.InvalidLanguage")
                return BadRequest(new { Error = errorCode });
            if (errorCode is not null)
                return BadRequest(new { Error = errorCode });
            return NoContent();
        }

    }
}
