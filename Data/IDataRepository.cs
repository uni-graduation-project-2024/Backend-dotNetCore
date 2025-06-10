using System.Linq.Expressions;
using Learntendo_backend.Models;
using Microsoft.EntityFrameworkCore;
namespace Learntendo_backend.Data
{
    public interface IDataRepository<T> where T : class
    {
        Task<List<Exam>> GetByFileIdAsync(int fileId);
        Task<IEnumerable<T>> GetAllFun();
        Task<List<Subject>> GetAllsubbyUserFun(int userId);
        //NEW
        Task<List<Exam>> GetAllExambysubFun(int subId);
        Task<List<Exam>> GetAllExambyUserFun(int userId, int? subid);
        Task CheckDailyChallenge(int userId);
        Task UpdateUserTable(Exam exam, bool IsExamNew);
        Task UpdateDeleteExamRelatedTable(int examId);
        Task UpdateDeleteExamWithProgressRelatedTable(int examId);
        Task<T> GetByIdFun(int? id);
        Task AddFun(T entity);
        Task UpdateFun(T entity);
        Task DeleteFun(int id);

        Task<string> GetBase64ImageAsync(string profilePicturePath);


    }
}
