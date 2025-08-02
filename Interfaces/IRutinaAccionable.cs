using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    public interface IRutinaAccionable
    {
        string Describir();
    }
}


namespace AppEntrenamientoPersonal.Interfaces
{
    public interface IRutinaAccionable : IDescribible
    {
    }
}

