using System;
using System.Linq.Expressions;

namespace Nike.Framework.Domain.Specifications
{
    public class SpecificationBuilder<T>
    {
        private readonly RelationalSpecification<T> _relationalSpecification = new RelationalSpecification<T>();
        public static RelationalSpecification<T> Instance => new RelationalSpecification<T>();

        public SpecificationBuilder<T> AddInclude(Expression<Func<T, object>> include)
        {
            _relationalSpecification.AddInclude(include);
            return this;
        }

        public SpecificationBuilder<T> AddInclude(string include)
        {
            _relationalSpecification.AddInclude(include);
            return this;
        }

        public SpecificationBuilder<T> ApplyOrderBy(Expression<Func<T, object>> orderBy)
        {
            _relationalSpecification.ApplyOrderBy(orderBy);
            return this;
        }

        public SpecificationBuilder<T> ApplyOrderByDescending(Expression<Func<T, object>> orderByDescending)
        {
            _relationalSpecification.ApplyOrderByDescending(orderByDescending);
            return this;
        }

        public SpecificationBuilder<T> ApplySkipTake(int skip, int take)
        {
            _relationalSpecification.ApplySkipTake(skip, take);
            return this;
        }

        public SpecificationBuilder<T> ApplyCriteria(Expression<Func<T, bool>> criteria)
        {
            _relationalSpecification.ApplyCriteria(criteria);
            return this;
        }

        public SpecificationBuilder<T> DisableTracking()
        {
            _relationalSpecification.DisableTracking();
            return this;
        }

        public RelationalSpecification<T> Build()
        {
            return _relationalSpecification;
        }
    }
}