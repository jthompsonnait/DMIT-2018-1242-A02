<Query Kind="Program">
  <Connection>
    <ID>7feab626-14df-4634-b90d-faf30c4fafbb</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>OLTP-DMIT2018</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	GetCustomer(1).Dump();
}

public CustomerEditView GetCustomer(int customerID)
{
	//  Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		rule:	customerID must be valid 

	if (customerID == 0)
	{
		throw new ArgumentNullException("Please provide a customer");
	}

	return Customers
		.Where(x => (x.CustomerID == customerID
					 && x.RemoveFromViewFlag == false))
		.Select(x => new CustomerEditView
		{
			CustomerID = x.CustomerID,
			FirstName = x.FirstName,
			LastName = x.LastName,
			Address1 = x.Address1,
			Address2 = x.Address2,
			City = x.City,
			ProvStateID = x.ProvStateID,
			CountryID = x.CountryID,
			PostalCode = x.PostalCode,
			Phone = x.Phone,
			Email = x.Email,
			StatusID = x.StatusID,
			RemoveFromViewFlag = x.RemoveFromViewFlag
		}).FirstOrDefault();
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