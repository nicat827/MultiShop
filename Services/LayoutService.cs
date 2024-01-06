using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;

namespace MultiShop.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            return await _context.Settings.ToDictionaryAsync(s => s.Key, s=> s.Value);
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
        }
    }
}
