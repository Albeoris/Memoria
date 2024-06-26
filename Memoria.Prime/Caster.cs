using System;
using System.Linq.Expressions;

namespace Memoria.Prime
{
    public static class Caster<TSource, TTarget>
    {
        public static readonly Func<TSource, TTarget> Cast = UncheckedCast();

        private static Func<TSource, TTarget> UncheckedCast()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), null);
            UnaryExpression expression = Expression.Convert(parameter, typeof(TTarget));
            return Expression.Lambda<Func<TSource, TTarget>>(expression, parameter).Compile();
        }
    }
}