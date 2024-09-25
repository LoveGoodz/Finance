namespace Finance.Services
{
    public interface IActTransService
    {
        Task<IEnumerable<ActTrans>> GetActTrans(string transactionType, int pageNumber, int pageSize);
        Task<ActTrans> GetActTranById(int id);
        Task<bool> PutActTran(int id, ActTrans actTran);
        Task<ActTrans> PostActTran(ActTrans actTran);
        Task<bool> DeleteActTran(int id);
        bool ActTranExists(int id);
    }
}
