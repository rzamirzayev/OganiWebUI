using Microsoft.AspNetCore.Mvc;
using Services.Categories;

namespace WebUI.ViewComponents
{
    public class AllCategoriesViewComponent : ViewComponent
    {
        private readonly ICategoryService categoryService;

        public AllCategoriesViewComponent(ICategoryService categoryService) {
            this.categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string view = null)
        {
            var response = await categoryService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(view))
                return View(view, response);

            return View(response);
        }
    }
}
