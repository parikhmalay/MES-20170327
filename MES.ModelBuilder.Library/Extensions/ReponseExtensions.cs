using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core;

namespace MES.ModelBuilder.Library.Extensions
{
    public static class ReponseExtensions
    {
        public static bool Succeeded(this IResponse response)
        {
            return response.StatusCode == 200;
        }
    }
}
