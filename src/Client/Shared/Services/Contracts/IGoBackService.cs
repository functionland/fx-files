using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public class IGoBackService
    {
        public Func<Task>? GoBackAsync { get; set; }

    }
}
