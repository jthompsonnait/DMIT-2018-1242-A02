<Query Kind="Program">
  <Connection>
    <ID>3fccf1a0-9f59-4855-8ea2-91b4e4e9b3d3</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>.</Server>
    <Database>OLTP-DMIT2018</Database>
    <DisplayName>OLTP-DMIT2018-Entity</DisplayName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	CustomerEditView customerEdit = new CustomerEditView
	{
		
	};
	
	AddEditCustomer(customerEdit).Dump();	
}

public CustomerEditView AddEditCustomer( CustomerEditView  editCustomer)
{
	#region Business Logic and Parameter Exceptions
	//    create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();
	//  Business Rules
	//    These are processing rules that need to be satisfied
	//        for valid data
	//    rule:    customer cannot be null
	if (editCustomer == null)
	{
		throw new ArgumentNullException("No customer was supply");
	}
	
	//	rule: first name, last name, phone number 
	//			and email are required (not empty)
	if(string.IsNullOrEmpty(editCustomer.FirstName))
	{
		errorList.Add(new Exception("First name is required"));
	}

	if (string.IsNullOrEmpty(editCustomer.LastName))
	{
		errorList.Add(new Exception("Last name is required"));
	}

	if (string.IsNullOrEmpty(editCustomer.Phone))
	{
		errorList.Add(new Exception("Phone number is required"));
	}

	if (string.IsNullOrEmpty(editCustomer.Email))
	{
		errorList.Add(new Exception("Email is required"));
	}

	//		rule: 	first name, last name and phone number cannot be duplicated (found more than once)
	if (editCustomer.CustomerID == 0)
	{
		bool customerExist = Customers
						.Where(x => x.FirstName == editCustomer.FirstName
									&& x.LastName == editCustomer.LastName
									&& x.Phone == editCustomer.Phone)
						.Any();

		if (customerExist)
		{
			errorList.Add(new Exception("Customer already exist in the database and cannot be enter again"));
		}
	}
	#endregion

	Customer customer =
		Customers.Where(x => x.CustomerID == editCustomer.CustomerID)
			.Select(x => x).FirstOrDefault();

}

public class CustomerEditView
{
    public int CustomerID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public int ProvStateID { get; set; }
    public int CountryID { get; set; }
    public string PostalCode { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public int StatusID { get; set; }
    public bool RemoveFromViewFlag { get; set; }
} 
