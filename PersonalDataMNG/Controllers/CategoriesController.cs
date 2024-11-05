using DataTablesParser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalDataMNG.Data;
using PersonalDataMNG.MappingService;
using PersonalDataMNG.Models;
using PersonalDataMNG.Models.CommonViewModel;

namespace PersonalDataMNG.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<CategoryVM> listCategory =await  GetAllCategory();
            if (listCategory.Count < 1)
            {
                await SeedData.CreateCreatedDateList(_context);
            }
            return View();
        }

        public IActionResult Data()
        {
            var listCategory = _context.Category.Where(x => x.Cancelled == false).OrderByDescending(x => x.Id).AsQueryable();
            var parser = new Parser<Category>(Request.Form, listCategory);
            return Json(parser.Parse());
        }

        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            var vm = await _context.Category.FirstOrDefaultAsync(m => m.Id == id);

            var mapper = new Mapper<Category, CategoryVM>();
            CategoryVM result = mapper.Map(vm);
            return PartialView("_Details", result);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            Category _Category = new();
            if (id > 0) _Category = await _context.Category.Where(x => x.Id == id).FirstOrDefaultAsync();

            var mapper = new Mapper<Category, CategoryVM>();
            CategoryVM vm = mapper.Map(_Category);
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(CategoryVM vm)
        {
            JsonResultViewModel _JsonResultViewModel = new();
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.Id > 0)
                    {
                        var _Categories = await _context.Category.FindAsync(vm.Id);
                        vm.CreatedDate = _Categories.CreatedDate;
                        vm.CreatedBy = _Categories.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = HttpContext.User.Identity.Name;
                        _context.Entry(_Categories).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        _JsonResultViewModel.AlertMessage = "Category Updated Successfully. ID: " + _Categories.Id;
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                    }
                    else
                    {
                        //Using reflection mapping
                        //var mapper = new Mapper<CategoryVM, Category>();
                        //Category _Categorie = mapper.Map(vm);

                        //Using Specific Mapper Service
                        CategoryMapperService _CategoryMapperService = new();
                        Category _Categorie = _CategoryMapperService.MapToCategory(vm);


                        _Categorie.CreatedDate = DateTime.Now;
                        _Categorie.ModifiedDate = DateTime.Now;
                        _Categorie.CreatedBy = HttpContext.User.Identity.Name;
                        _Categorie.ModifiedBy = HttpContext.User.Identity.Name;
                        _context.Add(_Categorie);
                        await _context.SaveChangesAsync();

                        _JsonResultViewModel.AlertMessage = "Category Created Successfully. ID: " + _Categorie.Id;
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                    }
                }
                _JsonResultViewModel.AlertMessage = "Operation failed.";
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (Exception ex)
            {
                _JsonResultViewModel.IsSuccess = false;
                _JsonResultViewModel.AlertMessage = ex.Message;
                return new JsonResult(_JsonResultViewModel);
                throw;
            }
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(Int64 id)
        {
            try
            {
                var _Categories = await _context.Category.FindAsync(id);
                _Categories.ModifiedDate = DateTime.Now;
                _Categories.ModifiedBy = HttpContext.User.Identity.Name;
                _Categories.Cancelled = true;

                _context.Update(_Categories);
                await _context.SaveChangesAsync();
                return new JsonResult(_Categories);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CategoryVM>> GetAllCategory()
        {
            List<Category> listCategory = await _context.Category.ToListAsync();
            List<CategoryVM> listCategoryVM = listCategory.Select(x => new CategoryVM
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,

                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate,
                ModifiedBy = x.ModifiedBy,
            }).ToList();

            return listCategoryVM;
        }
    }
}
