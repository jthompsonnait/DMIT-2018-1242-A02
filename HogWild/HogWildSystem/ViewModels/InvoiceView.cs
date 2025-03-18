using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.ViewModels
{
    public class InvoiceView
    {
        public int InvoiceID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total => SubTotal + Tax;
        public List<InvoiceLineView> InvoiceLines { get; set; } = new();
        public bool RemoveFromViewFlag { get; set; }
    }
}
