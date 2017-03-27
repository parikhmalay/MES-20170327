using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MES.API.Attributes
{
    public class UIPublicUserPrefixAttribute : RoutePrefixAttribute, IAttributePosition
    {
        public UIPublicUserPrefixAttribute(string pref)
            : base("public-ui/" + pref)
        {
            this.Position = 0;

        }

        public UIPublicUserPrefixAttribute(string pref, int position)
            : base("public-ui/" + pref)
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