using System.Linq.Expressions;
using Learntendo_backend.Models;
namespace Learntendo_backend.Data
{
    public interface IDataRepository<T> where T : class
    {

        Task<IEnumerable<T>> GetAllFun();
        Task ChecknumofgeneratedFile(int userId);
        Task<List<Subject>> GetAllsubbyUserFun(int userId);
        //NEW
        Task<List<Exam>> GetAllExambysubFun(int subId);
        Task<List<Exam>> GetAllExambyUserFun(int userId, int? subid);
        Task CheckDailyChallenge(int userId);
        Task UpdatePostExamRelatedTable(int examId);
        Task UpdateDeleteExamRelatedTable(int examId);
        Task UpdateDeleteExamWithProgressRelatedTable(int examId);
        Task<T> GetByIdFun(int? id);
        Task AddFun(T entity);
        Task UpdateFun(T entity);
        Task DeleteFun(int id);
        Task UpdatePostExamRelatedTable(object examId);
    }
}
