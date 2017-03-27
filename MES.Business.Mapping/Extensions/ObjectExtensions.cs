using AutoMapper;
using MES.Business.Mapping.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MES.Business.Mapping.Extensions
{
    public static class ObjectLibExtensions
    {
        public static List<TDestination> AutoConvert<TSource, TDestination>(this IEnumerable<TSource> sourceDomainObject, bool useStaticMapping = false)
        {
            return ObjectHelper.Convert<TSource, TDestination>(sourceDomainObject, useStaticMapping);
        }

        public static List<TDestination> AutoConvert<TSource, TDestination>(this IQueryable<TSource> sourceDomainObject, bool useStaticMapping = false)
        {
            return ObjectHelper.Convert<TSource, TDestination>(sourceDomainObject, useStaticMapping);
        }

        public static TDestination AutoConvert<TDestination>(object source)
        {
            return ObjectMapper.MapDynamic<TDestination>(source);
        }


        public static System.Globalization.CultureInfo GetCulture(this object o)
        {
            if (o != null)
                return Thread.CurrentThread.CurrentCulture;
            else
                return Thread.CurrentThread.CurrentCulture;
        }
    }
}
