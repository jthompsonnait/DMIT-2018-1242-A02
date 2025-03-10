using System.Globalization;
using HogWildSystem.BLL;
using Microsoft.AspNetCore.Components;

namespace HogWildWeb.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
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
        #region Properties
        //  The customer service
        [Inject] protected CustomerService CustomerService { get; set; } = default!;

        //  Customer ID used to create or edit a customer
        [Parameter] public int CustomerID { get; set; } = 0;
        #endregion
    }
}
