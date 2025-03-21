<Query Kind="Program">
  <Connection>
    <ID>4383e956-cbc4-4e04-b56d-009fd1fae79e</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>ChinookSept2018</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//	Driver is responsible for orchestrating the flow by calling 
//	various methods and classes that contain the actual business logic 
//	or data processing operations.
void Main()
{
	#region Get Artist (GetArtist)
	//	Pass
	TestGetArtist(1).Dump("Pass - Valid ID");
	TestGetArtist(1000).Dump("Pass - Valid ID - No artist found");
	
	//	Fail
	//	Rule:	artistID must be valid
	TestGetArtist(0).Dump("Fail - ArtistID must be Valid");
	
	//  simple get without using a test
	//	showing list of artist for a single record.
	GetArtist1(1).Dump("Pass - Valid ID");
	#endregion

	#region Get Artists (GetArtist)
	//	Pass
	TestGetArtists("ABB").Dump("Pass - Valid Name");
	TestGetArtists("ABC").Dump("Pass - Valid Name - No artist found");

	//	Fail
	//	Rule:	artistID must be valid
	TestGetArtists(string.Empty).Dump("Fail - Artist name was empty");
	#endregion
}

//	This region contains methods used for testing the functionality
//	of the application's business logic and ensuring correctness.
#region Test Methods
public ArtistEditView TestGetArtist(int artistID)
{
	try
	{
		return GetArtist(artistID);
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

public List<ArtistEditView> TestGetArtists(string artistName)
{
	try
	{
		return GetArtists(artistName);
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
public ArtistEditView GetArtist(int artistID)
{
	#region Business Logic and Parameter Exceptiions
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//  Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		Rule:	artistID must be valid
	
	if (artistID == 0)
	{
		throw new ArgumentNullException("Please provide a valid artist ID");
	}
	#endregion

	return Artists
			.Where(x => x.ArtistId == artistID)
			.Select(x => new ArtistEditView
			{
				ArtistID = x.ArtistId,
				Name = x.Name

			}).FirstOrDefault();
}

public List<ArtistEditView> GetArtists(string artistName)
{
	#region Business Logic and Parameter Exceptiions
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//  Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		Rule:	artist name is required

	if (string.IsNullOrEmpty(artistName))
	{
		throw new ArgumentNullException("Artist name is required");
	}
	#endregion

	return Artists
		.Where(x => x.Name.ToUpper().Contains(artistName.ToUpper()))
		.Select(x => new ArtistEditView
		{
			ArtistID = x.ArtistId,
			Name = x.Name
		}).ToList();
}
public List<ArtistEditView> GetArtist1(int artistID)
{
	#region Business Logic and Parameter Exceptiions
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//  Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		Rule:	artistID must be valid

	if (artistID == 0)
	{
		throw new ArgumentNullException("Please provide a valid artist ID");
	}
	#endregion

	return Artists
			.Where(x => x.ArtistId == artistID)
			.Select(x => new ArtistEditView
			{
				ArtistID = x.ArtistId,
				Name = x.Name
			}).ToList();
}
#endregion

//	This region includes the view models used to 
//	represent and structure data for the UI.
#region View Models
public class ArtistEditView
{
	public int ArtistID { get; set; }
	public string Name { get; set; }
}
#endregion

