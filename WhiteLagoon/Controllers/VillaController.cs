using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Implementation;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;

        public VillaController(IVillaService villaService)
        {
            _villaService = villaService;
        }


        public IActionResult Index()
        {
            var villas = _villaService.GetAllVillas();  
            return View(villas);
        }
        public IActionResult Create()
        {
            return View();  
        }
        [HttpPost]
        public IActionResult Create(Villa obj)
        {

            if (obj.Name == obj.Description)
            {
                ModelState.AddModelError("", "Name can't be the same as Description");
            }
            if(ModelState.IsValid) {    
               bool sucessCreate = _villaService.CreateVilla(obj);
                if (sucessCreate)
                {
                    TempData["success"] = "Villa is Created successfuly . ";
                    return RedirectToAction(nameof(Index));

                }     
            }

            TempData["error"] = "can't  Created Villa ";
            return View(obj);
          
        }

        #region update
        public IActionResult Update(int VillaId)
        {
            Villa? obj = _villaService.GetVillaById(VillaId);
            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View("Update", obj);

        }
        [HttpPost]
        public IActionResult Update(Villa obj)
        {

            if (ModelState.IsValid)
            {
                bool villaUpdated = _villaService.UpdateVilla(obj);
                if (villaUpdated)
                {
                   TempData["success"] = "Villa is updated successfuly . ";
                   return RedirectToAction(nameof(Index));
                }
             
            }
            TempData["error"] = "Villa can't be update ";
            return View(obj);

        }

        #endregion
     
        public IActionResult Delete(int VillaId)
        {
            Villa? obj = _villaService.GetVillaById(VillaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View( obj);

        }
        [HttpPost]
        public IActionResult Delete(Villa obj)
        {

            bool villaDeleted = _villaService.DeleteVilla(obj.Id);
            if (villaDeleted)
            {
                TempData["success"] = "the villa has been deleted successfuly";
                return RedirectToAction(nameof(Index));
            }

            else
            {
                TempData["error"] = "the villa can't be deleted";
                return View(obj);
            }
           

        }

    }
}
