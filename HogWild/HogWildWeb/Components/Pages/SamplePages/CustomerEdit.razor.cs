using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace HogWildWeb.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
        #region Fields
        // The customer
        private CustomerEditView customer = new();
        //  The provinces
        private List<LookupView> provinces = new();
        //  The countries
        private List<LookupView> countries = new();
        //  The status lookup
        private List<LookupView> statusLookup = new();
        private List<InvoiceView> invoices = new List<InvoiceView>();
        //  mudform control
        private MudForm customerForm = new();
        #endregion
        #region Feedback & Error Messages
        // The feedback message
        private string feedbackMessage = string.Empty;

        // The error message
        private string? errorMessage = string.Empty;

        // has feedback
        private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);

        // has error
        private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);

        // error details
        private List<string> errorDetails = new();
        #endregion

        #region Validation
        // flag to if the form is valid.
        private bool isFormValid;
        //  flag if data has change
        private bool hasDataChanged = false;
        //  set text for cancel/close button
        private string closeButtonText => hasDataChanged ? "Cancel" : "Close";
        #endregion

        #region Properties
        //  The customer service
        [Inject] protected CustomerService CustomerService { get; set; } = default!;

        //  Category/lookup service
        [Inject] protected CategoryLookupService CategoryLookupService { get; set; } = default!;

        //  The invoice service
        [Inject] protected InvoiceService InvoiceService { get; set; } = default!;

        //   Injects the NavigationManager dependency
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

        //   Injects the DialogService dependency
        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

        //  Customer ID used to create or edit a customer
        [Parameter] public int CustomerID { get; set; } = 0;
        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            try
            {
                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;
                //  check to see if we are navigating using a valid customer CustomerID.
                //      or are we going to create a new customer.
                if (CustomerID > 0)
                {
                    customer = CustomerService.GetCustomer(CustomerID) ?? new();
                    invoices = InvoiceService.GetCustomerInvoices(CustomerID);
                }

                // lookups
                provinces = CategoryLookupService.GetLookups("Province");
                countries = CategoryLookupService.GetLookups("Country");
                statusLookup = CategoryLookupService.GetLookups("Customer Status");

                StateHasChanged();
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

        // save the customer
        private void Save()
        {
            //  reset the error detail list
            errorDetails.Clear();

            //  reset the error message to an empty string
            errorMessage = string.Empty;

            //  reset feedback message to an empty string
            feedbackMessage = String.Empty;
            try
            {
                customer = CustomerService.AddEditCustomer(customer);
                feedbackMessage = "Data was successfully saved!";

                //Reset change tracking
                hasDataChanged = false;
                isFormValid = false;
                customerForm.ResetTouched(); // reset the touched
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
                errorMessage = $"{errorMessage}Unable to save the customer";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        //  Cancels/closes this instance.
        private async Task Cancel()
        {
            if (hasDataChanged)
            {
                bool? results = await DialogService.ShowMessageBox("Confirm Cancel",
                    $"Do you wish to close the customer editor? All unsaved changes will be lost.",
                    yesText: "Yes", cancelText: "No");

                //  true means affirmative action (e.g., "Yes").
                //  null means the user dismissed the dialog(e.g., clicking "No" or closing the dialog).
                if (results == null)
                {
                    return;
                }
            }
            NavigationManager.NavigateTo("/SamplePages/CustomerList");
        }

        /// New invoice.
        private void NewInvoice()
        {
            //  NOTE:   we will hard code employee ID (1)            
            NavigationManager.NavigateTo($"/SamplePages/InvoiceEdit/0/{CustomerID}/1");
        }

        /// <summary>
        /// Edit the invoice.
        /// </summary>
        /// <param name="invoiceID">The invoice identifier.</param>
        private void EditInvoice(int invoiceID)
        {
            //  NOTE:   we will hard code employee ID (1)            
            NavigationManager.NavigateTo($"/SamplePages/InvoiceEdit/{invoiceID}/{CustomerID}/1");
        }
        #endregion
    }
}
