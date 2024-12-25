using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.ViewModels;

namespace WhiteLagoon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties:"Villa");
            return View(amenities);
        }
        #region Create
        public IActionResult Create()
        {
            AmenityVM model = new AmenityVM()
            {
                VillaList = _unitOfWork.Villa.GetAll()
                                                .Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                })
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
                _unitOfWork.Amenity.Add(amenityDB);
                _unitOfWork.Save();
                TempData["success"] = "A new Amenity Created successfuly";
                return RedirectToAction("Index");
            }
            TempData["error"] = "can't create Amentiy";
            return View(amenityVM);
        } 
        #endregion
        public IActionResult Update(int ameityId)
        {
            var amentiyDB = _unitOfWork.Amenity.Get(x => x.Id == ameityId);
            var amenityVM = new AmenityVM()
            {
                VillaList = _unitOfWork.Villa.GetAll()
                                                .Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                }),
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
                _unitOfWork.Amenity.Update(amenityDB);
                _unitOfWork.Save();
                TempData["success"] = "Amenity is updated successfuly";
                return RedirectToAction(nameof(Index));

            }
            TempData["error"] = "Can not Update Amenity";
            return View(amenityVM);


        }
        public IActionResult Delete(int villaId)
        {

            var amenityDB = _unitOfWork.Amenity.Get(x => x.Id == villaId);
            var amenityVM = new AmenityVM()
            {
                VillaList = _unitOfWork.Villa.GetAll()
                                                .Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                }),
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
            var amenityDB = _unitOfWork.Amenity.Get(x => x.Id == amenityVM.Id);
            if (amenityDB is not null)
            {
                _unitOfWork.Amenity.Remove(amenityDB);
                _unitOfWork.Save();
                TempData["success"] = "Villa Number Deleted successfly";
                return RedirectToAction(nameof(Index));

            }
            TempData["error"] = "Can't Delete the Villa Number";
            return View(amenityVM);
        }
    }
}
