using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork  _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        public VillaController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnviroment)
        {
			_unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnviroment;    
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
                if(obj.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Images\VillaImage");

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        obj.Image.CopyTo(fileStream);
                        obj.ImageUrl = @"\Images\VillaImage\" + fileName; 
                    }
                }
                else
                {
                    //default image
                    obj.ImageUrl = "https://placehold.co/600x400";
                }
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
                if (obj.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Images\VillaImage");
                    //delete old ImageUrl 
                    if (!string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath); 
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        obj.Image.CopyTo(fileStream);
                        obj.ImageUrl = @"\Images\VillaImage\" + fileName;
                    }
                }
               
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
                if (!string.IsNullOrEmpty(objDb.ImageUrl))
                {
                    string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, objDb.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
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
