using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;

//first i need an instance of the context
using var context = new FootballLeagueDbContext();

//select all teams cip...31

//GetAllTeams(); cip...33

//select one team cip...34
//GetOneTeamAsync();

//select all records that meet a condition //cip...34
//await GetFilteredTeamsAsync();

//await GetAllTeamsQuerySyntaxAsync(); //cip...36

//aggregate methods //cip...37
//await AggregateMethodsAsync();

//grouping and aggregating //cip...38
//await GroupByMethodAsync();

//ordering //cip...39
//await OrderByMethodsAsync();

//skip and take - great for paging //cip...40
//await SkipAndTakeAsync();

//select and projections - more precise queries //cip...41
//await ProjectionsAndSelectAsync();

//no tracking - ef core tracks ojbjects tha are retrieved from the database. this is less useful in disconnected scenarios like api's and web apps. //cip...42
//await NoTrackingAsync();

//iqueryables vs list types //cip...43
await IQueryablesVsListTypesAsync();
async Task IQueryablesVsListTypesAsync() //cip...43
{
    Console.Write("Enter '1' for team with Id = 1 or '2' for teams that contain 'FC':");
    var optionSelected = Convert.ToInt32(Console.ReadLine());
    List<Team> teamsAsList = new List<Team>();
    
    //after executing ToListAsync(), the query is executed and the results are stored in memory. any op is performed on the in-memory list.
    teamsAsList = await context.Teams.ToListAsync();
    switch (optionSelected)
    {
        case 1:
            teamsAsList = teamsAsList.Where(q => q.Id == 1).ToList();
            break;
        case 2:
            teamsAsList = teamsAsList.Where(q => q.Name.Contains("FC")).ToList();
            break;
        default:
            Console.WriteLine("Invalid option selected.");
            break;
    }

    foreach (var team in teamsAsList)
    {
        Console.WriteLine($"teamsAsList Team: {team.Name}, Created Date: {team.CreatedDate}");
    }

    //records stay as IQueryable until the executing method is called.
    var teamsAsQueryable = context.Teams.AsQueryable();
    switch (optionSelected)
    {
        case 1:
            teamsAsQueryable = teamsAsQueryable.Where(q => q.Id == 1);
            break;
        case 2:
            teamsAsQueryable = teamsAsQueryable.Where(q => q.Name.Contains("FC"));
            break;
        default:
            Console.WriteLine("Invalid option selected.");
            break;
    }

    //actual query execution
    teamsAsList = await teamsAsQueryable.ToListAsync();
    foreach (var team in teamsAsList)
    {
        Console.WriteLine($"teamsAsQueryable Team: {team.Name}, Created Date: {team.CreatedDate}");
    }
}

async Task NoTrackingAsync() //cip...42
{
    //no tracking
    var teams = await context.Teams
        .AsNoTracking()
        .ToListAsync(); //cip...42. AsNoTracking() is a method of DbSet<T> and not a LINQ method.
    foreach (var team in teams)
    {
        Console.WriteLine($"Team: {team.Name}, Created Date: {team.CreatedDate}");
    }
}

async Task ProjectionsAndSelectAsync() //cip...41
{
    //select and projections
    var teamNames1 = await context.Teams
        .Select(q => q.Name) //select only the Name property
        .ToListAsync();

    foreach (var name in teamNames1)
    {
        Console.WriteLine($"teamNames1: {name}");
    }

    var teamNames2 = await context.Teams
        .Select(q => new { q.Name, q.CreatedDate }) //select the Name and CreatedDate properties (into new anonymous datatype)
        .ToListAsync();

    foreach (var team in teamNames2)
    {
        Console.WriteLine($"teamNames2: {team.Name}, Created Date: {team.CreatedDate}");
    }

    var teamNames3 = await context.Teams
        .Select(q => new TeamInfo { Id = q.Id, Name = q.Name, CreatedDate = q.CreatedDate }) //select the Name and CreatedDate properties (into a new class)
        .ToListAsync();

    foreach (var team in teamNames3)
    {
        Console.WriteLine($"teamNames3: {team.Name}, Id: {team.Id}, Created Date: {team.CreatedDate}");
    }
}

async Task SkipAndTakeAsync() //cip...40
{
    var recordCount = 3;
    var page = 0;
    var next = true;
    //skip and take
    while (next)
    {
        var teams = await context.Teams
            .Skip(page++ * recordCount) //skip the necessary records and (post-)increment the page no
            .Take(recordCount) //take recordCount records
            .ToListAsync();
        
        if (teams.Count == 0) break; //if no records returned, exit the loop

        foreach (var team in teams)
        {
            Console.WriteLine($"Team: {team.Name}, Created Date: {team.CreatedDate}");
        }
        Console.Write("Enter 'true' for the next set of records or 'false' to exit:");
        next = Convert.ToBoolean(Console.ReadLine());

        if (!next) break;
    }
}

async Task OrderByMethodsAsync() //cip...39

{
    //order by
    var orderedTeams = await context.Teams
        .OrderBy(q => q.Name) //ascending order
        .ToListAsync();

    var descOrderedTeams = await context.Teams
        .OrderByDescending(q => q.Name) //descending order
        .ToListAsync();

    //order by descending
    foreach (var team in orderedTeams)
    {
        Console.WriteLine($"Team (Ascending): {team.Name}, Created Date: {team.CreatedDate}");
    }

    //order by descending
    foreach (var team in descOrderedTeams)
    {
        Console.WriteLine($"Team Descending: {team.Name}, Created Date: {team.CreatedDate}");
    }

    //MaxBy (not currently asynchronous)
    var maxBy = context.Teams.MaxBy(q => q.Id); //cip...39. returns the record with the max value Id
    //it's standard equivalent:
    var maxByDescOrderedTeams = await context.Teams
        .OrderByDescending(q => q.Id) //descending order
        .FirstOrDefaultAsync(); //cip...39. returns the record with the max value Id

    //MinBy (not currently asynchronous)
    var minBy = context.Teams.MinBy(q => q.Id); //cip...39. returns the record with the min value Id
    //it's standard equivalent:
    var minByDescOrderedTeams = await context.Teams
        .OrderBy(q => q.Id) //ascending order
        .FirstOrDefaultAsync(); //cip...39. returns the record with the min value Id
}

async Task GroupByMethodAsync() //cip...38
{
    //grouping and aggregating
    //var groupedTeams = await context.Teams.GroupBy(q => new {q.CreatedDate, q.Name, Q.Id}) //multiple columns in the groupby
    //var groupedTeams = await context.Teams.GroupBy(q => q.CreatedDate) //single column in the groupby
    var groupedTeams = (context.Teams
        //.Where(q => q.Name == "") //translates to a WHERE clause
        //.GroupBy(q => new { q.CreatedDate.Date }))
        .GroupBy(q => q.CreatedDate.Date));
    //.Where(q => q.Name == "") //translates to a HAVING clause
    //.ToListAsync(); //use the executing method to load the results into memory before processing
    //NOTE: ef core can iterate through records on demand. here, there is no executing method (ie ToList(), ToListAsync()) but ef core is still able to iterate through the records on demand.
    //this is because the groupby method is not a LINQ method, but a method of DbSet<T>. It will return null if no record is found.
    //it is generally better to use the executing method to load the results into memory before processing.
    foreach (var group in groupedTeams)
    {
        //group.Key is what i grouped on
        Console.WriteLine($"Created Date: {group.Key}");
        Console.WriteLine($"Id Sum: {group.Sum(q => q.Id)}");
        foreach (var team in group)
        {
            //group is the collection of items in the group
            Console.WriteLine($"Team: {team.Name}, Created Date: {team.CreatedDate}");
        }
    }
}

async Task AggregateMethodsAsync() //cip...37
{
    //count
    var numOfTeams = await context.Teams.CountAsync();
    Console.WriteLine($"Number of teams: {numOfTeams}");
    var numOfTeamsWithCondition1 = await context.Teams.CountAsync(q => q.Id == 1);
    Console.WriteLine($"Number of teams with condition #1: {numOfTeamsWithCondition1}");
    var numOfTeamsWithCondition2 = await context.Teams.CountAsync(q => EF.Functions.Like(q.Name, $"%i%"));
    Console.WriteLine($"Number of teams with condition #2: {numOfTeamsWithCondition2}");

    //max
    var maxTeamsId = await context.Teams.MaxAsync(q => q.Id);
    Console.WriteLine($"Max team id: {maxTeamsId}");

    //min
    var minTeamsId = await context.Teams.MinAsync(q => q.Id);
    Console.WriteLine($"Min team id: {minTeamsId}");

    //avarage
    var avgTeamsId = await context.Teams.AverageAsync(q => q.Id);
    Console.WriteLine($"Average team id: {avgTeamsId}");

    //sum
    var sumTeamsId = await context.Teams.SumAsync(q => q.Id);
    Console.WriteLine($"Sum team id: {sumTeamsId}");
}

async Task GetAllTeamsQuerySyntaxAsync() //cip...36
{
    Console.Write("Enter search term: ");
    //var desiredTeam = Console.ReadLine(); // Read the input from the user //cip...34
    var searchTerm = Console.ReadLine(); // Read the input from the user //cip...35
    //SELECT * FROM Teams
    var teams = await (
        from t
        in context.Teams
        where EF.Functions.Like(t.Name, $"%{searchTerm}%")
        select t
    ).ToListAsync();
    foreach (var team in teams)
    {
        Console.WriteLine($"GetAllTeamsQuerySyntax.team: {team.Name}, Created Date: {team.CreatedDate}");
    }
}

async Task GetFilteredTeamsAsync() //cip...34
{
    // Prompt the user for input
    Console.Write("Enter search term: ");
    //var desiredTeam = Console.ReadLine(); // Read the input from the user //cip...34
    var searchTerm = Console.ReadLine(); // Read the input from the user //cip...35
    // Use the input to filter the teams
    var teamsFiltered = await context.Teams.Where(q => q.Name == searchTerm).ToListAsync();
    foreach (var team in teamsFiltered)
    {
        Console.WriteLine($"Team: {team.Name}, Created Date: {team.CreatedDate}");
    }

    //var partialMatches = await context.Teams.Where(q => q.Name.Contains(searchTerm)).ToListAsync();
    //SELECT * FROM Teams WHERE Name LIKE '%FC%'
    var partialMatches = await context.Teams.Where(q => EF.Functions.Like(q.Name, $"%{searchTerm}%")).ToListAsync();
    foreach (var item in partialMatches)
    {
        Console.WriteLine($"Partial Match: {item.Name}, Created Date: {item.CreatedDate}");
    }
}

async Task GetOneTeamAsync() //cip...34
{
    //cip...33. selecting a single record - first in the list
    var team1 = await context.Teams.FirstAsync();
    //var team1 = await context.Teams.FirstOrDefaultAsync();
    if (team1 != null)
    {
        Console.WriteLine($"team1: {team1.Name}, Created Date: {team1.CreatedDate}");
    }

    //cip...33. selecting a single record - first in the list where id == 1
    var team2 = await context.Teams.FirstAsync(q => q.Id == 1);
    //var team2 = await context.Teams.FirstOrDefaultAsync(q => q.Id == 1);
    if (team2 != null)
    {
        Console.WriteLine($"team2: {team2.Name}, Created Date: {team2.CreatedDate}");
    }

    //var coach = await context.Coaches.FirstAsync(); //cip...33. exception as no coaches yet
    var coach = await context.Coaches.FirstOrDefaultAsync(); //cip...33. ok even though no coaches yet
    if (coach != null)
    {
        Console.WriteLine($"coach: {coach.Name}, Created Date: {coach.CreatedDate}");
    }

    //cip...33. selecting a single record - must return a single record
    //var team3 = await context.Teams.SingleAsync(); //cip...33. exception as more than 1 record in Teams table
    var team3 = await context.Teams.SingleAsync(q => q.Id == 3); //cip...33. ok even though more than 1 record in Teams table. NOTE: LIMIT 2 so that ef can error if more than 1 record was returned.
    if (team3 != null)
    {
        Console.WriteLine($"team3: {team3.Name}, Created Date: {team3.CreatedDate}");
    }

    var team4 = await context.Teams.SingleOrDefaultAsync(q => q.Id == 2); //if nothing returned, returns null instead of exception
    if (team4 != null)
    {
        Console.WriteLine($"team4: {team4.Name}, Created Date: {team4.CreatedDate}");
    }

    //cip...33. selecting based on id
    var teamBasedOnId = await context.Teams.FindAsync(3); //uses the primary key to find the record. NOTE: FindAsync() is not a LINQ method, but a method of DbSet<T>. It will return null if no record is found.
    if (teamBasedOnId != null)
    {
        Console.WriteLine($"teamBasedOnId: {teamBasedOnId.Name}, Created Date: {teamBasedOnId.CreatedDate}");
    }
}

async Task GetAllTeams() //cip...33
{
    //SELECT * FROM Teams
    var teams = await context.Teams.ToListAsync();
    foreach (var team in teams)
    {
        Console.WriteLine($"Team: {team.Name}, Created Date: {team.CreatedDate}");
    }
}

class TeamInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
}
