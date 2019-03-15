﻿using Birder.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birder.Data.Repository
{
    public class BirdRepository : IBirdRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BirdRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Bird> GetBirdDetail(int id)
        {
            return (from b in _dbContext.Birds
                    .Include(cs => cs.BirdConserverationStatus)
                    where (b.BirdId == id)
                    select b).FirstOrDefaultAsync();
        }

        public Task<List<Bird>> GetBirdSummaryList(BirderStatus birderStatusFilter)
        {
            return (from b in _dbContext.Birds
                    .Include(cs => cs.BirdConserverationStatus)
                    where (b.BirderStatus == birderStatusFilter)
                    select b).ToListAsync();
        }
    }
}
