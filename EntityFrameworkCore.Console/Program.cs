using EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;

//first i need an instance of the context
using var context = new FootballLeagueDbContext();

//select all teams cip...31
GetAllTeams();

//select one teams cip...34
GetOneTeam();

await GetFilteredTeams(); //cip...34

async Task GetFilteredTeams() //cip...34
{
    // Prompt the user for input
    Console.WriteLine("Enter search term: ");
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
    foreach(var item in partialMatches)
    {
        Console.WriteLine($"Partial Match: {item.Name}, Created Date: {item.CreatedDate}");
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

async Task GetOneTeam() //cip...34
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
    if(team3 != null)
    {
        Console.WriteLine($"team3: {team3.Name}, Created Date: {team3.CreatedDate}");
    }

    var team4 = await context.Teams.SingleOrDefaultAsync(q => q.Id == 2); //if nothing returned, returns null instead of exception
    if(team4 != null)
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