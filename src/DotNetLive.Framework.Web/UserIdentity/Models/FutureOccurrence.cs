using System;

namespace DotNetLive.Framework.Web.UserIdentity.Models
{
    public class FutureOccurrence : Occurrence
    {
        public FutureOccurrence() : base()
        {
        }

        public FutureOccurrence(DateTime willOccurOn) : base(willOccurOn)
        {
        }
    }
}
