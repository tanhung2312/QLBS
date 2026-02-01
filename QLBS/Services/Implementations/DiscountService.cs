using QLBS.Dtos.Discount;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<IEnumerable<DiscountCodeResponseDto>> GetAllDiscountsAsync()
        {
            var discounts = await _discountRepository.GetAllAsync();

            return discounts.Select(d => new DiscountCodeResponseDto
            {
                DiscountCodeId = d.DiscountCodeId,
                Code = d.Code,
                DiscountType = d.DiscountType,
                DiscountValue = d.DiscountValue,
                MaxDiscountAmount = d.MaxDiscountAmount,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Quantity = d.Quantity,
                MinOrderAmount = d.MinOrderAmount,
                IsActive = d.IsActive
            });
        }

        public async Task<DiscountCodeResponseDto?> GetDiscountByIdAsync(int id)
        {
            var d = await _discountRepository.GetByIdAsync(id);
            if (d == null) return null;

            return new DiscountCodeResponseDto
            {
                DiscountCodeId = d.DiscountCodeId,
                Code = d.Code,
                DiscountType = d.DiscountType,
                DiscountValue = d.DiscountValue,
                MaxDiscountAmount = d.MaxDiscountAmount,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Quantity = d.Quantity,
                MinOrderAmount = d.MinOrderAmount,
                IsActive = d.IsActive
            };
        }

        public async Task<DiscountCodeResponseDto?> CreateDiscountAsync(CreateDiscountCodeDto createDto)
        {
            if (await _discountRepository.CodeExistsAsync(createDto.Code))
            {
                return null;
            }

            if (createDto.EndDate <= createDto.StartDate)
            {
                return null;
            }

            var entity = new DiscountCode
            {
                Code = createDto.Code.ToUpper(),
                DiscountType = createDto.DiscountType,
                DiscountValue = createDto.DiscountValue,
                MaxDiscountAmount = createDto.MaxDiscountAmount,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                Quantity = createDto.Quantity,
                MinOrderAmount = createDto.MinOrderAmount,
                IsActive = true
            };

            var newDiscount = await _discountRepository.AddAsync(entity);

            return new DiscountCodeResponseDto
            {
                DiscountCodeId = newDiscount.DiscountCodeId,
                Code = newDiscount.Code,
                DiscountType = newDiscount.DiscountType,
                DiscountValue = newDiscount.DiscountValue,
                MaxDiscountAmount = newDiscount.MaxDiscountAmount,
                StartDate = newDiscount.StartDate,
                EndDate = newDiscount.EndDate,
                Quantity = newDiscount.Quantity,
                MinOrderAmount = newDiscount.MinOrderAmount,
                IsActive = newDiscount.IsActive
            };
        }

        public async Task<bool> DeleteDiscountAsync(int id)
        {
            return await _discountRepository.DeleteAsync(id);
        }
    }
}