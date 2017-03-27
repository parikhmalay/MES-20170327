using MES.DTO.Library.UserManagement;
using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.UserManagement
{
    public interface IPreferencesRepository : ICrudMethods<Preferences, int?, string, Preferences, int, bool?, string, Preferences>
    {

    }
}

