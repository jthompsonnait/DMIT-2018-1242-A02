using HogWildSystem.BLL;
using HogWildSystem.Entities;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace HogWildWeb.Components.Pages.SamplePages
{
    public partial class InvoiceEdit
    {
        #region Fields
        private InvoiceView invoice = new();
        private int? categoryID;

        //Parts Search
        private string description = string.Empty;
        private List<LookupView> partCategories = [];
        private List<PartView> parts = [];
        private bool noParts;

        //Errors and Feedback
        private List<string> errorDetails = [];
        private string feedbackMessage = string.Empty;
        private string errorMessage = string.Empty;

        #endregion

        #region Properties
        [Inject]
        protected InvoiceService InvoiceService { get; set; } = default!;
        [Inject]
        protected PartService PartService { get; set; } = default!;
        [Inject] 
        protected CategoryLookupService CategoryLookupService { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        
        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

        //Page Parameters
        [Parameter] public int InvoiceID { get; set; }
        [Parameter] public int CustomerID { get; set; }
        [Parameter] public int EmployeeID { get; set; }
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            try
            {
                //  get the invoice
                invoice = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);
                partCategories = CategoryLookupService.GetLookups("Part Categories");
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
                errorMessage = $"{errorMessage}Unable to search for part";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }
        private void SearchParts()
        {
            try
            {
                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;

                //  clear the part list before we do our search
                parts.Clear();

                // reset no parts to false
                noParts = false;

                if (!categoryID.HasValue && string.IsNullOrWhiteSpace(description))
                {
                    throw new ArgumentException("Please provide either a category and/or description");
                }
                //  search for our parts
                List<int> existingPartIDs =
                    invoice.InvoiceLines
                    .Select(x => x.PartID)
                    .ToList();

                parts = PartService.GetParts(categoryID.HasValue ? categoryID.Value : 0, description, existingPartIDs);

                if (parts.Count() > 0)
                {
                    feedbackMessage = "Search for part(s) was successful";
                }
                else
                {
                    noParts = true;
                    feedbackMessage = "No part were found for your search criteria";
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
                errorMessage = $"{errorMessage}Unable to search for part";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }
        private void AddPart(int partID)
        {
            try
            {
                PartView? part = PartService.GetPart(partID);
                if (part != null)
                {
                    InvoiceLineView invoiceLine = new InvoiceLineView();
                    invoiceLine.PartID = partID;
                    invoiceLine.Description = part.Description;
                    invoiceLine.Price = part.Price;
                    invoiceLine.Taxable = part.Taxable;
                    invoiceLine.Quantity = 0;
                    invoice.InvoiceLines.Add(invoiceLine);
                    UpdateSubtotalAndTax();
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
                errorMessage = $"{errorMessage}Unable to search for part";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        //  delete invoice line
        /// <summary>
        /// Deletes the invoice line.
        /// </summary>
        /// <param name="invoiceLine">The invoice line to remove.</param>
        private async Task DeleteInvoiceLine(InvoiceLineView invoiceLine)
        {
            bool? results = await DialogService.ShowMessageBox("Confirm Delete", $"Are you sure that you wish to remove {invoiceLine.Description}?", yesText: "Remove", cancelText: "Cancel");

            if(results == true)
            {
                invoice.InvoiceLines.Remove(invoiceLine);
                UpdateSubtotalAndTax();
            }
        }
        private void SyncPrice(InvoiceLineView line)
        {
            //Find the original price of the Part from the database
            try
            {
                PartView? part = PartService.GetPart(line.PartID);
                if(part != null)
                {
                    line.Price = part.Price;
                    UpdateSubtotalAndTax();
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
                errorMessage = $"{errorMessage}Unable to search for part";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }
        private void UpdateSubtotalAndTax()
        {
            invoice.SubTotal = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Quantity * x.Price);
            invoice.Tax = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Taxable ? x.Quantity * x.Price * 0.05m : 0);
        }
        private void QuantityEdited(InvoiceLineView lineView, int newQuantity)
        {
            lineView.Quantity = newQuantity;
            UpdateSubtotalAndTax();
        }
        private void PriceEdited(InvoiceLineView lineView, decimal newPrice)
        {
            lineView.Price = newPrice;
            UpdateSubtotalAndTax();
        }
        private async Task Save()
        {
            try
            {
                bool isNewInvoice = false;
                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;

                //SAVE NEEDS TO BE CODED
                isNewInvoice = invoice.InvoiceID == 0;
                invoice = InvoiceService.Save(invoice);
                InvoiceID = invoice.InvoiceID;
                feedbackMessage = isNewInvoice
                    ? $"New Invoice No {invoice.InvoiceID} was created"
                    : $"Invoice No {invoice.InvoiceID} was updated";
                await InvokeAsync(StateHasChanged);
            }
            //  Your Catch Code Below
            //  code here
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
                errorMessage = $"{errorMessage}Unable to save invoice";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        private async Task Close()
        {
           bool? results = await DialogService.ShowMessageBox("Confirm Cancel", $"Do you wish to close the invoice editor? All unsaved changes will be lost.", yesText: "Yes", cancelText: "No");

            if(results == true)
            {
                NavigationManager.NavigateTo($"/SamplePages/CustomerEdit/{CustomerID}");
            }
        }
        #endregion
    }
}
