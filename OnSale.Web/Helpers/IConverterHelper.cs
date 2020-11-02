using OnSale.Common.Entities;
using OnSale.Web.Data.Entities;
using OnSale.Web.Models;
using System;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public interface IConverterHelper
    {
        Fair ToCategory(CategoryViewModel model, Guid imageId, bool isNew);

        CategoryViewModel ToCategoryViewModel(Fair category);

        Task<Cow> ToProductAsync(CowViewModel model, bool isNew);

        CowViewModel ToProductViewModel(Cow product);

    }

}
