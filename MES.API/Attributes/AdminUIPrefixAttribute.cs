using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MES.API.Attributes
{
    public class UIAdminPrefixAttribute : RoutePrefixAttribute, IAttributePosition
    {
        public UIAdminPrefixAttribute(string pref)
            : base("admin-ui/" + pref)
        {
            this.Position = 0;
        }
        public UIAdminPrefixAttribute(string pref, int position)
            : base("admin-ui/" + pref)
        {
            this.Position = position;
        }

        public int Position
        {
            get;
            set;
        }
    }
}