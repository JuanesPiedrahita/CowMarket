using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;
using OnSale.Web.Data;
using OnSale.Web.Data.Entities;
using OnSale.Web.Helpers;
using OnSale.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CowsController : Controller
    {
        private readonly DataContext _context;
        private readonly IBlobHelper _blobHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;

        public CowsController(DataContext context, IBlobHelper blobHelper, ICombosHelper combosHelper, IConverterHelper converterHelper)
        {
            _context = context;
            _blobHelper = blobHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Cows
                .Include(f => f.Fair)
                .Include(o => o.Owner)
                .Include(c => c.city)
                .Include(p => p.ProductImages)
                .Include(p => p.Qualifications)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            CowViewModel model = new CowViewModel
            {
                Fairs = _combosHelper.GetComboCategories(),
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CowViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Cow cow = await _converterHelper.ToProductAsync(model, true);

                    if (model.ImageFile != null)
                    {
                        Guid imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                        cow.ProductImages = new List<CowImage>
                        {
                            new CowImage { ImageId = imageId }
                        };
                    }

                    _context.Add(cow);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There are a record with the same name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            model.Fairs = _combosHelper.GetComboCategories();
            return View(model);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cow cow = await _context.Cows
                .Include(p => p.Fair)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (cow == null)
            {
                return NotFound();
            }

            CowViewModel model = _converterHelper.ToProductViewModel(cow);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CowViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Cow cow = await _converterHelper.ToProductAsync(model, false);

                    if (model.ImageFile != null)
                    {
                        Guid imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                        if (cow.ProductImages == null)
                        {
                            cow.ProductImages = new List<CowImage>();
                        }

                        cow.ProductImages.Add(new CowImage { ImageId = imageId });
                    }

                    _context.Update(cow);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There are a record with the same name.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            model.Fairs = _combosHelper.GetComboCategories();
            return View(model);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cow cow = await _context.Cows
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (cow == null)
            {
                return NotFound();
            }

            try
            {
                _context.Cows.Remove(cow);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cow cow = await _context.Cows
                .Include(c => c.Fair)
                .Include(c => c.ProductImages)
                .Include(c => c.Qualifications)
                .ThenInclude(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cow == null)
            {
                return NotFound();
            }

            return View(cow);
        }
        public async Task<IActionResult> AddImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cow cow = await _context.Cows.FindAsync(id);
            if (cow == null)
            {
                return NotFound();
            }

            AddProductImageViewModel model = new AddProductImageViewModel { CowId = cow.Id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddProductImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                Cow cow = await _context.Cows
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == model.CowId);
                if (cow == null)
                {
                    return NotFound();
                }

                try
                {
                    Guid imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                    if (cow.ProductImages == null)
                    {
                        cow.ProductImages = new List<CowImage>();
                    }

                    cow.ProductImages.Add(new CowImage { ImageId = imageId });
                    _context.Update(cow);
                    await _context.SaveChangesAsync();
                    return RedirectToAction($"{nameof(Details)}/{cow.Id}");

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(model);
        }
        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CowImage productImage = await _context.ProductImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            Cow cow = await _context.Cows.FirstOrDefaultAsync(p => p.ProductImages.FirstOrDefault(pi => pi.Id == productImage.Id) != null);
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{cow.Id}");
        }

    }

}
