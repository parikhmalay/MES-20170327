using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.UserManagement
{
    public class Preferences : CreateEditPropBase
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public Nullable<int> PagesizeId { get; set; }
        public Nullable<int> DefaultLandingPageId { get; set; }

        public string DefaultController { get; set; }
        public string DefaultAction { get; set; }
        public string AssignmentId { get; set; }
        public string CulturePreference { get; set; }
        public string DefaultLandingPagePath { get; set; }
    }
}
