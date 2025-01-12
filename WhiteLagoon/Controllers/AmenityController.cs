using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.ViewModels;

namespace WhiteLagoon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AmenityController : Controller
    {
        private readonly IAmenityService _amenityService;
        private readonly IVillaService _villaService;


        public AmenityController(IAmenityService amenityService, IVillaService virillaService)
        {

            _amenityService = amenityService;
            _villaService = virillaService;
        }
        public IActionResult Index()
        {
            var amenities = _amenityService.GetAllAmenities(IncludeProperties:"Villa");
            return View(amenities);
        }
        #region Create
        public IActionResult Create()
        {
            AmenityVM model = new AmenityVM()
            {
                VillaList = VillaDropDownList()

            };
            return View("Create", model);
        }
        [HttpPost]
        public IActionResult Create(AmenityVM amenityVM)
        {
            if (ModelState.IsValid)
            {
                var amenityDB = new Amenity()
                {
                    Name = amenityVM.Name,
                    Description = amenityVM.Description,
                    VillaId = amenityVM.VillaId

                };
                bool created = _amenityService.CreateAmenity(amenityDB);
                if (created)
                {
                    TempData["success"] = "A new Amenity Created successfuly";
                    return RedirectToAction("Index");
                }
                
            }
            TempData["error"] = "can't create Amentiy";
            return View(amenityVM);
        } 
        #endregion
        public IActionResult Update(int ameityId)
        {
            var amentiyDB = _amenityService.GetAmenityById( ameityId);
            var amenityVM = new AmenityVM()
            {
                VillaList = VillaDropDownList(),
                Name = amentiyDB.Name,
                VillaId = amentiyDB.VillaId,
                Description = amentiyDB.Description


            };
            if (amentiyDB == null)
            {
                TempData["error"] = " Villa Number dosn't exists ";
                return RedirectToAction("error", "Home");
            }

            return View(amenityVM);
        }


        [HttpPost]
        public IActionResult Update(AmenityVM amenityVM)
        {
            if(ModelState.IsValid)
            {
                var amenityDB = new Amenity()
                {
                    Name = amenityVM.Name,
                    Description = amenityVM.Description,
                    VillaId = amenityVM.VillaId,
                };
               bool updated =  _amenityService.UpdateAmenity(amenityDB);
                if (updated)
                {
                    TempData["success"] = "Amenity is updated successfuly";
                   return RedirectToAction(nameof(Index));
                }
            }
            TempData["error"] = "Can not Update Amenity";
            return View(amenityVM);


        }
        public IActionResult Delete(int villaId)
        {

            var amenityDB = _amenityService.GetAmenityById( villaId);
            var amenityVM = new AmenityVM()
            {
                VillaList = VillaDropDownList(),
                Description = amenityDB.Description,
                VillaId = amenityDB.VillaId,
                Name = amenityDB.Name,
                Id =  amenityDB.Id


            };
            if (amenityDB == null)
            {
                TempData["error"] = " Villa Number dosn't exists ";
                return RedirectToAction("error", "Home");
            }
            return View(amenityVM);
        }


        [HttpPost]
        public IActionResult Delete(AmenityVM amenityVM)
        {
            bool deleted = _amenityService.DeleteAmenity(amenityVM.Id);
            if (deleted)
            {
                TempData["success"] = "Villa Number Deleted successfly";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "Can't Delete the Villa Number";
            return View(amenityVM);
        }
        private IEnumerable<SelectListItem> VillaDropDownList()
        {
            return _villaService.GetAllVillas().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

        }

    }
    
}
