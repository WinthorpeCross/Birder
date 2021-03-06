﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Linq;

namespace Birder.Helpers
{
    public static class PagingExtension
    {
        // var paged = _birdRepository.GetBirdSummaryList(BirderStatus.Common).GetPaged(1, 5);
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query,
                                         int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        // var viewModel = _birdRepository.GetBirdSummaryList(BirderStatus.Common).GetPaged<Bird, BirdSummaryViewModel>(1, 5, _mapper); 
        public static PagedResult<U> GetPaged<T, U>(this IQueryable<T> query,
                                            int page, int pageSize, IMapper _mapper) where U : class
        {
            var result = new PagedResult<U>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip)
                                  .Take(pageSize)
                                  .ProjectTo<U>(_mapper.ConfigurationProvider)
                                  .ToList();

            return result;
        }
    }
}
