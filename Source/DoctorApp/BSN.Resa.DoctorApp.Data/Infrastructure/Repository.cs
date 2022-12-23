using BSN.Resa.DoctorApp.Commons.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BSN.Resa.DoctorApp.Data.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(Expression<Func<TEntity, bool>> where, bool asReadOnly = false);

        TEntity Get(string id, bool asReadOnly = false);

        IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where, bool asReadOnly = false);

        IEnumerable<TEntity> GetAll(bool asReadOnly = false);

        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Important note:
        /// This methods supposes the entity has only a simple single property ID field, as opposed to composite primary key(in which
        /// the ID of an entity is composed of multiple properties)
        /// This method SHOULD be refactored to support entities with composite keys.
        /// </summary>
        /// <param name="entity"></param>
        void AddOrUpdate(TEntity entity);

        /// <summary>
        /// Important note:
        /// This methods supposes the entity has only a simple single property ID field, as opposed to composite primary key(in which
        /// the ID of an entity is composed of multiple properties)
        /// This method SHOULD be refactored to support entities with composite keys.
        /// </summary>
        /// <param name="entities"></param>
        void AddOrUpdateRange(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);

        int Count();

    }

    public class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public RepositoryBase(IDatabaseFactory databaseFactory)
        {
            DbSet = databaseFactory.Context.Set<TEntity>();
            Context = databaseFactory.Context;
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> where, bool asReadOnly = false)
        {
            if (!asReadOnly)
                return DbSet.FirstOrDefault(where);

            return DbSet.AsNoTracking().FirstOrDefault(where);
        }

        public virtual TEntity Get(string id, bool asReadOnly = false)
        {
            if (id.IsNullOrEmptyOrSpace())
                return null;

            var result = DbSet.Find(new object[] { id });

            if (!asReadOnly)
                return result;

            if(result != null)
                Context.Entry(result).State = EntityState.Detached;

            return result;
        }

        public virtual IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where, bool asReadOnly = false)
        {
            if(!asReadOnly)
                return DbSet.Where(where);

            return DbSet.AsNoTracking().Where(where);
        }

        public virtual IEnumerable<TEntity> GetAll(bool asReadOnly = false)
        {
            if (!asReadOnly)
                return DbSet;

            return DbSet.AsNoTracking();
        }

        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
        }

        /// <summary>
        /// Important note:
        /// This methods supposes the entity has only a simple single property ID field, as opposed to composite primary key(in which
        /// the ID of an entity is composed of multiple properties)
        /// This method SHOULD be refactored to support entities with composite keys.
        /// For more info about implementation idea visit:
        /// https://stackoverflow.com/a/44222952/5941852
        /// </summary>
        /// <param name="entity"></param>
        public void AddOrUpdate(TEntity entity)
        {
            var entityIdProperties = Context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties.Select(p => p.Name);
            var firstIdProperty = entityIdProperties.FirstOrDefault();

            if (firstIdProperty == null)
                throw new Exception("The entity has not a valid ID field");

            var typeFirstIdProperty = typeof(TEntity).GetProperty(firstIdProperty);

            if (typeFirstIdProperty == null)
                throw new Exception("The entity has not a valid ID field");

            var theIdValue = typeFirstIdProperty.GetValue(entity);

            if (theIdValue == null || theIdValue.ToString().IsNullOrEmptyOrSpace())
                throw new Exception("The entity has not a valid ID value");

            var dbEntity = Get(theIdValue.ToString());

            if (dbEntity == null)//does entity already exist in DB? if no then Add entity otherwise Update it.
            {
                Add(entity);
            }
            else
            {
                Update(entity);
            }
        }

        /// <summary>
        /// Important note:
        /// This methods supposes the entity has only a simple single property ID field, as opposed to composite primary key(in which
        /// the ID of an entity is composed of multiple properties)
        /// This method SHOULD be refactored to support entities with composite keys.
        /// </summary>
        /// <param name="entities"></param>
        public void AddOrUpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                AddOrUpdate(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public int Count()
        {
            return DbSet.Count();
        }

        protected readonly DbSet<TEntity> DbSet;

        protected readonly DoctorAppContext Context;
    }
}
