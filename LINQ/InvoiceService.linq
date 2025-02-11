<Query Kind="Program">
  <Connection>
    <ID>0a06d005-fd18-42d6-89d3-79eb231c2633</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
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

//	Driver is responsible for orchestrating the flow by calling 
//	various methods and classes that contain the actual business logic 
//	or data processing operations.
void Main()
{
	#region Add New Invoice
	//  Header information
	Console.WriteLine("==================");
	Console.WriteLine("=====  Add New Invoice  =====");
	Console.WriteLine("==================");
	//    setup Add Invoice
	InvoiceView invoiceView = new InvoiceView();

	Console.WriteLine("==================");
	Console.WriteLine("=====  Add New Invoice Fail =====");
	Console.WriteLine("==================");
	//    rule:    customer id must be supply
	//    rule:    employee id must be supply    
	//    rule:    there must be invoice lines provided
	//    rule:    for each invoice line, there must be a part
	//    rule:    for each invoice line, the price cannot be less than zero
	//    rule:    for each invoice line, the quantity cannot be less than one
	//    rule:    parts cannot be duplicated on more than one line.
	TestAddEditInvoice(invoiceView).Dump("Fail - All rules but missing model and rules involving invoice lines");

	//  do not set the InvoiceID
	//    Sam Smith (Customer)
	invoiceView.CustomerID = 1;
	// Willie Work (Employee)
	invoiceView.EmployeeID = 2;
	TestAddEditInvoice(invoiceView).Dump("Fail - Rules missing invoice lines");

	//  add invoice items
	InvoiceLineView invoiceLine = new InvoiceLineView();
	invoiceLine.PartID = 0;
	invoiceView.InvoiceLines.Add(invoiceLine);
	TestAddEditInvoice(invoiceView).Dump("Fail - Missing partID and price");

	invoiceLine.PartID = 1;
	invoiceLine.Price = -1m;
	TestAddEditInvoice(invoiceView).Dump("Fail - Price is less than zero");

	invoiceLine.PartID = 1;
	invoiceLine.Price = 10m;
	invoiceLine.Quantity = 1;
	//  adding second invoice line item, same part ID
	invoiceLine = new InvoiceLineView();
	invoiceLine.PartID = 1;
	invoiceLine.Price = 10m;
	invoiceLine.Quantity = 2;
	invoiceView.InvoiceLines.Add(invoiceLine);
	TestAddEditInvoice(invoiceView).Dump("Fail - Parts cannot be duplicated on more than one line");
	Console.WriteLine("==================");
	Console.WriteLine("=====  Add New Invoice Pass =====");
	Console.WriteLine("==================");
	//  need to remove the second invoice line.
	invoiceView.InvoiceLines.Remove(invoiceLine);
	//  get last 5 records from the invoice table 
	//    before adding new invoice record
	Invoices.Skip(Invoices.Count() - 5).Dump("Return last 5 records before adding new invoice");
	TestAddEditInvoice(invoiceView).Dump("Pass - New invoice with one invoice line");
	//  get last 5 records from the invoice table 
	//    after adding new invoice record
	Invoices.Skip(Invoices.Count() - 5).Dump("Return last 5 records after adding new invoice");

	Console.WriteLine("==================");
	Console.WriteLine("=====  Edit Existing Invoice Pass =====");
	Console.WriteLine("==================");
	int lastInvoiceID = Invoices.Skip(Invoices.Count() - 1).Select(x => x.InvoiceID).FirstOrDefault();
	invoiceView = GetInvoice(lastInvoiceID);
	invoiceView.Dump("Last invoice in the invoice table");
	//  update employee to "Nole Body"
	invoiceView.EmployeeID = 1;
	//  update qty from 1 to 10
	invoiceView.InvoiceLines[0].Quantity = 10;
	//  create a new invoice line
	invoiceLine = new InvoiceLineView();
	invoiceLine.PartID = 3;
	invoiceLine.Price = 150m;
	invoiceLine.Quantity = 2;
	invoiceView.InvoiceLines.Add(invoiceLine);
	TestAddEditInvoice(invoiceView).Dump($"Pass - Updated invoice {lastInvoiceID}");
	#endregion
}

//	This region contains methods used for testing the functionality
//	of the application's business logic and ensuring correctness.
#region Test Methods
public InvoiceView TestGetInvoice(int invoiceID)
{
	try
	{
		return GetInvoice(invoiceID);
	}
	#region catch all exceptions (define later)
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
	return null;  //  Ensures a valid return value even on failure
}

public InvoiceView TestAddEditInvoice(InvoiceView invoiceView)
{
	try
	{
		return AddEditInvoice(invoiceView);
	}
	#region catch all exceptions (define later)
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
	return null;  //  Ensures a valid return value even on failure
}
#endregion

//	This region contains support methods for testing
#region Support Methods
public Exception GetInnerException(System.Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}
#endregion

//	This region contains all methods responsible 
//	for executing business logic and operations.
#region Methods
public InvoiceView GetInvoice(int invoiceID)
{
	//  Business Rules
	//    These are processing rules that need to be satisfied
	//        for valid data
	//        rule:    invoice id must be valid 
	if (invoiceID == 0)
	{
		throw new ArgumentNullException("Please provide a invoice id");
	}
	return Invoices
				.Where(x => x.InvoiceID == invoiceID
							&& !x.RemoveFromViewFlag)
				.Select(x => new InvoiceView
				{
					InvoiceID = x.InvoiceID,
					InvoiceDate = x.InvoiceDate,
					CustomerID = x.CustomerID,
					CustomerName = $"{x.Customer.FirstName} {x.Customer.LastName}",
					EmployeeID = x.EmployeeID,
					EmployeeName = $"{x.Employee.FirstName} {x.Employee.LastName}",
					SubTotal = x.SubTotal,
					Tax = x.Tax,
					RemoveFromViewFlag = x.RemoveFromViewFlag,
					InvoiceLines = x.InvoiceLines
										.Where(il => !il.RemoveFromViewFlag)
										.Select(il => new InvoiceLineView
										{
											InvoiceLineID = il.InvoiceLineID,
											InvoiceID = il.InvoiceID,
											PartID = il.PartID,
											Description = il.Part.Description,
											Quantity = il.Quantity,
											Price = il.Price,
											RemoveFromViewFlag = il.RemoveFromViewFlag
										}).ToList()
				}).FirstOrDefault();
}

public InvoiceView AddEditInvoice(InvoiceView invoiceView)
{
	#region Business Logic and Parameter Exceptions
	//    create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();
	//  Business Rules
	//    These are processing rules that need to be satisfied
	//        for valid data
	//    rule:    invoice cannot be null
	if (invoiceView == null)
	{
		throw new ArgumentNullException("No invoice was supply");
	}
	//    rule:    customer id must be supply
	if (invoiceView.CustomerID == 0)
	{
		errorList.Add(new Exception("Customer is required"));
	}
	//    rule:    employee id must be supply    
	if (invoiceView.EmployeeID == 0)
	{
		errorList.Add(new Exception("Employee is required"));
	}
	//    rule:    there must be invoice lines provided
	if (invoiceView.InvoiceLines.Count == 0)
	{
		errorList.Add(new Exception("Invoice details are required"));
	}
	//    rule:    for each invoice line, there must be a part
	//    rule:    for each invoice line, the price cannot be less than zero
	//    rule:    for each invoice line, the quantity cannot be less than 1
	foreach (var invoiceLine in invoiceView.InvoiceLines)
	{
		if (invoiceLine.PartID == 0)
		{
			throw new ArgumentNullException("Missing part ID");
		}
		if (invoiceLine.Price < 0)
		{
			string partName = Parts
								.Where(x => x.PartID == invoiceLine.PartID)
								.Select(x => x.Description)
								.FirstOrDefault();
			errorList.Add(new Exception($"Part {partName} has a price that is less than zero"));
		}
		if (invoiceLine.Quantity < 1)
		{
			string partName = Parts
								.Where(x => x.PartID == invoiceLine.PartID)
								.Select(x => x.Description)
								.FirstOrDefault();
			errorList.Add(new Exception($"Part {partName} has a quantity that is less than one"));
		}
	}

	//    rule:    parts cannot be duplicated on more than one line.
	List<string> duplicatedParts = invoiceView.InvoiceLines
									.GroupBy(x => new { x.PartID })
									.Where(gb => gb.Count() > 1)
									.OrderBy(gb => gb.Key.PartID)
									.Select(gb => Parts
													.Where(p => p.PartID == gb.Key.PartID)
													.Select(p => p.Description)
													.FirstOrDefault()
									).ToList();
	if (duplicatedParts.Count > 0)
	{
		foreach (var partName in duplicatedParts)
		{
			errorList.Add(new Exception($"Part {partName} can only be added to the invoice lines once."));
		}
	}
	#endregion

	// Retrieve the invoice from the database or create a new one if it doesn't exist.
	Invoice invoice = Invoices
							.Where(x => x.InvoiceID == invoiceView.InvoiceID)
							.FirstOrDefault();
	// If the invoice doesn't exist, initialize it.
	if (invoice == null)
	{
		invoice = new Invoice();
		invoice.InvoiceDate = DateOnly.FromDateTime(DateTime.Now); // Set the current date for new invoices.
	}
	else
	{
		// Update the date for existing invoices.
		invoice.InvoiceDate = invoiceView.InvoiceDate;
	}
	// Map attributes from the view model to the data model.
	invoice.CustomerID = invoiceView.CustomerID;
	invoice.EmployeeID = invoiceView.EmployeeID;
	//  reset the subtotal & total as this will be updated from the invoice lines.
	invoice.SubTotal = 0;
	invoice.Tax = 0;
	invoice.RemoveFromViewFlag = invoiceView.RemoveFromViewFlag;

	// Process each line item in the provided view model.
	foreach (var invoiceLineView in invoiceView.InvoiceLines)
	{
		InvoiceLine invoiceLine = InvoiceLines
										.Where(x => x.InvoiceLineID == invoiceLineView.InvoiceLineID
												&& !x.RemoveFromViewFlag)
										.FirstOrDefault();
		// If the line item doesn't exist, initialize it.
		if (invoiceLine == null)
		{
			invoiceLine = new InvoiceLine();
			invoiceLine.PartID = invoiceLineView.PartID;
		}
		// Map fields from the line item view model to the data model.
		invoiceLine.Quantity = invoiceLineView.Quantity;
		invoiceLine.Price = invoiceLineView.Price;
		invoiceLine.RemoveFromViewFlag = invoiceLineView.RemoveFromViewFlag;

		// Handle new or existing line items.
		if (invoiceLine.InvoiceLineID == 0)
		{
			invoice.InvoiceLines.Add(invoiceLine); // Add new line items.
		}
		else
		{
			InvoiceLines.Update(invoiceLine); // Update existing line items.
		}
		//    need to update total and tax if the 
		//        invoice line item is not set to be removed from view.
		if (!invoiceLine.RemoveFromViewFlag)
		{
			invoice.SubTotal += invoiceLine.Quantity * invoiceLine.Price;
			bool isTaxable = Parts
								.Where(x => x.PartID == invoiceLine.PartID)
								.Select(x => x.Taxable)
								.FirstOrDefault();
			invoice.Tax += isTaxable ? invoiceLine.Quantity * invoiceLine.Price * .05m : 0;
		}
	}
	// If it's a new invoice, add it to the collection.
	if (invoice.InvoiceID == 0)
	{
		Invoices.Add(invoice);
	}
	else
	{
		Invoices.Update(invoice);
	}

	//    If there are errors present in the error list:
	//    NOTE:  YOU CAN ONLY HAVE ONE CHECK FOR ERRORS AND SAVE CHANGES
	//              IN A METHOD
	if (errorList.Count > 0)
	{
		// Clear changes to maintain data integrity.
		ChangeTracker.Clear();
		string errorMsg = "Unable to add or edit Invoice or Invoice Lines.";
		errorMsg += " Please check error message(s)";
		throw new AggregateException(errorMsg, errorList);
	}
	else
	{
		//    NOTE:  YOU CAN ONLY HAVE ONE SAVE CHANGES IN A METHOD    
		SaveChanges();
	}
	return GetInvoice(invoice.InvoiceID);
}
#endregion

//	This region includes the view models used to 
//	represent and structure data for the UI.
#region View Models
public class InvoiceView
{
	public int InvoiceID { get; set; }
	public DateOnly InvoiceDate { get; set; }
	public int CustomerID { get; set; }
	public string CustomerName { get; set; }
	public int EmployeeID { get; set; }
	public string EmployeeName { get; set; }
	public decimal SubTotal { get; set; }
	public decimal Tax { get; set; }
	public List<InvoiceLineView> InvoiceLines { get; set; } = new List<InvoiceLineView>();
	public bool RemoveFromViewFlag { get; set; }
}

public class InvoiceLineView
{
	public int InvoiceLineID { get; set; }
	public int InvoiceID { get; set; }
	public int PartID { get; set; }
	public string Description { get; set; }
	public int Quantity { get; set; }
	public decimal Price { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}
#endregion

