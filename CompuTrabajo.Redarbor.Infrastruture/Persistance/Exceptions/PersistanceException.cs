using System;
using System.Collections.Generic;
using System.Text;

namespace CompuTrabajo.Redarbor.Infrastruture.Persistance.Exceptions
{
    public class PersistanceException : Exception
    {
        public PersistanceException(string message) : base(message)
        {
            
        }
    }
}
