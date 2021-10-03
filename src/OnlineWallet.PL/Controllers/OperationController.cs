using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OlineWallet.Domain.Extensions;
using OnlineWallet.BLL.Interfaces;
using OnlineWallet.Domain;
using OnlineWallet.Domain.Enums;
using OnlineWallet.PL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineWallet.PL.Controllers
{
    [Authorize]
    public class OperationController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IAccountService _accountService;

        public OperationController(IWalletService walletService, IAccountService accountService)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        public async Task<IActionResult> Index()
        {
            var userId = await _accountService.GetUserIdByNameAsync(User.Identity.Name);
            var operations = await _walletService.GetAllOperationAsync(userId);

            var operationViewModel = new List<OperationViewModel>();

            foreach (var operation in operations)
            {
                operationViewModel.Add(new OperationViewModel
                {
                    Id = operation.Id,
                    Amount = operation.Amount,
                    Description = operation.Description,
                    FinishDate = operation.FinishDate,
                    Name = operation.Name,
                    OperationType = operation.OperationTypes.ValidateOperationType(),
                    OperationCategory = operation.OperationCategories.ValidateOperationCategory(),
                    Percent = operation.Percent,
                    Residue = operation.Residue,
                    StartDate = operation.StartDate,
                    Planned = operation.Planned,
                });
            }

            operationViewModel.Where(o => o.StartDate.Month <= DateTime.Now.Month)
                              .OrderBy(o => o.StartDate)
                              .ToList();

            return View(operationViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            GenerateOperationCategoryList();
            GenerateOperationTypeList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OperationActionViewModel model)
        {
            model = model ?? throw new ArgumentNullException(nameof(model));

            if (ModelState.IsValid)
            {
                var userId = await _accountService.GetUserIdByNameAsync(User.Identity.Name);
                var balance = await _walletService.GetBalanceByUserIdAsync(userId);

                var newOperation = new Operation
                {
                    BalanceId = balance.Id,
                    Name = model.Name,
                    Amount = model.Amount,
                    Description = model.Description,
                    OperationTypes = OperationTypeExtension.ToOperationType(model.Type),
                    OperationCategories = OperationCategoryExtension.ToOperationCategory(model.Category),
                    Percent = model.Percent,
                    FinishDate = model.FinishDate,
                    StartDate = model.StartDate,
                    Planned = model.Planned,
                };

                if (await _walletService.AddOperationAsync(newOperation))
                {
                    return RedirectToAction("Index", "Operation");
                }
                else
                {
                    // TODO: fix this
                    throw new ArgumentException(nameof(Operation), "Not enough money on the balance.");
                }
            }

            GenerateOperationTypeList();
            GenerateOperationCategoryList();

            return View(model);
        }

        [NonAction]
        private void GenerateOperationCategoryList()
        {
            IEnumerable<OperationCategoryModel> operationCategories = new List<OperationCategoryModel>
            {
                new OperationCategoryModel { Id = (int)OperationCategory.Credit, Category = OperationCategory.Credit.ToLocal() },
                new OperationCategoryModel { Id = (int)OperationCategory.Loan, Category = OperationCategory.Loan.ToLocal() },
                new OperationCategoryModel { Id = (int)OperationCategory.MoneyBox, Category = OperationCategory.MoneyBox.ToLocal() },
                new OperationCategoryModel { Id = (int)OperationCategory.Other, Category = OperationCategory.Other.ToLocal() },
            };

            ViewBag.OperationCategories = new SelectList(operationCategories, "Id", "Category");
        }

        [NonAction]
        private void GenerateOperationTypeList()
        {
            IEnumerable<OperationTypeModel> operationTypes = new List<OperationTypeModel>
            {
                new OperationTypeModel { Id = (int)OperationType.Income, Type = OperationType.Income.ToLocal() },
                new OperationTypeModel { Id = (int)OperationType.Outcome, Type = OperationType.Outcome.ToLocal() },
            };

            ViewBag.OperationTypes = new SelectList(operationTypes, "Id", "Type");
        }
    }
}
