using BudjetManagement.Models;
using BudjetManagement.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace BudjetManagement.Controllers
{
    public class TypesAccountsController : Controller
    {
        private readonly ITypesAccountsRepo typesAccountsRepo;
        private readonly IUserService userService;

        public TypesAccountsController(ITypesAccountsRepo typesAccountsRepo, IUserService userService)
        {
            this.typesAccountsRepo = typesAccountsRepo;
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userService.ObtainUserId();
            var typesAccounts = await typesAccountsRepo.ListItemsAccount(userId);
            return View(typesAccounts);
        }
        public IActionResult Create() {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TypeAccount typeAccount)
        {
            if (!ModelState.IsValid)
            {
                return View(typeAccount);
            }

            typeAccount.UserId = userService.ObtainUserId();

            var alreadyExistTypeAccount =
                await typesAccountsRepo.Exist(typeAccount.Name, typeAccount.UserId);

            if (alreadyExistTypeAccount)
            {
                ModelState.AddModelError(nameof(typeAccount.Name), $"The name {typeAccount.Name} does already exist.");
                return View(typeAccount);
            }
            await typesAccountsRepo.Create(typeAccount);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var userId = userService.ObtainUserId();
            var typeAccount = await typesAccountsRepo.ObtainById(id, userId);
            if (typeAccount is null)
            {
                return RedirectToAction("NotFoundPage", "TypesAccounts");
            }
            return View(typeAccount);
        }

        [HttpPost]
        public async Task<IActionResult> Update(TypeAccount typeAccount)
        {
            var userId = userService.ObtainUserId();
            var typeAccountExist = await typesAccountsRepo.ObtainById(typeAccount.Id, userId);
            if(typeAccountExist is null)
            {
                return RedirectToAction("NotFoundPage", "TypesAccounts");
            }
            await typesAccountsRepo.Update(typeAccount);
            return RedirectToAction("Index");

        }

        public IActionResult NotFoundPage()
        {
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.ObtainUserId();
            var typeAccount = await typesAccountsRepo.ObtainById(id, userId);
            if (typeAccount is null)
            {
                return RedirectToAction("NotFoundPage", "TypesAccounts");
            }

            return View(typeAccount);
        }

        [HttpPost]

        public async Task<IActionResult> DeleteTypeAccount(int id)
        {
            var userId = userService.ObtainUserId();
            var typeAccountExist = await typesAccountsRepo.ObtainById(id, userId);
            if (typeAccountExist is null)
            {
                return RedirectToAction("NotFoundPage", "TypesAccounts");
            }
            await typesAccountsRepo.Delete(id);
            return RedirectToAction("Index");
        }





        [HttpGet]
        public async Task<IActionResult> VerifyExistTypeAccount(string name)
        {
            var userId = userService.ObtainUserId();
            var alreadyExist = await typesAccountsRepo.Exist(name, userId);
            if (alreadyExist)
            {
                return Json($"The name {name} already exists on DB");
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Order([FromBody] int[] ids)
        {
            var userId = userService.ObtainUserId() ;
            var typeAccounts = await typesAccountsRepo.ListItemsAccount(userId);
            var idsTypeAccounts = typeAccounts.Select(x => x.Id);
            var idsTypeAccountsDontBelongToUser = ids.Except(idsTypeAccounts).ToList();

            if(idsTypeAccountsDontBelongToUser.Count>0) 
            {
                return Forbid();
            }

            var typesAccountsOrdered = ids.Select((valor, indice) =>
             new TypeAccount() { Id = valor, OrderNumber = indice + 1 }).AsEnumerable();
            await typesAccountsRepo.Order(typesAccountsOrdered);

            return Ok();
        }
    }
}
