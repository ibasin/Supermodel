using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using ReflectionMapper;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;
using Supermodel.DDD.Models.View.Mvc.UIComponents;
using Supermodel.DDD.UnitOfWork;

namespace Supermodel.DDD.Repository 
{
    public class SqlLinqEFSimpleRepo<EntityT> : ISqlLinqEFDataRepo<EntityT> where EntityT : class, IEntity, new()
    {
		#region Properties
        public virtual DbSet<EntityT> DbSet
        {
            get { return DbContextAmbientContextCore.CurrentDbContext.Set<EntityT>(); }
		}
        public virtual IQueryable<EntityT> Items
        {
            get { return DbSet; }
        }
        #endregion

		#region Methods
        public void Add(EntityT item)
        {
            if (item.Id == 0) DbSet.Add(item);
        }
        public void Delete(EntityT item)
        {
            DbSet.Remove(item);
        }

        public virtual IEntity GetIEntityById(int id)
        {
            return GetById(id);
        }

        public virtual EntityT GetById(int id)
        {
            var item = DbSet.SingleOrDefault(p => p.Id == id);
            if (item == null) throw new SupermodelException("GetById can't find an entity");
            return item;
		}

        public virtual IEnumerable<IEntity> GetIEntityAll()
        {
            return GetAll();
        }

        public virtual IEnumerable<EntityT> GetAll()
		{
            return Items.ToList();
		}

        public EntityT GetSingle()
        {
            var item = Items.SingleOrDefault();
            if (item == null) throw new SupermodelException("GetSingle can't find any entity");
            return item;
        }

        /// <summary>
        /// Returns an entity satisfying <paramref name="predicatesLambda"/>, creating one with the required
        /// values set if none exists.  A created entity is added to the unit of work.
        /// </summary>
        /// <param name="predicatesLambda">
        /// A lambda of the form <code>entity => entity.Member1 == CONSTANT && entity.Member2 == CONSTANT</code>
        /// </param>
        public EntityT GetOrCreate(Expression<Func<EntityT, bool>> predicatesLambda)
        {
            var entity = this.Items.SingleOrDefault(predicatesLambda);
            if (entity == null)
            {
                entity = (EntityT)new EntityT().ConstructVirtualProperties();

                var remainingPredicates = predicatesLambda.Body;
                while (remainingPredicates is BinaryExpression && remainingPredicates.NodeType == ExpressionType.AndAlso)
                {
                    var andAlso = (BinaryExpression)remainingPredicates;
                    var currentPredicate = andAlso.Left;
                    updateObjectFromPredicate(entity, currentPredicate);
                    remainingPredicates = andAlso.Right;
                }
                // remainingPredicates should contain a single predicate because it is no longer an AndAlso binary expression.
                this.updateObjectFromPredicate(entity, remainingPredicates);

                entity.Add();
            }
            return entity;
        }

        private void updateObjectFromPredicate(object entity, Expression predicate)
        {
            switch (predicate.NodeType)
            {
                case ExpressionType.Equal:
                    var equal = (BinaryExpression)predicate;
                    var memberExpression = equal.Left as MemberExpression;
                    if (memberExpression == null)
                    {
                        throw new InvalidOperationException("The left side of a predicate must be a member expression");
                    }

                    object value;
                    switch (equal.Right.NodeType)
                    {
                        case ExpressionType.Constant:
                            var constantExpression = (ConstantExpression)equal.Right;
                            value = constantExpression.Value;
                            break;

                        case ExpressionType.MemberAccess:
                            // http://stackoverflow.com/a/2616959/39396
                            var memberAccesss = (MemberExpression)equal.Right;
                            // Compiler has generated a closure class for the member access
                            // First get the expression where the compiler is accessing the closure's enclosed member-owner
                            var closureAccess = (MemberExpression)memberAccesss.Expression;
                            // Then get the closure itself (not sure why it is a constant expression, but it is.  I guess the C# specification would explain why.)
                            var closure = (ConstantExpression)closureAccess.Expression;
                            // Applying the closureAccess to the closure yields the enclosed member-owner, which actually owns the value we're interested in.
                            var memberOwner = ((FieldInfo)closureAccess.Member).GetValue(closure.Value);
                            // Applying the memberAccess to the memberOwner yields the value.
                            value = ((PropertyInfo)memberAccesss.Member).GetValue(memberOwner, null);
                            // This technique has not been tested against multiple levels of member access, e.g., x => x.Name == Grandparent.Parent.Name.
                            break;

                        default:
                            throw new InvalidOperationException(String.Format("{0} is not supported on the right side of a predicate.", equal.Right.NodeType));
                    }

                    var memberName = memberExpression.Member.Name;
                    entity.PropertySet(memberName, value);
                    break;

                default:
                    throw new InvalidOperationException("Only equality predicates AndAlso'd together are supported (e.g.: x => x.Name == other.Name && x.Value == other.Value).");
            }
        }

        public virtual List<DropdownMvcModel.Option> GetDropdownOptions<MvcModelT>() where MvcModelT : MvcModelForEntityCore
        {
            var entities = GetAll();
            var mvcModels = new List<MvcModelT>();
            mvcModels = (List<MvcModelT>)mvcModels.MapFromObject(entities);
            mvcModels = mvcModels.OrderBy(p => p.Label).ToList();
            return !mvcModels.Any() ? 
                new List<DropdownMvcModel.Option>() : 
                mvcModels.Select(item => new DropdownMvcModel.Option(item.Id.ToString(CultureInfo.InvariantCulture), item.Label, item.IsDisabled)).ToList();
        }
        public virtual List<MultiSelectMvcModelCore.Option> GetMultiSelectOptions<MvcModelT>() where MvcModelT : MvcModelForEntityCore
        {
            var entities = GetAll();
            var mvcModels = new List<MvcModelT>();
            mvcModels = (List<MvcModelT>)mvcModels.MapFromObject(entities);
            mvcModels = mvcModels.OrderBy(p => p.Label).ToList();
            return !mvcModels.Any() ?
                new List<MultiSelectMvcModelCore.Option>() :
                mvcModels.Select(item => new MultiSelectMvcModelCore.Option(item.Id.ToString(CultureInfo.InvariantCulture), item.Label, item.IsDisabled)).ToList();
        }
        #endregion
    }
}
