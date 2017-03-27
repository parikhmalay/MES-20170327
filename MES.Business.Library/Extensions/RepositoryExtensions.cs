using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Library.Extensions
{
    public static class RepositoryExtensions
    {
        public static void PresetClient(this object o, IClientIdentityInfo clientInfo)
        {
            if (o is IBusinessBase)
            {
                ((IBusinessBase)o).Preset(new BusinessPresets() { PresetClientInfo = clientInfo });
            }
            else
                throw new Exception("Unable to Cast Object to IBusinessBase. Please ensure that the Repository-Object fetched from Container is implements IBusinessBase");
        }

        public static bool Succeeded(this IResponse response)
        {
            return response.StatusCode == 200;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "b")]
        public static Exception GetEntityValidationException(this ContextBusinessBase b, System.Data.Entity.Validation.DbEntityValidationException e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var eve in e.EntityValidationErrors)
            {
                sb.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    sb.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                        ve.PropertyName, ve.ErrorMessage);
                }
            }
            return new Exception(sb.ToString());
        }
        public static Exception GetEntityValidationException(System.Data.Entity.Validation.DbEntityValidationException e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var eve in e.EntityValidationErrors)
            {
                sb.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    sb.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                        ve.PropertyName, ve.ErrorMessage);
                }
            }
            return new Exception(sb.ToString());
        }
    }



}
