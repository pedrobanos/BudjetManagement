using BudjetManagement.Models;
using BudjetManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudjetManagement.Controllers
{
    public class CategoriesController: Controller
    {
        private readonly ICategoriesRepo categoriesRepo;
        private readonly IUserService userService;

        public CategoriesController(ICategoriesRepo categoriesRepo, IUserService userService)
        {
            this.categoriesRepo = categoriesRepo;
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userService.ObtainUserId();
            var categories = await categoriesRepo.ListItemsCategory(userId);
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if(!ModelState.IsValid)
            {
                return View(category);
            }

            var userId = userService.ObtainUserId();
            category.UserId = userId;
            await categoriesRepo.Create(category);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Update(int id) 
        {
            var userId = userService.ObtainUserId();
            var category = await categoriesRepo.ObtainById(id, userId);

            if (category is null) 
            {
                return RedirectToAction("NotFoundPage", "Index");
            }
            return View(category);
        }

        [HttpPost]

        public async Task<IActionResult> Update (Category categoryEdit)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryEdit);
            }
            var userId = userService.ObtainUserId();
            var category = await categoriesRepo.ObtainById(categoryEdit.Id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }

            categoryEdit.UserId= userId;

            await categoriesRepo.Update(categoryEdit);
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.ObtainUserId();
            var category = await categoriesRepo.ObtainById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFoundPage", "Index");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = userService.ObtainUserId();
            var typeAccountExist = await categoriesRepo.ObtainById(id, userId);
            if (typeAccountExist is null)
            {
                return RedirectToAction("NotFoundPage", "TypesAccounts");
            }
            await categoriesRepo.Delete(id);

            return RedirectToAction("Index");
        }

    }
}
