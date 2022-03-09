﻿using Microsoft.EntityFrameworkCore;
using Minitwit.Models.Context;
using Minitwit.Models.DTO;
using Minitwit.Models.Entity;
using Prometheus;


namespace Minitwit.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MinitwitContext _context;

        private static readonly Gauge getUserByIdTime = Metrics.CreateGauge("getuserbyid_time_s", "Time of GetUserById()");
        private static readonly Gauge getUserByUsernameTime = Metrics.CreateGauge("getuserbyusername_time_s", "Time of GetUserByUsername()");
        private static readonly Gauge getUsersTime = Metrics.CreateGauge("getusers_time_s", "Time of GetUsers()");
        private static readonly Gauge insertUserTime = Metrics.CreateGauge("insertuser_time_s", "Time of InsertUser()");
        private static readonly Gauge getUserFollowsTime = Metrics.CreateGauge("getuserfollows_time_s", "Time of GetUserFollows()");
        private static readonly Gauge getUserFollowedByTime = Metrics.CreateGauge("getuserfollowedby_time_s", "Time of GetUserFollowedBy()");
        private static readonly Gauge getFollowTime = Metrics.CreateGauge("getfollow_time_s", "Time of GetFollow()");
        private static readonly Gauge followTime = Metrics.CreateGauge("follow_time_s", "Time of Follow()");
        private static readonly Gauge unfollowTime = Metrics.CreateGauge("unfollow_time_s", "Time of Unfollow()");
        public static readonly Counter totalUsers = Metrics.CreateCounter("totalusers", "Total amount of users registerd");


        public UserRepository(MinitwitContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserById(int id)
        {
            using (getUserByIdTime.NewTimer())
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            using (getUserByUsernameTime.NewTimer())
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == username);
            }
        }

        public async Task<List<User>?> GetUsers()
        {
            using (getUsersTime.NewTimer())
            {
                return await _context.Users.ToListAsync();
            }
        }

        public async void InsertUser(User user)
        {
            using (insertUserTime.NewTimer())
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Follow>?> GetUserFollows(int id)
        {
            using (getUserFollowsTime.NewTimer())
            {
                return await _context.Users
                    .Include(u => u.Follows)
                    .Where(u => u.Id == id)
                    .SelectMany(u => u.Follows)
                    .ToListAsync();
            }
        }

        public async Task<FilteredFollowDTO?> GetFilteredFollows(string username, int limit = 100)
        {
            return await _context.Users
                .Include(u => u.Follows)
                .Where(u => u.UserName == username)
                .Select(u => new FilteredFollowDTO
                {
                    follows = u.Follows
                        .Join(
                            _context.Users,
                            f => f.FolloweeId,
                            u => u.Id,
                            (f, u) => u
                            )
                        .Select(u2 => u2.UserName)
                        .Take(limit)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<Follow>?> GetUserFollowedBy(int id)
        {
            using (getUserFollowedByTime.NewTimer())
            {
                return await _context.Users
                    .Include(u => u.FollowedBy)
                    .Where(u => u.Id == id)
                    .SelectMany(u => u.FollowedBy)
                    .ToListAsync();
            }
        }
        
        public async Task<Follow?> GetFollow(int followerId, int followeeId)
        {
            using (getFollowTime.NewTimer())
            {
                return await _context.Follows
                    .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
            }
        }

        public async Task Follow(int followerId, int followeeId)
        {
            using (followTime.NewTimer())
            {
                _context.Follows.Add(new Follow()
                {
                    FollowerId = followerId,
                    FolloweeId = followeeId,
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task Unfollow(Follow follow)
        {
            using (unfollowTime.NewTimer())
            {
                _context.Follows.Remove(follow);
                await _context.SaveChangesAsync();
            }
        }
    }
}
