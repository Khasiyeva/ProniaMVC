using Microsoft.AspNetCore.Mvc;
using Pronia.DAL;

namespace Pronia.Services
{
    public class LayoutService
    {
        AppDbContext _context;
        public LayoutService(AppDbContext context) 
        {
          _context = context;
        }

        public async Task<Dictionary<string,string>> GetSetting()
        {
            Dictionary<string, string> setting = _context.Settings.ToDictionary(s=>s.Key,s=>s.Value);
            return setting;
        }
    }
}
