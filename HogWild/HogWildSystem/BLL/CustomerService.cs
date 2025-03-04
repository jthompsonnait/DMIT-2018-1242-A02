#nullable disable
using HogWildSystem.DAL;
using HogWildSystem.ViewModels;
using System;
using System.Linq;

namespace HogWildSystem.BLL
{
    public class CustomerService
    {
        //  hog wild context
        private readonly HogWildContext _hogWildContext;

        //  Constructor for the WorkingVersionsService class.
        internal CustomerService(HogWildContext hogWildContext)
        {
            //  Initialize the _hogWildContext field with the provided HogWildContext instance.
            _hogWildContext = hogWildContext;
        }

        //  get customer method
        public List<CustomerSearchView> GetCustomers(string lastName, string phone)
        {
            //  Business Rules
            //    These are processing rules that need to be satisfied
            //        for valid data

            // 	rule:	Both last name and phone number cannot be empty
            // 	rule:	RemoveFromViewFlag must be false
            if (string.IsNullOrEmpty(lastName) && string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentNullException("Please provide either a last name and/or phone number");
            }

            //	Need to update parameters so we are not searching on n empty value.
            //	Otherwise, an empty string will return all records
            if (string.IsNullOrWhiteSpace(lastName))
            {
                lastName = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrWhiteSpace(phone))
            {
                phone = Guid.NewGuid().ToString();
            }

            // return customers that meet our criteria
            return _hogWildContext.Customers
                    .Where(x => x.LastName.Contains(lastName)
                            || x.Phone.Contains(phone)
                            && !x.RemoveFromViewFlag)
                    .Select(x => new CustomerSearchView
                    {
                        CustomerID = x.CustomerID,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        City = x.City,
                        Phone = x.Phone,
                        Email = x.Email,
                        StatusID = x.StatusID,
                        TotalSales = x.Invoices.Sum(x => x.SubTotal + x.Tax)
                    })
                    .OrderBy(x => x.LastName)
                    .ToList();
        }

    }
}
