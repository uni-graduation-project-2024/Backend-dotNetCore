using System.Linq.Expressions;
using Learntendo_backend.Dtos.Learntendo_backend.DTOs;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Learntendo_backend.Data
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        private readonly DataContext _db;
        private readonly DbSet<T> table;

        public DataRepository(DataContext db)
        {
           _db = db;
            table =_db.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllFun()
        {
            return await table.ToListAsync();
        }
        public async Task<List<Subject>> GetAllsubbyUserFun(int userId)
        {
            return await _db.Subject.Where(x=>x.UserId == userId).ToListAsync();
        }
        public async Task<List<Exam>> GetAllExambysubFun(int subId)
        {
            return await _db.Exam.Where(x => x.SubjectId == subId).ToListAsync();
        }



        public async Task<T> GetByIdFun(int id)
        {
            var result = await table.FindAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException($" id {id} not found");
            }
            return result;
            
        }

        public async Task AddFun(T entity)
        {
            await table.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateFun(T entity)
        {
            table.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteFun(int id)
        {
            var entity = await GetByIdFun(id);
            if (entity != null)
            {
                table.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
        public async Task UpdatePostExamRelatedTable(int examId)
        {
            var exam = await _db.Exam.FindAsync(examId);
            if (exam == null) return;

            var subject = await _db.Subject.FindAsync(exam.SubjectId);
            if (subject != null)
            {
                subject.NumExams += 1;
                subject.TotalQuestions += exam.NumQuestions;
                _db.Subject.Update(subject);
            }

            var user = await _db.User.FindAsync(exam.UserId);
            if (user != null)
            {
                user.TotalQuestion += exam.NumQuestions;
                if (exam.XpCollected > 0)
                {
                    user.TotalXp += exam.XpCollected;
                }

              
                await CheckDailyChallenge(user.UserId);

                _db.User.Update(user);
            }

            await _db.SaveChangesAsync();
        }

        public async Task UpdateDeleteExamRelatedTable(int examId)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    var exam = await _db.Exam.FindAsync(examId);
                    if (exam == null) return;

                    var subject = await _db.Subject.FindAsync(exam.SubjectId);
                    if (subject != null)
                    {
                        subject.NumExams -= 1;
                        subject.TotalQuestions -= exam.NumQuestions;
                        _db.Subject.Update(subject);
                    }

                    var user = await _db.User.FindAsync(exam.UserId);
                    if (user != null)
                    {
                        user.TotalQuestion -= exam.NumQuestions;
                        if (exam.XpCollected > 0)
                        {
                            user.TotalXp -= exam.XpCollected;
                        }

                        if (exam.CreatedDate == DateTime.UtcNow.Date)
                        {
                            user.DailyXp -= exam.XpCollected;
                            user.NumQuestionSolToday -= exam.NumQuestions;

                            int examCountToday = await _db.Exam
                                .CountAsync(e => e.UserId == exam.UserId && e.CreatedDate.Date == DateTime.UtcNow.Date);

                            if (examCountToday == 1)
                            {
                                user.CompleteDailyChallenge = false;
                                user.DateCompleteDailyChallenge = null;
                                user.DailyXp = 0;
                                user.NumQuestionSolToday = 0;
                            }
                        }

                        _db.User.Update(user);
                    }

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();  
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();  
                    throw; 
                }
            }
        }

        public async Task CheckDailyChallenge(int userId)
        {
            var user = await _db.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return;

            var today = DateTime.UtcNow.Date;

            
            bool hasExamToday = await _db.Exam
                .AnyAsync(e => e.UserId == userId && e.CreatedDate.Date == today && e.XpCollected > 0);

            if (hasExamToday)
            {
                user.CompleteDailyChallenge = true;
                user.DateCompleteDailyChallenge = DateTime.UtcNow;

           
                user.DailyXp = await _db.Exam
                    .Where(e => e.UserId == userId && e.CreatedDate.Date == today && e.XpCollected > 0 && e.NumQuestions > 0)
                    .SumAsync(e => e.XpCollected);

                user.NumQuestionSolToday = await _db.Exam
                    .Where(e => e.UserId == userId && e.CreatedDate.Date == today && e.XpCollected > 0 && e.NumQuestions > 0)
                    .SumAsync(e => e.NumQuestions);
            }
            else if (user.DateCompleteDailyChallenge?.Date != today)
            {
                user.CompleteDailyChallenge = false;
                user.DailyXp = 0;
                user.NumQuestionSolToday = 0;
            }

            await _db.SaveChangesAsync();
        }



    }

}
