using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MES.Business.Mapping.Helper
{
    /// <summary>
    /// Helper class to convert object from/to domain to DTO
    /// </summary>
    public sealed class ObjectHelper
    {


        public static TDestination Convert<TSource, TDestination>(TSource sourceDomainObject, bool useStaticMapping = true)
        {
            if (sourceDomainObject == null)
                return default(TDestination);

            if (!useStaticMapping)
                return ObjectMapper.MapDynamic<TDestination>(sourceDomainObject);
            else
                return ObjectMapper.MapStatic<TSource, TDestination>(sourceDomainObject);
        }

        public static TDestination Convert<TSource, TDestination>(TSource sourceDomainObject, TDestination destinationObject, bool useStaticMapping = true)
        {
            if (sourceDomainObject == null)
                return default(TDestination);

            if (!useStaticMapping)
                return ObjectMapper.MapDynamic<TDestination>(sourceDomainObject);
            else
                return ObjectMapper.MapStatic<TSource, TDestination>(sourceDomainObject, destinationObject);
        }



        public static List<TDestination> Convert<TSource, TDestination>(IEnumerable<TSource> sourceDomainObject, bool useStaticMapping = true)
        {

            if (!useStaticMapping)
                return sourceDomainObject.Select(item => Convert<TSource, TDestination>(item, useStaticMapping)).ToList();
            else
                return ObjectMapper.MapStatic<IEnumerable<TSource>, List<TDestination>>(sourceDomainObject);
        }

        public static IEnumerable<TDestination> ConvertEnumerable<TSource, TDestination>(IEnumerable<TSource> sourceDomainObject, bool useStaticMapping = true)
        {

            if (!useStaticMapping)
                return sourceDomainObject.Select(item => Convert<TSource, TDestination>(item, useStaticMapping)).ToList();
            else
                return ObjectMapper.MapStatic<IEnumerable<TSource>, IEnumerable<TDestination>>(sourceDomainObject);
        }

        public static List<TDestination> Convert<TSource, TDestination>(IEnumerable<TSource> sourceDomainObject, List<TDestination> destinationObject, bool useStaticMapping = true)
        {
            if (!useStaticMapping)
                return sourceDomainObject.Select(item => Convert<TSource, TDestination>(item, useStaticMapping)).ToList();
            else
                return ObjectMapper.MapStatic<IEnumerable<TSource>, List<TDestination>>(sourceDomainObject, destinationObject);
        }
       

        public static List<TDestination> Convert<TSource, TDestination>(IQueryable<TSource> sourceDomainObject, bool useStaticMapping = true)
        {
            return Convert<TSource, TDestination>(sourceDomainObject.AsEnumerable(), useStaticMapping);
        }

        public static List<TDestination> Convert<TSource, TDestination>(IList<TSource> sourceDomainObject, bool useStaticMapping = true)
        {
            if (!useStaticMapping)
                return sourceDomainObject.Select(item => Convert<TSource, TDestination>(item, useStaticMapping)).ToList();
            else
                return ObjectMapper.MapStatic<IList<TSource>, List<TDestination>>(sourceDomainObject);
        }

        public static List<TDestination> Convert<TSource, TDestination>(System.Data.IDataReader sourceDomainObject, bool useStaticMapping = true)
        {
            if (!useStaticMapping)
                return ObjectMapper.MapDynamic<List<TDestination>>(sourceDomainObject);
            else
                return ObjectMapper.MapStatic<System.Data.IDataReader, List<TDestination>>(sourceDomainObject);
        }
    }
}
