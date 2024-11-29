using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class SpecificationDetail
    {
        public int SpecificationDefinitionId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public string? Options { get; set; }
        public string? Value { get; set; }
    }
}