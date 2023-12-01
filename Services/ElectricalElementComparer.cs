using Semi_automatic_trace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semi_automatic_trace.Services
{
    public class ReportComparer : IEqualityComparer<ElectricalElement>
    {
        public bool Equals(ElectricalElement x, ElectricalElement y)
        {
            bool equals = false;
            if (x.MainElement.Id.IntegerValue == y.MainElement.Id.IntegerValue)
            {
                equals = true;
            }
            return equals;
        }

        public int GetHashCode(ElectricalElement obj)
        {
            int hash = obj.MainElement.Id.IntegerValue.GetHashCode();
            return hash;
        }
    }
}
