using Learntendo_backend.Models;
namespace Learntendo_backend.Data
{
    public interface IDataRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllFun();
        Task<List<Subject>> GetAllsubbyUserFun(int userId);
        Task<List<Exam>> GetAllExambysubFun(int subId);
        Task<Subject> GetSubbyUserFun(int id);
        Task<T> GetByIdFun(int id);
        Task AddFun(T entity);
        Task UpdateFun(T entity);
        Task DeleteFun(int id);
    }
}
