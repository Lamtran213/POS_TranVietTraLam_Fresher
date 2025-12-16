using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IAuthenticatedUser
    {
        Guid UserId { get; }
        string? Email { get; }
        IEnumerable<string> Roles { get; }
    }
}
