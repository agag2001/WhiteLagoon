using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Controllers
{
  
    public class VillaController : Controller
    {
        private readonly AppDbContext _context;
        public VillaController(AppDbContext context)
        {
            _context = context;
        }

        public AppDbContext Context { get; }

        public IActionResult Index()
        {
            var villas = _context.Villas.ToList();   
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
                _context.Villas.Add(obj);
                _context.SaveChanges();
                return RedirectToAction("index");
            }
            return View(obj);
          
        }
    }
}
