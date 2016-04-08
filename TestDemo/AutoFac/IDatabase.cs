using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoFac
{
    public interface IDatabase
    {
        string Name { get; }

        void Select(string commandText);

        void Insert(string commandText);

        void Update(string commandText);

        void Delete(string commandText);
    }
}
