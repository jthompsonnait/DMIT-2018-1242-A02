<Query Kind="Program">
  <Connection>
    <ID>7f1d3e56-6447-48ab-b226-55cd6694ad85</ID>
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

void Main()
{
	try
	{
		// AggregateExceptionTest("", "");

		//	passing an track ID larger than max TrackID
		//	ArgumentNullExceptionTest(10000);

		//	passing an invalid track ID (less than 1)
		//  ExceptionTest(0);
	}
	#region catch all exceptions
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
}

public Exception GetInnerException(System.Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}

public void AggregateExceptionTest(string firstName, string lastName)
{
	#region Business Logic and Parametter Exception
	//  create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//	Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		rule: first name cannot be empty
	//		rule: last name cannot be empty

	//  parameter validation	
	if (string.IsNullOrWhiteSpace(firstName))
	{
		errorList.Add(new Exception("First name is required and cannot be empty"));
	}

	if (string.IsNullOrWhiteSpace(lastName))
	{
		errorList.Add(new Exception("Last name is required and cannot be empty"));
	}
	#endregion

	/*
		actual code for method
	*/

	if (errorList.Count() > 0)
	{
		//  throw the list of business processing error(s)
		throw new AggregateException("Unable to proceed! Check concerns", errorList);
	}
}

public void ArgumentNullExceptionTest(int trackID)
{
	#region Business Logic and Parametter Exception
	//  create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//	Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		rule: Track must exist in the database

	//  parameter validation	
	var track = Tracks
					.Where(x => x.TrackId == trackID)
					.Select(x => x).FirstOrDefault();
	if (track == null)
	{
		throw new ArgumentNullException($"No track was found for Track ID: {trackID}");
	}

	#endregion

	/*
		actual code for method
	*/

	if (errorList.Count() > 0)
	{
		//  throw the list of business processing error(s)
		throw new AggregateException("Unable to proceed! Check concerns", errorList);
	}
}

public void ExceptionTest(int trackID)
{
	#region Business Logic and Parametter Exception
	//  create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	//	Business Rules
	//	These are processing rules that need to be satisfied
	//		for valid data
	//		rule: Track must exist in the database

	//  parameter validation	
	
	if (trackID <1)
	{
		throw new Exception($"Track ID is invalid: {trackID}");
	}

	#endregion

	/*
		actual code for method
	*/

	if (errorList.Count() > 0)
	{
		//  throw the list of business processing error(s)
		throw new AggregateException("Unable to proceed! Check concerns", errorList);
	}
}