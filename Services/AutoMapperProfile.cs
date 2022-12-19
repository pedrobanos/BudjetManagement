using AutoMapper;
using BudjetManagement.Models;

namespace BudjetManagement.Services
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Account,CreationAccountViewModel>();
        }
    }
}
