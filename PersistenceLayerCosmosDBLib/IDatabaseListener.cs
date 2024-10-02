using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceLayerCosmosDBLib
{
    public interface IDatabaseListener
    {
        void SaveChangesFailed(Exception ex);
    }
}
