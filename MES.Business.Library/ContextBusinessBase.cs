using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Business.Common.Base;
using NPE.Core.Constants;
using System.Configuration;
using NPE.Core;
using MES.Data.Library;
using MES.Business.Library.BO.UserManagement;

namespace MES.Business.Library
{
    /// <summary>
    /// The Base Class for all business implementations
    /// </summary>
    public class ContextBusinessBase : DefaultBusinessBase<MES.Data.Library.MESDataEntities, UnusedContext>, IObjectContextContainer<UnusedContext, MES.Data.Library.MESDataEntities, MES.Identity.Data.Library.IdentityContext>
    {
        public ContextBusinessBase(string repoName)
            : base(repoName)
        {

        }

        public MES.Data.Library.MESDataEntities MasterContext
        {
            get
            {
                return Get<IObjectContextAdapter>(ContextNamesConstants.MasterContextName) as MES.Data.Library.MESDataEntities;
            }
        }

        public string CDN
        {
            get
            {
                return ConfigurationManager.AppSettings["CDN"];
            }
        }

        public Identity.Data.Library.IdentityContext IdentityContext
        {
            get
            {
                return Get<IObjectContextAdapter>(ContextNamesConstants.IdentityContextName) as Identity.Data.Library.IdentityContext;
            }
        }

        public void RunOnDB(Action<MES.Data.Library.MESDataEntities> method, bool makeDbContextReadonly = false)
        {
            var context = Get<IObjectContextAdapter>(ContextNamesConstants.DataContextName) as MES.Data.Library.MESDataEntities;
            if (!makeDbContextReadonly)
                method(context);
            else
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                method(context);
                context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public System.Security.Claims.ClaimsPrincipal CurrentUserInfo
        {
            get
            {
                return (System.Threading.Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal);
            }
        }
        public MES.DTO.Library.Identity.LoginUser GetCurrentUser
        {
            get
            {
                UserManagement userObj = new UserManagement();
                MES.DTO.Library.Identity.LoginUser userInfo = userObj.GetCurrentUserInfo().Result;
                return userInfo;
            }
        }
        UnusedContext IContextContainer<UnusedContext>.MasterContext
        {
            get { throw new NotImplementedException(); }
        }

        public MESDataEntities DataContext
        {
            get { return Get<IObjectContextAdapter>(ContextNamesConstants.DataContextName) as MES.Data.Library.MESDataEntities; }
        }
    }


    public class UnusedContext : System.Data.Entity.DbContext
    {
        public UnusedContext()
        {

        }
    }

}
