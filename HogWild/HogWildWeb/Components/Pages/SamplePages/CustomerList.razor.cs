using Microsoft.AspNetCore.Components;
using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using HogWildWeb.Components;

namespace HogWildWeb.Components.Pages.SamplePages
{
    public partial class CustomerList
    {
        #region Fields

        // The last name
        private string lastName = string.Empty;

        // The phone number
        private string phoneNumber = string.Empty;

        // Tells us if the search has been performed
        private bool noRecords;

        // The feedback message
        private string feedbackMessage = string.Empty;

        // The error message
        private string errorMessage = string.Empty;

        // has feedback
        private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);

        // has error
        private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);

        // error details
        private List<string> errorDetails = new();
        #endregion

        #region Properties
        // Injects the CustomerService dependency.
        [Inject]
        protected CustomerService CustomerService { get; set; } = default!;

        // Injects the NavigationManager dependency.
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        // Gets or sets the customers search view.
        protected List<CustomerSearchView> Customers { get; set; } = new();
        #endregion

        #region Methods
        //  search for an existing customer
        private void Search()
        {
            try
            {
                // reset the no records flag
                noRecords = false;

                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;

                //  clear the customer list before we do our search
                Customers.Clear();

                if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phoneNumber))
                {
                    throw new ArgumentException("Please provide either a last name and/or phone number");
                }

                //  search for our customers

                Customers = CustomerService.GetCustomers(lastName, phoneNumber);
                if (Customers.Count > 0)
                {
                    feedbackMessage = "Search for customer(s) was successful";
                }
                else
                {
                    feedbackMessage = "No customers were found for your search criteria";
                    noRecords = true;
                }
            }
            catch (ArgumentNullException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch (ArgumentException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch (AggregateException ex)
            {
                //  have a collection of errors
                //  each error should be place into a separate line
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"{errorMessage}{Environment.NewLine}";
                }
                errorMessage = $"{errorMessage}Unable to search for customer";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        //  new customer
        private void New()
        {
            NavigationManager.NavigateTo("/SamplePages/CustomerEdit/0");
        }

        //  edit selected customer
        private void EditCustomer(int customerID)
        {
            NavigationManager.NavigateTo($"/SamplePages/CustomerEdit/{customerID}");
        }

        //  new invoice for selected customer
        private void NewInvoice(int customerID)
        {

        }

        #endregion
    }
}
