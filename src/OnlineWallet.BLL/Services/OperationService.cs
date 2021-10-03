using Microsoft.EntityFrameworkCore;
using OnlineWallet.BLL.Interfaces;
using OnlineWallet.DAL.Interfaces;
using OnlineWallet.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineWallet.BLL.Managers
{
    public class OperationService : IWalletService
    {
        private readonly IRepository<Operation> _operationRepository;
        private readonly IRepository<Balance> _balanceRepository;

        public OperationService(IRepository<Operation> operationRepository,
                                IRepository<Balance> balanceRepository)
        {
            _operationRepository = operationRepository ?? throw new ArgumentNullException(nameof(operationRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
        }

        public async Task<bool> AddOperationAsync(Operation operation)
        {
            bool result = false;

            switch (operation.OperationTypes)
            {
                case Domain.Enums.OperationType.Income:

                    switch (operation.OperationCategories)
                    {
                        case Domain.Enums.OperationCategory.Credit:

                            #region creditIncomeLogic

                            if (operation.Planned && operation.Percent != 0)
                            {
                                int paymentPeriod = СalculatePaymentPeriod(operation.StartDate, operation.FinishDate);

                                double paymentPerMonth = (operation.Amount / paymentPeriod);
                                double paymentPercent = (paymentPerMonth * (double)operation.Percent / 100);
                                double totalPayment = paymentPerMonth + paymentPercent;

                                

                                for (var i = 1; i <= paymentPeriod; i++)
                                {
                                    
                                    Operation plannedOperation = new Operation()
                                    {
                                        BalanceId = operation.BalanceId,
                                        Name = $"Credit repayment {operation.Name} #{i}",
                                        Amount = totalPayment,
                                        StartDate = operation.StartDate.AddMonths(i),
                                        FinishDate = operation.FinishDate,
                                        Planned = true,
                                        OperationTypes = Domain.Enums.OperationType.Outcome,
                                        OperationCategories = Domain.Enums.OperationCategory.Credit,
                                        Percent = operation.Percent,
                                        Residue = totalPayment,
                                        Description = $"Credit repayment {operation.Name} #{i} #{Guid.NewGuid()}",
                                    };

                                    await _operationRepository.CreateAsync(plannedOperation);
                                }
                            }

                            operation.Description += $" #{Guid.NewGuid()}";

                            await _operationRepository.CreateAsync(operation);

                            var newBalance = await GetBalanceAsync(operation.BalanceId);
                            newBalance.Amount += operation.Amount;

                            await _balanceRepository.UpdateAsync(newBalance);

                            result = true;
                            #endregion

                            break;

                        case Domain.Enums.OperationCategory.MoneyBox:

                            #region moneyboxIncomeLogic

                            var moneyBoxOperations = _operationRepository
                                .GetAll()
                                .Where(o => o.OperationCategories == Domain.Enums.OperationCategory.MoneyBox)
                                .AsNoTracking();

                            foreach (var moneyBoxOperation in moneyBoxOperations)
                            {
                                if (moneyBoxOperation.Name == operation.Name)
                                {
                                    moneyBoxOperation.Amount += operation.Amount;
                                    await _operationRepository.UpdateAsync(moneyBoxOperation);
                                }
                                else
                                {
                                    await _operationRepository.CreateAsync(operation);
                                }
                            }

                            result = true;

                            #endregion

                            break;

                        default:

                            #region defalutIncomeLogic

                            await _operationRepository.CreateAsync(operation);

                            var balance = await GetBalanceAsync(operation.BalanceId);
                            balance.Amount += operation.Amount;

                            await _balanceRepository.UpdateAsync(balance);

                            result = true;
                            #endregion

                            break;
                    }
                    break;

                case Domain.Enums.OperationType.Outcome:

                    switch (operation.OperationCategories)
                    {
                        case Domain.Enums.OperationCategory.Credit:

                            #region creditOutcomeLogic

                            var credits = _operationRepository.GetAll()
                                                              .Where(o => o.OperationTypes == Domain.Enums.OperationType.Outcome)
                                                              .Where(o => o.OperationCategories == Domain.Enums.OperationCategory.Credit)
                                                              .Where(o => o.Planned == true)
                                                              .OrderBy(o => o.StartDate)
                                                              .AsNoTracking();

                            var credit = credits.FirstOrDefault();

                            await CountingAmountCreditAsync(operation.Amount, credit.Amount, credits);

                            var balance = await GetBalanceAsync(operation.BalanceId);
                            balance.Amount -= operation.Amount;

                            await _balanceRepository.UpdateAsync(balance);

                            #endregion

                            break;

                        case Domain.Enums.OperationCategory.MoneyBox:

                            #region moneyboxOutcomeLogic

                            var moneyBoxOperations = _operationRepository
                                .GetAll()
                                .Where(o => o.OperationCategories == Domain.Enums.OperationCategory.MoneyBox)
                                .AsNoTracking();

                            foreach (var moneyBoxOperation in moneyBoxOperations)
                            {
                                if (moneyBoxOperation.Name == operation.Name)
                                {
                                    if (moneyBoxOperation.Amount > operation.Amount)
                                    {
                                        moneyBoxOperation.Amount -= operation.Amount;
                                    }
                                    else
                                    {
                                        moneyBoxOperation.Amount = 0;
                                    }

                                    await _operationRepository.UpdateAsync(moneyBoxOperation);

                                    await _operationRepository.CreateAsync(operation);
                                }
                                else
                                {
                                    result = false;
                                }

                            }

                            result = true;
                            #endregion 

                            break;

                        case Domain.Enums.OperationCategory.Loan:
                        default:

                            #region otherOutcomeLogic

                            if (await DoesOperationValidAsync(operation.BalanceId, operation.Amount))
                            {
                                await _operationRepository.CreateAsync(operation);

                                var newBalance = await GetBalanceAsync(operation.BalanceId);
                                newBalance.Amount -= operation.Amount;

                                await _balanceRepository.UpdateAsync(newBalance);
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }

                            #endregion

                            break;

                    }
                    break;

                default:
                    break;
            }

            return result;
        }

        async Task CountingAmountCreditAsync(double operationAmount, double creditAmount, IQueryable<Operation> credits)
        {
            if (operationAmount > creditAmount)
            {
                var newOperationAmount = operationAmount - creditAmount;

                var newCredits = credits.Where(c => c.Planned == true);

                var credit = newCredits.FirstOrDefault();

                credit.Residue = 0;
                credit.Planned = false;

                await _operationRepository.UpdateAsync(credit);

                await CountingAmountCreditAsync(newOperationAmount, creditAmount, newCredits);
            }

            if (operationAmount < creditAmount)
            {
                var newCredits = credits.Where(c => c.Planned == true);

                var credit = newCredits.FirstOrDefault();

                credit.Residue -= operationAmount;

                await _operationRepository.UpdateAsync(credit);
            }

            if (operationAmount == creditAmount)
            {
                var newCredits = credits.Where(c => c.Planned == true);

                var credit = newCredits.FirstOrDefault();

                credit.Residue = 0;
                credit.Planned = false;

                await _operationRepository.UpdateAsync(credit);
            }
        }

        public async Task EditLastOperationAsync(Operation newOperation)
        {
            newOperation = newOperation ?? throw new ArgumentNullException(nameof(newOperation));

            var existingOperation = await _operationRepository.GetAsync(newOperation.Id);

            if (existingOperation is null)
            {
                throw new KeyNotFoundException();
            }

            async Task<bool> CheckToUpdateAsync(Operation existingOperation, Operation newOperation)
            {
                bool updated = false;

                if (existingOperation.Amount != newOperation.Amount && await DoesOperationValidAsync(newOperation.BalanceId, newOperation.Amount))
                {
                    existingOperation.Amount = newOperation.Amount;
                    updated = true;
                }

                if (existingOperation.Description != newOperation.Description)
                {
                    existingOperation.Description = newOperation.Description;
                    updated = true;
                }

                if (existingOperation.Name != newOperation.Name)
                {
                    existingOperation.Name = newOperation.Name;
                    updated = true;
                }

                if (existingOperation.OperationCategories != newOperation.OperationCategories)
                {
                    existingOperation.OperationCategories = newOperation.OperationCategories;
                    updated = true;
                }

                if (existingOperation.OperationTypes != newOperation.OperationTypes)
                {
                    existingOperation.OperationTypes = newOperation.OperationTypes;
                    updated = true;
                }

                if (existingOperation.Percent != newOperation.Percent)
                {
                    existingOperation.Percent = newOperation.Percent;
                    updated = true;
                }

                if (existingOperation.StartDate != newOperation.StartDate)
                {
                    existingOperation.StartDate = newOperation.StartDate;
                    updated = true;
                }

                if (existingOperation.FinishDate != newOperation.FinishDate)
                {
                    existingOperation.FinishDate = newOperation.FinishDate;
                    updated = true;
                }

                return updated;
            }

            var result = CheckToUpdateAsync(existingOperation, newOperation);

            if (await result)
            {
                await _operationRepository.UpdateAsync(newOperation);
            }
        }

        public async Task<Balance> GetBalanceAsync(string id)
        {
            return await _balanceRepository.GetAsync(id);
        }

        private async Task<bool> DoesOperationValidAsync(string balanceId, double operationAmount)
        {
            var balance = await GetBalanceAsync(balanceId);

            if (balance.Amount - operationAmount >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<Operation>> GetAllOperationAsync(string userId)
        {
            var balance = await GetBalanceByUserIdAsync(userId);

            var listOfOperations = new List<Operation>();

            var allOperations = await _operationRepository
                .GetAll()
                .AsNoTracking()
                .Where(o => o.BalanceId == balance.Id)
                .ToListAsync();

            if (!allOperations.Any())
            {
                return listOfOperations;
            }

            foreach (var operation in allOperations)
            {
                listOfOperations.Add(new Operation
                {
                    Id = operation.Id,
                    Amount = operation.Amount,
                    Description = operation.Description,
                    FinishDate = operation.FinishDate,
                    Name = operation.Name,
                    OperationCategories = operation.OperationCategories,
                    OperationTypes = operation.OperationTypes,
                    Percent = operation.Percent,
                    StartDate = operation.StartDate,
                    BalanceId = balance.Id,
                });
            }

            return listOfOperations;
        }

        public async Task<Balance> GetBalanceByUserIdAsync(string userId)
        {
            var balance = await _balanceRepository
                .GetAll().Where(b => b.UserId == userId)
                .FirstOrDefaultAsync();

            return balance;
        }

        public int СalculatePaymentPeriod(DateTime start, DateTime end)
        {
            return (end.Year * 12 + end.Month) - (start.Year * 12 + start.Month);
        }
    }
}
