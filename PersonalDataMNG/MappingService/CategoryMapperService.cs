using PersonalDataMNG.Models;

namespace PersonalDataMNG.MappingService
{
    public class CategoryMapperService
    {
        public Category MapToCategory(CategoryVM vm)
        {
            return new Category
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,

                CreatedDate = vm.CreatedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedDate = vm.ModifiedDate,
                ModifiedBy = vm.ModifiedBy,
            };
        }

        public CategoryVM MapToCategoryVM(Category vm)
        {
            return new CategoryVM
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,

                CreatedDate = vm.CreatedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedDate = vm.ModifiedDate,
                ModifiedBy = vm.ModifiedBy,
            };
        }
    }
}
