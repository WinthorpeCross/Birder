﻿using Birder.Data;
using Birder.Data.Model;
using Birder.Helpers;
using Birder.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Birder.Services
{
    public interface IListService
    {
        Task<IEnumerable<LifeListViewModel>> GetLifeListsAsync(Expression<Func<Observation, bool>> predicate);
        Task<TopObservationsAnalysisViewModel> GetTopObservationsAsync(string username, DateTime startDate);
    }




    public class ListService : IListService
    {
        private readonly ApplicationDbContext _dbContext;
        public ListService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<LifeListViewModel>> GetLifeListsAsync(Expression<Func<Observation, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentException("The argument is null or empty", nameof(predicate));

            var query = await _dbContext.Observations
                     .AsNoTracking()
                     .Where(predicate)
                     .GroupBy(b => new { b.BirdId, b.Bird.EnglishName, b.Bird.Species, b.Bird.PopulationSize, b.Bird.BtoStatusInBritain, b.Bird.BirdConservationStatus.ConservationList, b.Bird.BirdConservationStatus.ConservationListColourCode })
                     .Select(b => new LifeListViewModel
                     {
                         BirdId = b.Key.BirdId,
                         EnglishName = b.Key.EnglishName,
                         Species = b.Key.Species,
                         PopulationSize = b.Key.PopulationSize,
                         BtoStatusInBritain = b.Key.BtoStatusInBritain,
                         ConservationStatus = b.Key.ConservationList,
                         ConservationListColourCode = b.Key.ConservationListColourCode,
                         Count = b.Count()
                     })
                     .ToListAsync();

            return query;
        }

        public async Task<TopObservationsAnalysisViewModel> GetTopObservationsAsync(string username, DateTime startDate)
        {
            //if (predicate is null)
            //    throw new ArgumentException("The argument is null or empty", nameof(predicate));

            var viewModel = new TopObservationsAnalysisViewModel();

            viewModel.TopObservations = await _dbContext.Observations
                     .AsNoTracking()
                     .Where(a => a.ApplicationUser.UserName == username)
                     .GroupBy(b => new { b.Bird.EnglishName })
                     .Select(b => new TopObservationsViewModel
                     {
                         Name = b.Key.EnglishName,
                         Count = b.Count()
                     })
                     .Take(5)
                     .ToListAsync();

            viewModel.TopMonthlyObservations = await _dbContext.Observations
                     .AsNoTracking()
                     .Where(a => a.ApplicationUser.UserName == username && a.ObservationDateTime >= startDate)
                     .GroupBy(b => new { b.Bird.EnglishName })
                     .Select(b => new TopObservationsViewModel
                     {
                         Name = b.Key.EnglishName,
                         Count = b.Count()
                     })
                     .Take(5)
                     .ToListAsync();

            return viewModel;


            //        viewModel.TopObservations = observations
            //.GroupBy(n => n.BirdId)
            //.Select(n => new TopObservationsViewModel
            //{
            //    BirdId = n.FirstOrDefault().BirdId,
            //    Name = n.FirstOrDefault().Bird?.EnglishName,
            //    Count = n.Count()
            //})
            //.OrderByDescending(n => n.Count).Take(5);

            //viewModel.TopMonthlyObservations = observations
            //    .Where(o => o.ObservationDateTime >= startDate)
            //    .GroupBy(n => n.BirdId)
            //    .Select(n => new TopObservationsViewModel
            //    {
            //        BirdId = n.FirstOrDefault().BirdId,
            //        Name = n.FirstOrDefault().Bird?.EnglishName,
            //        Count = n.Count()
            //    })
            //    .OrderByDescending(n => n.Count).Take(5);
        }
    }
}
