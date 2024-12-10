using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.ViewModels;

namespace WhiteLagoon.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly AppDbContext _context;

        public VillaNumberController( AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var villaNumbers = _context.VillaNumbers.Include(x=>x.Villa).ToList();
            return View(villaNumbers);
        }
        #region Create
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = _context.Villas.ToList()
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
            bool roomNumberExsits = _context.VillaNumbers.Any(x => x.Villa_Number == villaNumberModel.Villa_Number);
            if (ModelState.IsValid && !roomNumberExsits)
            {

                VillaNumber villaNumberDb = new VillaNumber()
                {
                    Villa_Number = villaNumberModel.Villa_Number,
                    SpecialDetails = villaNumberModel.SpecialDetails,
                    Villa_id = villaNumberModel.Villa_id
                };

                _context.Add(villaNumberDb);
                _context.SaveChanges();
                TempData["success"] = "Villa Number Created successfly";
                return RedirectToAction("Index");


            }
            TempData["error"] = "Can't Create the Villa Number";
            if (roomNumberExsits)
            {
                TempData["error"] = "Villa Number already exists";
            }
            villaNumberModel.VillaList = _context.Villas.ToList()
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
            var villNumberDB = _context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaId);
            var villaNumberVM = new VillaNumberVM()
            {
                VillaList = _context.Villas.ToList()
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

                _context.Update(villaNumberDb);
                _context.SaveChanges();
                TempData["success"] = "Villa Number Updated successfly";
                return RedirectToAction(nameof(Index));


            }
            TempData["error"] = "Can't Update the Villa Number";

            villaNumberModel.VillaList = _context.Villas.ToList()
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
            var villaNumberDB = _context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaId);
            var villaNumberVM = new VillaNumberVM()
            {
                VillaList = _context.Villas.ToList()
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
            var villaNumberDb = _context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumberModel.Villa_Number);
            if(villaNumberDb is not null)
            {
                _context.Remove(villaNumberDb);
                _context.SaveChanges();
                TempData["success"] = "Villa Number Deleted successfly";
                return RedirectToAction(nameof(Index));

            }
            TempData["error"] = "Can't Delete the Villa Number";

           
            return View("Delete");
        }



    }
           


    
}
