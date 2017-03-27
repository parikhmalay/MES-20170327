//using NPE.DTO.Library.Base.Messaging.Resources;

using MES.DTO.Library.Base.Messaging.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Account.DTO.Library
{
    public static class Languages
    {
        public static string GetResourceText(string resourceKey)
        {
            return DTOMessageResources.ResourceManager.GetString(resourceKey, Thread.CurrentThread.CurrentCulture);   
        }
    }
}