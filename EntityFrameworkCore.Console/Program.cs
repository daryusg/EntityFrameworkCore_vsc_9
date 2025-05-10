using EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;

using var context = new FootballLeagueDbContext();

//select all teams cip...31
//GetAllTeams();

//cip...33. selecting a single record - first in the list
var team1 = await context.Teams.FirstAsync();
//var team1 = await context.Teams.FirstOrDefaultAsync();

//cip...33. selecting a single record - first in the list where id == 1
var team2 = await context.Teams.FirstAsync(q => q.Id == 1);
//var team2 = await context.Teams.FirstOrDefaultAsync(q => q.Id == 1);

//var coach = await context.Coaches.FirstAsync(); //cip...33. exception as no coaches yet
var coach = await context.Coaches.FirstOrDefaultAsync(); //cip...33. ok even though no coaches yet

//cip...33. selecting a single record - must return a single record
//var team3 = await context.Teams.SingleAsync(); //cip...33. exception as more than 1 record in Teams table
var team3 = await context.Teams.SingleAsync(q => q.Id == 2); //cip...33. ok even though more than 1 record in Teams table. NOTE: LIMIT 2 so that ef can error if more than 1 record was returned.
var team4 = await context.Teams.SingleOrDefaultAsync(q => q.Id == 2); //if nothing returned, returns null instead of exception

//selecting based on id
var teamBasedOnId = await context.Teams.FindAsync(3); //cip...33. uses the primary key to find the record. NOTE: FindAsync() is not a LINQ method, but a method of DbSet<T>. It will return null if no record is found.
if (teamBasedOnId != null)
{
    Console.WriteLine($"Team: {teamBasedOnId.Name}, Created Date: {teamBasedOnId.CreatedDate}");
}
else
{
    Console.WriteLine("No team found with the specified ID.");
}

void GetAllTeams() //cip...33
{
    var teams = context.Teams.ToList();
    foreach (var team in teams)
    {
        Console.WriteLine($"Team: {team.Name}, Created Date: {team.CreatedDate}");
    }
}
