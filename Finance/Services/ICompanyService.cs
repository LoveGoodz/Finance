namespace Finance.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetCompaniesAsync(string name, int pageNumber, int pageSize);
        Task<Company> GetCompanyByIdAsync(int id);
        Task<Company> AddCompanyAsync(Company company);
        Task<bool> UpdateCompanyAsync(Company company);
        Task<bool> DeleteCompanyAsync(int id);
    }
}
