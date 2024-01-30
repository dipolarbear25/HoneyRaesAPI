using HoneyRaesAPI.Models;

List<Customer> customers = new List<Customer>
{
    new Customer
    {
        Id = 1,
        Name = "Cory Taylor",
        Address = "Knoxville, Tennessee"
    },
    new Customer
    {
        Id = 2,
        Name = "Brian Wille",
        Address = "Newtown, Connecticut"
     },
    new Customer
    {
        Id = 3,
        Name = "James Hetfield",
        Address = "Downey, California"
    }
};
List<Employee> employees = new List<HoneyRaesAPI.Models.Employee>
{
    new Employee
    {
        Id = 1,
        Name = "Joey Jordison",
        Description = "Kid Wonder",
        Specialty = "Major Drummer"
    },
    new Employee
    {
        Id = 2,
        Name = "Matt Young",
        Description = "an actual beast",
        Specialty = "Somehow can make the drums sing"
    },
    new Employee
    {
        Id = 3,
        Name = "Lars Ulrich",
        Description = "The God",
        Specialty = "Invented the drums"
    },
    new Employee
    {
        Id = 4,
        Name = "Ben Harclerode",
        Description = "Satan's personal drummer",
        Specialty = "can play the drums blindfolded upside down"
    }
};
List<ServiceTicket> serviceTickets = new List<HoneyRaesAPI.Models.ServiceTicket>
{
      new ServiceTicket
    {
        Id = 1,
        CustomerId = 2,
        EmployeeId = null,
        Description = "HeLp",
        Emergency = true,
    },
      new ServiceTicket
    {
        Id = 2,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "EZ PZ",
        Emergency = false,
        DateCompleted = new DateTime(2023,12,2)
    },
      new ServiceTicket
    {
        Id = 3,
        CustomerId = 1,
        Description = "'Tis but a scratch",
        Emergency = false,
    },
      new ServiceTicket
    {
        Id = 4,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "Code Red",
        Emergency = true,
        DateCompleted = new DateTime(2022,12,20)
    },
    new ServiceTicket
    {
        Id = 5,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Checkup",
        Emergency = false,

    },
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/employee", () =>
{
    return employees;
});

app.MapGet("/employee/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/customer", () =>
{
    return customers;
});

app.MapGet("/customer/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(e => e.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(customer);
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
}
);

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket ticketToRemove = serviceTickets.Where(st => st.Id == id).FirstOrDefault();
    serviceTickets.Remove(ticketToRemove);
});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket updatedServiceTicket) =>
{
    ServiceTicket existingServiceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    existingServiceTicket.EmployeeId = updatedServiceTicket.EmployeeId;

    if (existingServiceTicket == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(existingServiceTicket);

});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
    return Results.Ok("Ticket marked complete");
});

app.Run();