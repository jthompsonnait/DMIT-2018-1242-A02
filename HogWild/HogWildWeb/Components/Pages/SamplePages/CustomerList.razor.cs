using System.Diagnostics;
using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HogWildWeb.Components.Pages.SamplePages
{
	public partial class CustomerList
	{
		#region fields

		private string lastName = string.Empty;
		private string phoneNumber = string.Empty;
		private bool noRecords;
		private string feedbackMessage = string.Empty;
		private string errorMessage = string.Empty;

		private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);
		private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);
		private List<string> errorDetails = new();

		#endregion

		#region properties

		[Inject] 
		protected CustomerService CustomerService { get; set; } = default!;

		[Inject]
		protected NavigationManager NavigationManager { get; set; } = default!;
		protected List<CustomerSearchView> Customers { get; set; } = new();

		#endregion

		#region methods
		private void Search()
		{
			try
			{
				noRecords = false;
				errorDetails.Clear();
				errorMessage = string.Empty;
				feedbackMessage = string.Empty;
				Customers.Clear();

				if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phoneNumber))
				{
					throw new ArgumentException("Please provide either a last name and/or a phone number");
				}

				Customers = CustomerService.GetCustomers(lastName, phoneNumber);
				if (Customers.Count > 0)
				{
					feedbackMessage = "Search for customer(s) was successful!";
				}
				else
				{
					feedbackMessage = "No customers were found for your search criteria!";
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

		private void New()
		{
			
		}

		private void EditCustomer(int customerID)
		{

		}

		private void NewInvoice(int customerID)
		{

		}

		#endregion
	}
}
