using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProvider.Enums
{
    public enum Gender : int
    {
        [Description("Male")]
        Male = 1,
        [Description("Female")]
        Female = 2
    }
}
