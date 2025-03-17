using HogWildSystem.ViewModels;

namespace HogWildWeb.Components.Pages.SamplePages
{
    public partial class SimpleNonIndexList
    {
        protected List<CustomerEditView> Customers { get; set; } = new();
        private string customerName { get; set; } = string.Empty;

        private void RemoveCustomer(int customerId)
        {
            var selectedItem =
                Customers.FirstOrDefault(x => x.CustomerID == customerId);
            if (selectedItem != null)
            {
                Customers.Remove(selectedItem);
            }
        }

        private async Task AddCustomerToList()
        {
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                int maxID = Customers.Any() ? Customers.Max(x => x.CustomerID) + 1 : 1;
                Customers.Add(new CustomerEditView()
                {
                    CustomerID = maxID,
                    FirstName = customerName
                });
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
