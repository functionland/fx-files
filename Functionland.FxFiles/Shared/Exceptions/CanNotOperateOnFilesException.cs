using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Exceptions
{
    public class CanNotOperateOnFilesException : DomainLogicException
    {
        public List<FsArtifact> FsArtifacts { get; set; }

        public CanNotOperateOnFilesException(string message, List<FsArtifact> fsArtifacts) : base(message)
        {
            FsArtifacts = fsArtifacts;
        }
    }
}
