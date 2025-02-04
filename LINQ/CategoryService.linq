<Query Kind="Program">
  <Connection>
    <ID>0a06d005-fd18-42d6-89d3-79eb231c2633</ID>
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

//	Driver is responsible for orchestrating the flow by calling 
//	various methods and classes that contain the actual business logic 
//	or data processing operations.
void Main()
{
	#region Get Category (TestGetCategory)
	//	Pass
	TestGetCategory("Province").Dump("Pass - Valid Category");
	TestGetCategory("People").Dump("Pass - Valid Category - No category found");

	//	Fail
	//	Rule:	artistID must be valid
	TestGetCategory(string.Empty).Dump("Fail - Category name was empty");
	#endregion
}

//	This region contains methods used for testing the functionality
//	of the application's business logic and ensuring correctness.
#region Test Methods
public CategoryView TestAddEditCategory(CategoryView categoryView)
{
	try
	{
		return AddEditCategory(categoryView);
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

public CategoryView TestGetCategory(string categoryName)
{
	try
	{
		return GetCategory(categoryName);
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
public CategoryView GetCategory(string categoryName)
{
	#region Business Logic and Parameter Exceptiions
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//  Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		Rule:	category name is required

	if (string.IsNullOrWhiteSpace(categoryName))
	{
		throw new ArgumentNullException("category name cannot be empty");
	}
	#endregion

	return Categories
			.Where(x => x.CategoryName.ToUpper() == categoryName.ToUpper()
				&& !x.RemoveFromViewFlag)
			.Select(x => new CategoryView
			{
				CategoryID = x.CategoryID,
				CategoryName = x.CategoryName,
				RemoveFromViewFlag = x.RemoveFromViewFlag

			}).FirstOrDefault();
}

public CategoryView AddEditCategory(CategoryView categoryView)
{
	#region Business Logic and Parameter Exceptiions
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//  Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		Rule:	category view cannot be null
	//		Rule:	category name is required
	//		Rule:	category cannot be duplicated (found more than once)

	//		Rule:	category view cannot be null
	if (categoryView == null)
	{
		throw new ArgumentNullException("No category was supply");
	}

	//		Rule:	category name is required
	if (string.IsNullOrWhiteSpace(categoryView.CategoryName))
	{
		errorList.Add(new Exception("Category name cannot be empty"));
	}

	//		Rule:	category cannot be duplicated (found more than once)
	if (categoryView.CategoryID == 0)
	{
		bool categoryExist = Categories
								.Where(x => x.CategoryName.ToUpper() ==
										categoryView.CategoryName.ToUpper())
								.Any();
		if (categoryExist)
		{
			errorList.Add(new Exception("Category already exist in the database and cannot be enter again"));
		}

	}

	#endregion
	
	//	check to see if the category exist in the database
	Category category = 
					Categories.Where(x => x.CategoryID == categoryView.CategoryID)
					.Select(x => x).FirstOrDefault();
					
	//	if the category was not found (CategoryID == 0)
	//		then we are dealing with a new category
	if (category == null)
	{
		category = new Category();
	}
}
#endregion

//	This region includes the view models used to 
//	represent and structure data for the UI.
#region View Models
public class CategoryView
{
	public int CategoryID { get; set; }
	public string CategoryName { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}
#endregion

