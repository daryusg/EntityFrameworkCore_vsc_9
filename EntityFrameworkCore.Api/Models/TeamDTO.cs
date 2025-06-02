using System;

namespace EntityFrameworkCore.Api.Models;

public class TeamDTO //cip...104 (Data Transfer Object)
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string CoachName { get; set; }
}
