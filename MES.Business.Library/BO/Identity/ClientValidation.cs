using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Library.BO.Identity
{
    
    /// <summary>
    /// The class implementation is required by framework since it is designed for multi tenant setup. for MES we will keep this unimplemented.
    /// </summary>
    public class ClientValidation : ContextBusinessBase, IClientValidation
    {
        public ClientValidation()
            : base("ClientValidation")
        {}

        public IClientAPIConsumer GetAPIConsumerInfo(string consumerId)
        {
            throw new NotImplementedException();
        }

        public IClientIdentityInfo GetClientInfo(string productUrl, string productCode)
        {
            throw new NotImplementedException();
        }

        public IClientIdentityInfo GetClientInfo(int clientId, bool createClientIfRequired)
        {
            throw new NotImplementedException();
        }

        public List<IClientIdentityInfo> GetClients(string productCode)
        {
            throw new NotImplementedException();
        }

        public int GetContactIdentifier(string userId, int clientId)
        {
            throw new NotImplementedException();
        }

        public IClientIdentityInfo GetCurrent()
        {
            return null;
        }

        public IApplicationUser ValidatePublicUser(int clientId, string userName, string password, bool allowNonPublicUsers)
        {
            throw new NotImplementedException();
        }
    }
}
