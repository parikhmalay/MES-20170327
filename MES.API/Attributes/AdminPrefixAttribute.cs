using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MES.API.Attributes
{
    public class AdminPrefixAttribute : RoutePrefixAttribute, IAttributePosition
    {
        public AdminPrefixAttribute(string pref)
            : base("admin/" + pref)
        {
            this.Position = 0;
        }
        public AdminPrefixAttribute(string pref, int position)
            : base("admin/" + pref)
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