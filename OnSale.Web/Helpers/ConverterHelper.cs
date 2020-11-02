using OnSale.Common.Entities;
using OnSale.Web.Data;
using OnSale.Web.Data.Entities;
using OnSale.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(DataContext context,ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper ?? throw new ArgumentNullException(nameof(combosHelper));
        }

       
        public Fair ToCategory(CategoryViewModel model, Guid imageId, bool isNew)
        {
            return new Fair
            {
                Id = isNew ? 0 : model.Id,
                ImageId = imageId,
                Name = model.Name
            };
        }

        public CategoryViewModel ToCategoryViewModel(Fair category)
        {
            return new CategoryViewModel
            {
                Id = category.Id,
                ImageId = category.ImageId,
                Name = category.Name
            };
        }

        public async Task<Cow> ToProductAsync(CowViewModel model, bool isNew)
        {
            return new Cow
            {
                Fair = await _context.Faires.FindAsync(model.FairId),
                Description = model.Description,
                Id = isNew ? 0 : model.Id,
                IsActive = model.IsActive,
                Name = model.Name,
                Price = ToPrice(model.PriceString),
                ProductImages = model.ProductImages
            };
        }

        private decimal ToPrice(string priceString)
        {
            string nds = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (nds == ".")
            {
                priceString = priceString.Replace(',', '.');

            }
            else
            {
                priceString = priceString.Replace('.', ',');
            }

            return decimal.Parse(priceString);
        }

        public CowViewModel ToProductViewModel(Cow cow)
        {
            return new CowViewModel
            {
                Fairs = _combosHelper.GetComboCategories(),
                Fair = cow.Fair,
                FairId = cow.Fair.Id,
                Description = cow.Description,
                Id = cow.Id,
                IsActive = cow.IsActive,
                Name = cow.Name,
                Price = cow.Price,
                PriceString = $"{cow.Price}",
                ProductImages = cow.ProductImages
            };
        }

    }

}


