using Learntendo_backend.Data;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Learntendo_backend.Dtos;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;

namespace Learntendo_backend.Services
{
   
    public class LeagueService 
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper; 

        public LeagueService(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }


        public void ProcessMonthlyLeague()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var users = _db.User.ToList();
                string currentMonth = DateTime.Now.ToString("MMMM-yyyy", CultureInfo.InvariantCulture);
                //string currentMonth = "April-2025";
                foreach (var user in users)
                {

                    var userDto = _mapper.Map<UserDto>(user);
                    var leagueHistory = userDto.LeagueHistory ?? new Dictionary<string, string>();


                    if (userDto.MonthlyXp >= 3000)
                    {
                        userDto.CurrentLeague = "Gold";
                        userDto.Coins += 300;
                        userDto.CompleteMonthlyChallenge = true;
                    }
                    else if (userDto.MonthlyXp >= 2000)
                    {
                        userDto.CurrentLeague = "Silver";
                        userDto.Coins += 200;
                        userDto.CompleteMonthlyChallenge = true;
                    }
                    else if (userDto.MonthlyXp >= 1000)
                    {
                        userDto.CurrentLeague = "Bronze";
                        userDto.Coins += 100;
                        userDto.CompleteMonthlyChallenge = true;
                    }
                    else
                    {
                        userDto.CurrentLeague = null;
                        userDto.CompleteMonthlyChallenge = false;
                    }


                    if (userDto.CompleteMonthlyChallenge && !string.IsNullOrEmpty(userDto.CurrentLeague))
                    {
                        leagueHistory[currentMonth] = userDto.CurrentLeague;
                    }

                    userDto.MonthlyXp = 0;

                    _mapper.Map(userDto, user);
                }

                _db.SaveChanges();
            }
        }

      
    }

}

