using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;

//first i need an instance of the context
using var context = new FootballLeagueDbContext();
context.Database.MigrateAsync(); //cip...63. NOTE: this will create the database if it doesn't exist and apply any pending migrations. it will not create the database if it already exists.

//for sqlite users to see where the db file gets created:
//Console.WriteLine($"Database file location: {context.DbPath}");


#region Related Data  //cip...76

//insert record with fk
//await InsertMatch();
async Task InsertMatch() //cip...76
{
    var match1 = new Match
    {
        HomeTeamScore = 0,
        AwayTeamScore = 0,
        TicketPrice = 22.50m,
        Date = new DateTime(2025, 5, 24, 13, 0, 0),
        HomeTeamId = 2,
        AwayTeamId = 1,
        CreatedDate = DateTime.Now,
        CreatedBy = "TestUser1"
    };
    //NOTE: the Id fields are fk's and must have a valid record in the database.
    await context.AddAsync(match1); //cip...76
    await context.SaveChangesAsync();

    //ERROR. invalid fk:
    var match2 = new Match
    {
        HomeTeamScore = 0,
        AwayTeamScore = 0,
        TicketPrice = 22.50m,
        Date = new DateTime(2025, 5, 24, 13, 0, 0),
        HomeTeamId = 0,
        AwayTeamId = 10,
        CreatedDate = DateTime.Now,
        CreatedBy = "TestUser1"
    };
    //NOTE: the Id fields are fk's and must have a valid record in the database.
    await context.AddAsync(match2); //cip...76
    await context.SaveChangesAsync();
}

//insert parent/child
//await InsertTeamWithCoachAsync();
async Task InsertTeamWithCoachAsync() //cip...76
{
    var team = new Team
    {
        Name = "New Team",
        Coach = new Coach
        {
            Name = "Johnson",
            CreatedDate = DateTime.Now,
            CreatedBy = "TestUser1"
        },
        CreatedDate = DateTime.Now,
        CreatedBy = "TestUser1",
        LeagueId = 1 //cip...76. NOTE: this is a nullable field and can be null(???).
    };
    await context.AddAsync(team);
    await context.SaveChangesAsync();
    //NOTE: it inserted the coach first and then the team. this is because the coach is a child of the team. Team needs a valid Coach and coachID.
}

//insert parent with children
//await InsertLeagueWithTeamsAsync();
async Task InsertLeagueWithTeamsAsync() //cip...76

{
    var league = new League
    {
        Name = "Serie A",
        CreatedDate = DateTime.Now,
        CreatedBy = "TestUser1",
        Teams = new List<Team>
        {
            new Team
            {
                Name = "Juventus",
                Coach = new Coach
                {
                    Name = "Juve Coach",
                    CreatedDate = DateTime.Now,
                    CreatedBy = "TestUser1"
                },
                CreatedDate = DateTime.Now,
                CreatedBy = "TestUser1"
            },
            new Team
            {
                Name = "AC Milan",
                Coach = new Coach
                {
                    Name = "Milan Coach",
                    CreatedDate = DateTime.Now,
                    CreatedBy = "TestUser1"
                },
                CreatedDate = DateTime.Now,
                CreatedBy = "TestUser1"
            },
            new Team
            {
                Name = "AS Roma",
                Coach = new Coach
                {
                    Name = "Roma Coach",
                    CreatedDate = DateTime.Now,
                    CreatedBy = "TestUser1"
                },
                CreatedDate = DateTime.Now,
                CreatedBy = "TestUser1"
            }
        }
    };
    await context.AddAsync(league);
    await context.SaveChangesAsync();
    //NOTE: this is an all or nothing save.
}

//eager data loading - load related data //cip...78
//await EagerLoadingDataAsync();
async Task EagerLoadingDataAsync() //cip...78
{
    var leagues = await context.Leagues
        .Include(q => q.Teams)
            //or
            //.Include("Teams") <---"Teams" - the name of the navigation property.
            .ThenInclude(q => q.Coach)
        .ToListAsync(); //cip...78
    /*
        NOTE: the Include() and ThenInclude() statements genned joins:
        SELECT "l"."Id", "l"."CreatedBy", "l"."CreatedDate", "l"."ModifiedBy", "l"."ModifiedDate", "l"."Name", "s"."Id", "s"."CoachId", "s"."CreatedBy", "s"."CreatedDate", "s"."LeagueId", "s"."ModifiedBy", "s"."ModifiedDate", "s"."Name", "s"."Id0", "s"."CreatedBy0", "s"."CreatedDate0", "s"."ModifiedBy0", "s"."ModifiedDate0", "s"."Name0"
        FROM "Leagues" AS "l"
        LEFT JOIN (
            SELECT "t"."Id", "t"."CoachId", "t"."CreatedBy", "t"."CreatedDate", "t"."LeagueId", "t"."ModifiedBy", "t"."ModifiedDate", "t"."Name", "c"."Id" AS "Id0", "c"."CreatedBy" AS "CreatedBy0", "c"."CreatedDate" AS "CreatedDate0", "c"."ModifiedBy" AS "ModifiedBy0", "c"."ModifiedDate" AS "ModifiedDate0", "c"."Name" AS "Name0"
            FROM "Teams" AS "t"
            INNER JOIN "Coaches" AS "c" ON "t"."CoachId" = "c"."Id"
        ) AS "s" ON "l"."Id" = "s"."LeagueId"
        ORDER BY "l"."Id", "s"."Id"

        NOTE: if i omit .ThenInclude(q => q.Coach) then i'll get a null exception error when i try to print {team.Coach.Name}:
            Exception thrown: 'System.NullReferenceException' in EntityFrameworkCore.Console.dll
            Exception thrown: 'System.NullReferenceException' in System.Private.CoreLib.dll
    */
    foreach (var league in leagues)
    {
        Console.WriteLine($"League: {league.Name}");
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"\tTeam: {team.Name}\tCoach: {team.Coach.Name}");
        }
    }
}

//explicit data loading - load related data
//await ExplicitLoadingDataAsync();
async Task ExplicitLoadingDataAsync() //cip...78
{
    var league = await context.FindAsync<League>(1);
    //or
    //var league = await context.Leagues.FindAsync(1);
    if (league.Teams.Any())
    {
        Console.WriteLine($"League: {league.Name}");
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"\tTeam: {team.Name}");
        }
    }
    else
    {
        Console.WriteLine($"League: {league.Name} has no teams loaded.");
    }
    await context.Entry(league)
        .Collection(q => q.Teams)
        .LoadAsync(); //<--- league gets loaded with the Teams data.
    if (league.Teams.Any())
    {
        Console.WriteLine($"League: {league.Name}");
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"\tTeam: {team.Name}");
        }
    }
}

//lazy loading - load related data //cip...80
//await LazyLoadingData1Async();
async Task LazyLoadingData1Async() //cip...80
{
    var league = await context.FindAsync<League>(1);
    //or
    //var league = await context.Leagues.FindAsync(1);
    if (league.Teams.Any())
    {
        Console.WriteLine($"League: {league.Name}");
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"\tTeam: {team.Name}\tCoach: {team.Coach.Name}");
        }
    }
    else
    {
        Console.WriteLine($"League: {league.Name} has no teams loaded.");
    }
}

await LazyLoadingData2Async();
async Task LazyLoadingData2Async() //cip...80
{
    foreach (var league in context.Leagues)
    {
        Console.WriteLine($"League: {league.Name}");
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"\tTeam: {team.Name}\tCoach: {team.Coach.Name}");
        }
    }
}
#endregion

#region inserting data //cip...49
/* INSERT INTO Coaches (cols) VALUES (values) */

//simple insert //cip...49
//await InsertOneRecordAsync();
async Task InsertOneRecordAsync() //cip...49
{
    var newCoach = new Coach
    {
        Name = "Jose Mourinho",
        CreatedDate = DateTime.Now
    };
    await context.Coaches.AddAsync(newCoach);
    await context.SaveChangesAsync();
    //NOTE: always parameterise the queries so that ef can provise that extra layer of protection.
}

//loop insert //cip...49
//await InsertWithLoopAsync();
async Task InsertWithLoopAsync() //cip...49
{
    var newCoach = new Coach
    {
        Name = "Jose Mourinho",
        CreatedDate = DateTime.Now
    };

    var newCoach1 = new Coach
    {
        Name = "Theodore Whitmore",
        CreatedDate = DateTime.Now
    };

    List<Coach> coaches = new ()
    {
        newCoach,
        newCoach1
    };

    foreach (var coach in coaches)
    {
        await context.Coaches.AddAsync(coach);
    }
    //NOTE: it's personal preference whether i commit once or each time.
    Console.WriteLine("before save: " + context.ChangeTracker.DebugView.LongView);
    foreach (var coach in coaches)
        Console.WriteLine($"Coach ID: {coach.Id}, Coach: {coach.Name}, Created Date: {coach.CreatedDate}");

    await context.SaveChangesAsync();
    //the Id is returned to the object after the save.
    Console.WriteLine("after save: " + context.ChangeTracker.DebugView.LongView);

    foreach (var coach in coaches)
        Console.WriteLine($"Coach ID: {coach.Id}, Coach: {coach.Name}, Created Date: {coach.CreatedDate}");
}

//batch insert
//await InsertRangeAsync();
async Task InsertRangeAsync() //cip...49
{
    var newCoach = new Coach
    {
        Name = "Jose Mourinho",
        CreatedDate = DateTime.Now
    };

    var newCoach1 = new Coach
    {
        Name = "Theodore Whitmore",
        CreatedDate = DateTime.Now
    };

    List<Coach> coaches = new ()
    {
        newCoach,
        newCoach1
    };

    await context.Coaches.AddRangeAsync(coaches); //<---

    //NOTE: it's personal preference whether i commit once or each time.
    Console.WriteLine("before save: " + context.ChangeTracker.DebugView.LongView);
    foreach (var coach in coaches)
        Console.WriteLine($"Coach ID: {coach.Id}, Coach: {coach.Name}, Created Date: {coach.CreatedDate}");

    await context.SaveChangesAsync();
    //the Id is returned to the object after the save.
    Console.WriteLine("after save: " + context.ChangeTracker.DebugView.LongView);

    foreach (var coach in coaches)
        Console.WriteLine($"Coach ID: {coach.Id}, Coach: {coach.Name}, Created Date: {coach.CreatedDate}");
}
#endregion

//update operations //cip...50
//await UpdateAsync();
async Task UpdateAsync() //cip...50
{
    var coach = await context.Coaches.FindAsync(9); //NOTE: if the object's being tracked by the context, it will be returned. if not, it will be retrieved from the database.
    if (coach == null)
    {
        Console.WriteLine("Coach not found.");
        return;
    }
    //because i haven't tuenrned tracking off, coach is being tracked.
    coach.Name = $"harry potter ({DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond})";
    //it will only update the name because that was the only change.
    Console.WriteLine("before save 1: " + context.ChangeTracker.DebugView.LongView);
    await context.SaveChangesAsync();

    coach.Name = $"harry potter ({DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond})";
    coach.CreatedDate = DateTime.Now;
    //it will only update the name and createddate because both have changed.
    Console.WriteLine("before save 2: " + context.ChangeTracker.DebugView.LongView);
    await context.SaveChangesAsync();

    coach = await context.Coaches
        .AsNoTracking()
        //.FindAsync(8); //NOTE: fails to compile as FindAsync needs tracking to search the context first(??).
        .FirstOrDefaultAsync(q => q.Id == 8); //cip...50. NOTE: this is the same as FirstAsync() but returns null if no record is found.
    if (coach == null)
    {
        Console.WriteLine("Coach not found.");
        return;
    }
    coach.Name = $"harry potter-no tracking ({DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond})";
    //no tracking so nothing's changed.
    Console.WriteLine("before save 3: " + context.ChangeTracker.DebugView.LongView);
    await context.SaveChangesAsync();
    context.Update(coach); //cip...50. NOTE: it's not sure what changed so all the field(name)s, apart from the ID field, will be upadted.
    //or
    //context.UpdateRange(coaches);
    //it's also the equivalent as:
    //context.Entry(coach).State = EntityState.Modified;
    Console.WriteLine("before save 4: " + context.ChangeTracker.DebugView.LongView);
    await context.SaveChangesAsync();
    Console.WriteLine("after all updates: " + context.ChangeTracker.DebugView.LongView);
}

//update operations //cip...51
/* DELETE FROM Coaches WHERE Id = 4 */
//await DeleteAsync();
async Task DeleteAsync() //cip...51
{
    var coach = await context.Coaches.FindAsync(4); //NOTE: if the object's being tracked by the context, it will be returned. if not, it will be retrieved from the database.
    if (coach == null)
    {
        Console.WriteLine("Coach not found.");
        return;
    }
    //delete
    context.Remove(coach);
    //or
    //context.RemoveRange(coaches);
    //or
    //context.Entry(coach).State = EntityState.Deleted;
    await context.SaveChangesAsync();
}   

//update operations //cip...52
//await ExecuteDeleteAsync();
async Task ExecuteDeleteAsync() //cip...52
{
    // var coaches = await context.Coaches.Where(q => q.Name == "Theodore Whitmore").ToListAsync(); //NOTE: if the object's being tracked by the context, it will be returned. if not, it will be retrieved from the database.
    // if (coaches == null)
    // {
    //     Console.WriteLine("Coach not found.");
    //     return;
    // }
    // context.RemoveRange(coaches);
    // await context.SaveChangesAsync();
    //or
    await context.Coaches
        .Where(q => q.Name == "Theodore Whitmore")
        .ExecuteDeleteAsync(); //cip...52. NOTE: this is a new method in ef core 7.0. it will delete all records that meet the condition.
    // vscode help:
    //     (awaitable, extension) Task<int> IQueryable<Coach>.ExecuteDeleteAsync<Coach>([CancellationToken cancellationToken = default])
    //     Asynchronously deletes database rows for the entity instances which match the LINQ query from the database.

    //     This operation executes immediately against the database, rather than being deferred until DbContext.SaveChanges() is called. It also does not interact with the EF change tracker in any way: entity instances which happen to be tracked when this operation is invoked aren't taken into account, and aren't updated to reflect the changes.

    //     See Executing bulk operations with EF Core for more information and examples.

    //     Returns:
    //     The total number of rows deleted in the database.
}

//await ExecuteUpdateAsync();
async Task ExecuteUpdateAsync() //cip...52
{
    //NOTE: similar to ExecuteDeleteAsync
    await context.Coaches.Where(q => q.Name == "Jose Mourinho")
        .ExecuteUpdateAsync(set => set
            .SetProperty(prop => prop.Name, "Josep \"Pep\" Guardiola Sala")
            //.SetProperty(prop => prop.CreatedDate, DateTime.Now)
        ); //cip...52. NOTE: this is a new method in ef core 7.0. it will delete all records that meet the condition.}
}
//---------------------------------------------------------------------------
//---------------------------------------------------------------------------
//---------------------------------------------------------------------------
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
//await IQueryablesVsListTypesAsync();
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
