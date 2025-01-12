using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;
using WhiteLagoon.ViewModels;

namespace WhiteLagoon.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
        }
        public IActionResult Index()
        {
            var villaNumbers = _villaNumberService.GetAllVillaNumbers(includeProperties:"Villa");
            return View(villaNumbers);
        }


        #region Create
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = _villaService.GetAllVillas()
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
            bool roomNumberExsits = _villaNumberService.CheckVillaNumberExist(villaNumberModel.Villa_Number);
            if (ModelState.IsValid && !roomNumberExsits)
            {
                // mapping 
                VillaNumber villaNumberDb = new VillaNumber()
                {
                    Villa_Number = villaNumberModel.Villa_Number,
                    SpecialDetails = villaNumberModel.SpecialDetails,
                    Villa_id = villaNumberModel.Villa_id
                };

                bool created = _villaNumberService.CreateVillaNumber(villaNumberDb);
                if (created)
                {
                    TempData["success"] = "Villa Number Created successfly";
                    return RedirectToAction("Index");

                }

            }
            TempData["error"] = "Can't Create the Villa Number";
            if (roomNumberExsits)
            {
                TempData["error"] = "Villa Number already exists";
			}
            villaNumberModel.VillaList = _villaService.GetAllVillas()
												.Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                });

            return View(villaNumberModel);
        }
        #endregion

        #region Update
        public IActionResult Update(int villaNumberId)
        {
            var villNumberDB = _villaNumberService.GetVillaNumberById(villaNumberId);
            var villaNumberVM = new VillaNumberVM()
            {
                VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
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
                // Mapping

                VillaNumber villaNumberDb = new VillaNumber()
                {
                    Villa_Number = villaNumberModel.Villa_Number,
                    SpecialDetails = villaNumberModel.SpecialDetails,
                    Villa_id = villaNumberModel.Villa_id
                };
                //Update to the database
			    bool updated =  _villaNumberService.UpdateVillaNumber(villaNumberDb);
                if (updated)
                {
                    TempData["success"] = "Villa Number Updated successfly";
                    return RedirectToAction(nameof(Index));
                }
              


            }
            TempData["error"] = "Can't Update the Villa Number";

            villaNumberModel.VillaList = _villaService.GetAllVillas()
                                                .Select(x => new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                });

            return View(villaNumberModel);
        }
        #endregion

        #region Delete

        public IActionResult Delete(int villaNumberId)
        {
            var villaNumberDB = _villaNumberService.GetVillaNumberById(villaNumberId);
            var villaNumberVM = new VillaNumberVM()
            {
                VillaList = _villaService.GetAllVillas()
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
            bool deleted = _villaNumberService.DeleteVillaNumber(villaNumberModel.Villa_Number);

            if (deleted)
            {
                TempData["success"] = "Villa Number Deleted successfly";
                return RedirectToAction(nameof(Index));

            }
            TempData["error"] = "Can't Delete the Villa Number";
            return View("Delete");
        }


        #endregion

    }
           


    
}
