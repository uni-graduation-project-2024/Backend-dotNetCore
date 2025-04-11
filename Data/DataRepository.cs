﻿using System;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using Learntendo_backend.Dtos.Learntendo_backend.DTOs;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Learntendo_backend.Data
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        private readonly DataContext _db;
        private readonly DbSet<T> table;
        private object _logger;

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

       //NEW
        public async Task<List<Exam>> GetAllExambyUserFun(int userId, int? subId= null)
        {
            var query = _db.Exam.Where(x => x.UserId == userId);

            if (subId.HasValue)
            {
                if (subId.Value == -1)
                {
                    query = query.Where(x => x.SubjectId == null);
                }
                else
                {
                    query = query.Where(x => x.SubjectId == subId.Value);
                }
            }

            return await query.ToListAsync();
        }



        public async Task<T> GetByIdFun(int? id)
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
            /// The subjectId of any newly created Exam is null
            /// Incorrect logic, Retrying any exam again shouldn't update any subject releated fields

            //if (exam.SubjectId != null)
            //{
            //    var subject = await _db.Subject.FindAsync(exam.SubjectId);
            //    if (subject != null)
            //    {
            //        subject.NumExams += 1;
            //        subject.TotalQuestions += exam.NumQuestions;
            //        _db.Subject.Update(subject);
            //    }

            //}
            //else
            //{
            //    exam.SubjectId = null;
            //}
            var user = await _db.User.FindAsync(exam.UserId);
            if (user != null)
            {
                user.TotalQuestion += exam.NumQuestions;
                if (exam.XpCollected > 0)
                {
                    user.TotalXp += exam.XpCollected;
                    /////////////////////////////////////////
                    user.WeeklyXp += exam.XpCollected;
                }

                await CheckDailyChallenge(user.UserId);

                _db.User.Update(user);
            }

            await _db.SaveChangesAsync();
        }
        public async Task UpdateDeleteExamRelatedTable(int examId)
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
            await _db.SaveChangesAsync();
        }
        public async Task UpdateDeleteExamWithProgressRelatedTable(int examId)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {

                    await UpdateDeleteExamRelatedTable(examId);
                    var exam = await _db.Exam.FindAsync(examId);
                    if (exam == null) return;
                    var user = await _db.User.FindAsync(exam.UserId);
                    if (user != null)
                    {
                        user.TotalQuestion -= exam.NumQuestions;
                        if (exam.XpCollected > 0)
                        {
                            user.TotalXp -= exam.XpCollected;
                            /////////////////////////////////////////
                            user.WeeklyXp += exam.XpCollected;
                        }

                        if (exam.CreatedDate.Date == DateTime.UtcNow.Date)
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
                                user.StreakScore -= 1;
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

            var oldLastExamDate = user.LastExamDate;
            // جلب أحدث امتحان للمستخدم
            var latestExam = await _db.Exam
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedDate)
                .FirstOrDefaultAsync();

            // تحديث تاريخ آخر امتحان إذا كان هناك امتحان جديد اليوم
            if (latestExam != null && latestExam.CreatedDate.Date == today)
            {
                user.LastExamDate = latestExam.CreatedDate; // تحديث التاريخ إلى أحدث امتحان
            }

            bool hasExamToday = await _db.Exam
                .AnyAsync(e => e.UserId == userId && e.CreatedDate.Date == today && e.XpCollected > 0);

            if (hasExamToday)
            {
               // user.CompleteDailyChallenge = true;
                user.DateCompleteDailyChallenge = today;

                user.DailyXp = await _db.Exam
                    .Where(e => e.UserId == userId && e.CreatedDate.Date == today && e.XpCollected > 0 && e.NumQuestions > 0)
                    .SumAsync(e => e.XpCollected);
                /////////////////////////////////////////
                user.WeeklyXp = await _db.Exam
                   .Where(e => e.UserId == userId && e.CreatedDate.Date == today && e.XpCollected > 0 && e.NumQuestions > 0)
                   .SumAsync(e => e.XpCollected);
                user.MonthlyXp = await _db.Exam
                   .Where(e => e.UserId == userId && e.CreatedDate.Date == today && e.XpCollected > 0 && e.NumQuestions > 0)
                   .SumAsync(e => e.XpCollected);
                ///////////////////////////////////////////////
                user.NumQuestionSolToday = await _db.Exam
                    .Where(e => e.UserId == userId && e.CreatedDate.Date == today && e.XpCollected > 0 && e.NumQuestions > 0)
                    .SumAsync(e => e.NumQuestions);

                if (!user.CompleteDailyChallenge && user.DailyXp >= 50) // if user did not complete daily challenge && user dailyXp > 50
                {
                    user.Coins += 5;
                    user.CompleteDailyChallenge = true;
                }
            }
            else
            {

                //if user streakscore is equal to zero no need to decrease the user freezeStreak
                if (user.StreakScore != 0 && user.FreezeStreak > 0) // if user streak score not equal zero && freezeStreak > 0
                {
                    user.FreezeStreak -= 1;
                }
                else
                {
                    user.StreakScore = 0;
                }

            }

            // تحديث Streak Score
            var existexam = await _db.Exam.CountAsync(e => e.UserId == userId && e.CreatedDate.Date == today && e.XpCollected > 0);

            if (existexam == 1 && (oldLastExamDate == null || oldLastExamDate?.Date != today))
            {
                user.StreakScore += 1;
                user.LastExamDate = today; // تحديث تاريخ آخر زيادة لـ StreakScore
            }


            await _db.SaveChangesAsync();
        }

        public Task UpdatePostExamRelatedTable(object examId)
        {
            throw new NotImplementedException();
        }
        public async Task CheckDailyChallengeForAllUsers()
        {
            var users = await _db.User.Select(u => u.UserId).ToListAsync();

            foreach (var userId in users)
            {
                await CheckDailyChallenge(userId);
            }

            await _db.SaveChangesAsync(); // حفظ التغييرات بعد تعديل جميع المستخدمين
        }


    }
}



