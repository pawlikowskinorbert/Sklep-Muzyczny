using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Interfaces
{
    public interface IPagination
    {
        int PageIndex {get;}
        int PageSize {get;}
        int Count {get;}
    }
}