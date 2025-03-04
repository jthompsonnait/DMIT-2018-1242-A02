using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HogWildWeb.Components.Pages.SamplePages
{
    public partial class CustomerList
    {
        #region Fields
        //  The last name
        private string lastName = string.Empty;

        //  The phone number
        private string phoneNumber = string.Empty;

        //  Tells us if the search has been performed
        private bool noRecords;

        //  The feedback message
        private string feedbackMessage = string.Empty;

        //  The error message
        private string errorMessage = string.Empty;

        //  has feedback
        private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);

        //  has error
        private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);

        // error details
        private List<string> errorDetails = new();
        #endregion

        #region Properties
        //  Injects the CustomerService dependency.
        [Inject]
        protected CustomerService CustomerService { get; set; } = default!;

        //  Injects the NavigationManager dependency.
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        //  Get or sets the customers search view        
        protected List<CustomerSearchView> Customers { get; set; } = new();
        #endregion

        #region Methods
        //  search for an existing customer
        private void Search()
        {

        }

        //  new customer
        private void New()
        {

        }

        //  edit customer
        private void EditCustomer(int customerID)
        {

        }

        //  new invoice for selected customer
        private void NewInvoice(int customerID)
        {

        }
        #endregion
    }
}
