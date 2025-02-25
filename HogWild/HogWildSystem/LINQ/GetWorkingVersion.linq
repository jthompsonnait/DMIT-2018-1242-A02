<Query Kind="Program">
  <Connection>
    <ID>86bdf9e1-7f71-4cbc-a54b-f58c14f9615b</ID>
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
  <RuntimeVersion>9.0</RuntimeVersion>
</Query>

void Main()
{
	GetWorkingVersion().Dump();
}

public WorkingVersionsView GetWorkingVersion()
{
	return WorkingVersions
			.Select(x => new WorkingVersionsView
			{
				VersionID = x.VersionId,
				Major = x.Major,
				Minor = x.Minor,
				Build = x.Build,
				Revision = x.Revision,
				AsOfDate = x.AsOfDate,
				Comments = x.Comments
			}).FirstOrDefault();
}

public class WorkingVersionsView
{
	public int VersionID { get; set; }
	public int Major { get; set; }
	public int Minor { get; set; }
	public int Build { get; set; }
	public int Revision { get; set; }
	public DateTime AsOfDate { get; set; }
	public string Comments { get; set; }
}

// You can define other methods, fields, classes and namespaces here
