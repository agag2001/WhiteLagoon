using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Controllers
{
  
    public class VillaController : Controller
    {
        private readonly IUnitOfWork  _unitOfWork;
        public VillaController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
        }

        public AppDbContext Context { get; }

        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();  
            return View(villas);
        }
        public IActionResult Create()
        {
            return View();  
        }
        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if (obj.Name== obj.Description)
            {
                ModelState.AddModelError("", "Name can't be the same as Description");

            }
            if (ModelState.IsValid)
            {
				_unitOfWork.Villa.Add(obj);
				_unitOfWork.Save();
                TempData["success"] = "Villa is Created successfuly . ";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Villa is Created successfuly . ";
            return View(obj);
          
        }

        #region update
        public IActionResult Update(int VillaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(x => x.Id == VillaId);
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
				_unitOfWork.Villa.Update(obj);
				_unitOfWork.Save();
                TempData["success"] = "Villa is updated successfuly . ";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Villa can't be update ";
            return View(obj);

        }
        #endregion
     
        public IActionResult Delete(int VillaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(x => x.Id == VillaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View( obj);

        }
        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            Villa? objDb = _unitOfWork.Villa.Get(x => x.Id == obj.Id);

            if (objDb is not null)
            {
				_unitOfWork.Villa.Remove(objDb);
				_unitOfWork.Save();
                TempData["success"] = "the villa has been deleted successfuly";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "the villa can't be deleted";
            return View(obj);

        }

    }
}
