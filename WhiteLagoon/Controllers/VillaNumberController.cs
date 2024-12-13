using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;
using WhiteLagoon.ViewModels;

namespace WhiteLagoon.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberController( IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties:"Villa");
            return View(villaNumbers);
        }
        #region Create
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = _unitOfWork.Villa.GetAll()
                                                .Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                })
            };


            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Create(VillaNumberVM villaNumberModel)
        {
            bool roomNumberExsits = _unitOfWork.VillaNumber.Any(x => x.Villa_Number == villaNumberModel.Villa_Number);
            if (ModelState.IsValid && !roomNumberExsits)
            {

                VillaNumber villaNumberDb = new VillaNumber()
                {
                    Villa_Number = villaNumberModel.Villa_Number,
                    SpecialDetails = villaNumberModel.SpecialDetails,
                    Villa_id = villaNumberModel.Villa_id
                };

                _unitOfWork.VillaNumber.Add(villaNumberDb);
                _unitOfWork.Save();
                TempData["success"] = "Villa Number Created successfly";
                return RedirectToAction("Index");


            }
            TempData["error"] = "Can't Create the Villa Number";
            if (roomNumberExsits)
            {
                TempData["error"] = "Villa Number already exists";
			}
            villaNumberModel.VillaList = _unitOfWork.Villa.GetAll()
												.Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                });

            return View(villaNumberModel);
        }
        #endregion

        #region Update
        public IActionResult Update(int villaId)
        {
            var villNumberDB = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == villaId);
            var villaNumberVM = new VillaNumberVM()
            {
                VillaList = _unitOfWork.Villa.GetAll()
												.Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                }),
                Villa_Number = villNumberDB.Villa_Number,
                Villa_id = villNumberDB.Villa_id,
                SpecialDetails = villNumberDB.SpecialDetails


            };
            if (villNumberDB == null)
            {
                TempData["error"] = " Villa Number dosn't exists ";
                return RedirectToAction("error", "Home");
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberModel)
        {

            if (ModelState.IsValid)
            {

                VillaNumber villaNumberDb = new VillaNumber()
                {
                    Villa_Number = villaNumberModel.Villa_Number,
                    SpecialDetails = villaNumberModel.SpecialDetails,
                    Villa_id = villaNumberModel.Villa_id
                };

				_unitOfWork.VillaNumber.Update(villaNumberDb);
				_unitOfWork.Save();
                TempData["success"] = "Villa Number Updated successfly";
                return RedirectToAction(nameof(Index));


            }
            TempData["error"] = "Can't Update the Villa Number";

            villaNumberModel.VillaList = _unitOfWork.Villa.GetAll()
                                                .Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                });

            return View(villaNumberModel);
        }
        #endregion

        public IActionResult Delete(int villaId)
        {
            var villaNumberDB = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == villaId);
            var villaNumberVM = new VillaNumberVM()
            {
                VillaList = _unitOfWork.Villa.GetAll()
												.Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                }),
                Villa_Number = villaNumberDB.Villa_Number,
                Villa_id = villaNumberDB.Villa_id,
                SpecialDetails = villaNumberDB.SpecialDetails


            };
            if (villaNumberDB == null)
            {
                TempData["error"] = " Villa Number dosn't exists ";
                return RedirectToAction("error", "Home");
            }
            return View(villaNumberVM);
        }


        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberModel)
        {
            var villaNumberDb = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == villaNumberModel.Villa_Number);
            if(villaNumberDb is not null)
            {
                _unitOfWork.VillaNumber.Remove(villaNumberDb);
                _unitOfWork.Save();
                TempData["success"] = "Villa Number Deleted successfly";
                return RedirectToAction(nameof(Index));

            }
            TempData["error"] = "Can't Delete the Villa Number";
            return View("Delete");
        }



    }
           


    
}
