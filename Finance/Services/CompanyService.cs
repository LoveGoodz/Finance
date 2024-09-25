using Finance.Data;
using Finance.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace Finance.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly FinanceContext _context;
        private readonly IDatabase _redisDb;

        public CompanyService(FinanceContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redisDb = redis.GetDatabase();
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync(string name, int pageNumber, int pageSize)
        {
            var cacheKey = "all_companies";
            var cachedData = await _redisDb.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<Company>>(cachedData);
            }

            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            var companies = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (companies.Any())
            {
                var serializedCompanies = JsonSerializer.Serialize(companies);
                await _redisDb.StringSetAsync(cacheKey, serializedCompanies, TimeSpan.FromMinutes(5));
            }

            return companies;
        }

        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            var cacheKey = $"company_{id}";
            var cachedData = await _redisDb.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<Company>(cachedData);
            }

            var company = await _context.Companies.FindAsync(id);

            if (company != null)
            {
                var serializedCompany = JsonSerializer.Serialize(company);
                await _redisDb.StringSetAsync(cacheKey, serializedCompany, TimeSpan.FromMinutes(5));
            }

            return company;
        }

        public async Task<Company> AddCompanyAsync(Company company)
        {
            company.CreatedAt = DateTime.Now;
            company.UpdatedAt = DateTime.Now;

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            await _redisDb.KeyDeleteAsync("all_companies");
            return company;
        }

        public async Task<bool> UpdateCompanyAsync(Company company)
        {
            _context.Entry(company).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                await _redisDb.KeyDeleteAsync($"company_{company.ID}");
                await _redisDb.KeyDeleteAsync("all_companies");
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return false;
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            await _redisDb.KeyDeleteAsync($"company_{id}");
            await _redisDb.KeyDeleteAsync("all_companies");
            return true;
        }
    }
}
