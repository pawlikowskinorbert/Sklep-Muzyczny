using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Helpers;

namespace Project.Models
{
    public class OrderListViewModel
    {
        public Pagination<Order> Pagination { get; set; }
    }
}