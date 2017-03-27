using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace MES.Business.Mapping.Helper
{
    public sealed class ObjectMapper
    {
        public static bool RegisterMap<TSource, TDestination>()
        {
            var mapCreated = Mapper.CreateMap<TSource, TDestination>();
            return (mapCreated != null);
        }

        public static IMappingExpression<TSource, TDestination> RegisterMapWithRule<TSource, TDestination>()
        {
            return Mapper.CreateMap<TSource, TDestination>();
        }

        public static bool RegisterMapWithRule<TSource, TDestination, TMember>(System.Linq.Expressions.Expression<Func<TDestination, object>> destProperty, System.Linq.Expressions.Expression<Func<TSource, TMember>> sourceInfo)
        {
            var mapCreated = Mapper.CreateMap<TSource, TDestination>().ForMember(destProperty, ops => ops.MapFrom(sourceInfo));
            return (mapCreated != null);
        }

        internal static TDestination MapStatic<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        internal static TDestination MapStatic<TSource, TDestination>(TSource source, TDestination dest)
        {
            return Mapper.Map<TSource, TDestination>(source, dest);
        }


        internal static TDestination MapDynamic<TDestination>(object source)
        {
            return Mapper.DynamicMap<TDestination>(source);
        }
    }
}
