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
	#region Get Lookup (GetLookup)
	//  Header information
	Console.WriteLine("==================");
	Console.WriteLine("=====  Get Lookup Pass  =====");
	Console.WriteLine("==================");
	//  Pass
	TestGetLookup("Alberta").Dump("Pass - Valid lookup name");
	TestGetLookup("Alberta1").Dump("Pass - Valid name - No Lookup found");

	//  Header information
	Console.WriteLine();
	Console.WriteLine("==================");
	Console.WriteLine("=====  Get Lookup Fail  =====");
	Console.WriteLine("==================");
	//  Fail
	//  rule:	Lookup name cannot be empty or null
	//	string.Empty -> ""
	TestGetLookup(string.Empty).Dump("Fail - Lookup name was empty");
	#endregion

	#region Add New Lookup
	//  Header information
	Console.WriteLine("==================");
	Console.WriteLine("=====  Add New Lookup  =====");
	Console.WriteLine("==================");

	//  create a new lookupView view model for adding/editing
	LookupView lookupView = new LookupView();

	// Fail
	Console.WriteLine("==================");
	Console.WriteLine("=====  Add New Lookup Fail  =====");
	Console.WriteLine("==================");

	//		rule: 	category id is required
	TestAddEditLookup(lookupView).Dump("Fail - Category ID is missing");
	#endregion

}

//	This region contains methods used for testing the functionality
//	of the application's business logic and ensuring correctness.
#region Test Methods
public LookupView TestGetLookup(string lookupName)
{
	try
	{
		return GetLookup(lookupName);
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

public LookupView TestAddEditLookup(LookupView lookupView)
{
	try
	{
		return AddEditLookup(lookupView);
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

/// <summary>
/// Generates a random name of a given length.
/// The generated name follows a pattern of alternating consonants and vowels.
/// </summary>
/// <param name="len">The desired length of the generated name.</param>
/// <returns>A random name of the specified length.</returns>
public string GenerateName(int len)
{
	// Create a new Random instance.
	Random r = new Random();

	// Define consonants and vowels to use in the name generation.
	string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
	string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };

	string Name = "";

	// Start the name with an uppercase consonant and a vowel.
	Name += consonants[r.Next(consonants.Length)].ToUpper();
	Name += vowels[r.Next(vowels.Length)];

	// Counter for tracking the number of characters added.
	int b = 2;

	// Add alternating consonants and vowels until we reach the desired length.
	while (b < len)
	{
		Name += consonants[r.Next(consonants.Length)];
		b++;
		Name += vowels[r.Next(vowels.Length)];
		b++;
	}

	return Name;
}
#endregion

//	This region contains all methods responsible 
//	for executing business logic and operations.
#region Methods
public LookupView GetLookup(string lookupName)
{
	#region Business Logic and Parameter Exceptiions
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//  Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		Rule:	lookup name is required

	if (string.IsNullOrWhiteSpace(lookupName))
	{
		throw new ArgumentNullException("Lookup name cannot be empty");
	}
	#endregion

	return Lookups
			.Where(x => x.Name.ToUpper() == lookupName.ToUpper()
				&& !x.RemoveFromViewFlag)
			.Select(x => new LookupView
			{
				LookupID = x.LookupID,
				CategoryID = x.CategoryID,
				Name = x.Name,
				RemoveFromViewFlag = x.RemoveFromViewFlag

			}).FirstOrDefault();
}

public LookupView AddEditLookup(LookupView lookupView)
{
	#region Business Logic and Parameter Exceptiions
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//  Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		Rule:	lookup view cannot be null
	//		Rule:	categoryID must be supply
	//		Rule:	lookup name is required
	//		Rule:	lookup cannot be duplicated (found more than once)

	//		Rule:	category view cannot be null
	if (lookupView == null)
	{
		throw new ArgumentNullException("No lookup was supply");
	}

	if (lookupView.CategoryID == 0)
	{
		throw new ArgumentNullException("No category id was supply");
	}

	if (string.IsNullOrWhiteSpace(lookupView.Name))
	{
		errorList.Add(new Exception("Lookup name is required"));
	}

	//	rule:  lookup cannot be duplicated for category id that is provided
	//			(found more than once)
	if (lookupView.LookupID == 0)
	{
		bool lookupExist = Lookups
						.Where(x => x.CategoryID == lookupView.CategoryID
							&& x.Name == lookupView.Name)
						.Any();

		if (lookupExist)
		{
			errorList.Add(new Exception("Lookup already exist in the database and cannot be enter again"));
		}
	}
	#endregion

	// check to see if the category exist in the database
	Lookup lookup =
			Lookups.Where(x => x.LookupID == lookupView.LookupID)
			.Select(x => x).FirstOrDefault();

	//  if the lookup was not found (LookupID == 0)
	//		then we are dealing with a new lookup
	if (lookup == null)
	{
		lookup = new Lookup();
		//	Updating category ID only when we have a new lookup.
		lookup.CategoryID = lookupView.CategoryID;
	}

	// 	Updating lookup name.
	//	NOTE:  	You do not have to update the primary key "LookupID".
	//				This is true for all primary keys for any view models.
	//			- If it is a new lookup, the LookupID will be "0"
	//			- If it is an existing lookup, there is no need
	//				to update it.

	lookup.Name = lookupView.Name;

	//	You must set the RemoveFromViewFlag incase it has been soft delete
	lookup.RemoveFromViewFlag = lookupView.RemoveFromViewFlag;

	//	If there are errors present in the error list:
	//	NOTE:  YOU CAN ONLY HAVE ONE CHECK FOR ERRORS AND SAVE CHANGES
	//			  IN A METHOD
	if (errorList.Count > 0)
	{
		// 	Clearing the "track changes" ensures consistency in our entity system.
		//	Otherwise we leave our entity system in flux
		ChangeTracker.Clear();
		//	Throw an AggregateException containing the list of business processing errors.
		throw new AggregateException("Unable to add or edit lookup. Please check error message(s)", errorList);
	}
	else
	{
		if (lookup.LookupID == 0)
			//  Adding a new category record to the Lookup table
			Lookups.Add(lookup);
		else
			//	Updating an category record in the Lookup table
			Lookups.Update(lookup);

		//	try catch handles issue from the database schema such as
		//		max field length of 10 of value cannot be less than zero
		try
		{
			//	NOTE:  YOU CAN ONLY HAVE ONE SAVE CHANGES IN A METHOD
			SaveChanges();
		}
		catch (Exception ex)
		{
			throw new Exception($"An error occurred while saving: {ex.Message}", ex);
		}
	}
	return GetLookup(lookupView.Name);
}
#endregion

//	This region includes the view models used to 
//	represent and structure data for the UI.
#region View Models
public class LookupView
{
	public int LookupID { get; set; }
	public int CategoryID { get; set; }
	public string Name { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}

#endregion

