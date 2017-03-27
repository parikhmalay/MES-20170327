using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MES.API.Attributes
{
    public class PublicUserPrefixAttribute : RoutePrefixAttribute, IAttributePosition
    {
        public PublicUserPrefixAttribute(string pref)
            : base("public/" + pref)
        {
            this.Position = 0;

        }

        public PublicUserPrefixAttribute(string pref, int position)
            : base("public/" + pref)
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