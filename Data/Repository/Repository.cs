﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Birder.Data.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        // This method was not in the videos, but I thought it would be useful to add.
        Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }


    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        //private readonly DbContext Context;

        //public Repository(DbContext context)
        //{
        //    Context = context;
        //}
        protected readonly ApplicationDbContext _dbContext;
        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TEntity Get(int id)
        {
            // Here we are working with a DbContext, not PlutoContext. So we don't have DbSets 
            // such as Courses or Authors, and we need to use the generic Set() method to access them.
            //return Context.Set<TEntity>().Find(id);
            return _dbContext.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            // Note that here I've repeated Context.Set<TEntity>() in every method and this is causing
            // too much noise. I could get a reference to the DbSet returned from this method in the 
            // constructor and store it in a private field like _entities. This way, the implementation
            // of our methods would be cleaner:
            // 
            // _entities.ToList();
            // _entities.Where();
            // _entities.SingleOrDefault();
            // 
            // I didn't change it because I wanted the code to look like the videos. But feel free to change
            // this on your own.
            //return Context.Set<TEntity>().ToList();
            return _dbContext.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            //return Context.Set<TEntity>().Where(predicate);
            return _dbContext.Set<TEntity>().Where(predicate);
        }

        public async Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            //return Context.Set<TEntity>().SingleOrDefault(predicate);
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public void Add(TEntity entity)
        {
            //Context.Set<TEntity>().Add(entity);
            _dbContext.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            //Context.Set<TEntity>().AddRange(entities);
            _dbContext.Set<TEntity>().AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            //Context.Set<TEntity>().Remove(entity);
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            //Context.Set<TEntity>().RemoveRange(entities);
            _dbContext.Set<TEntity>().RemoveRange(entities);
        }
    }
}
