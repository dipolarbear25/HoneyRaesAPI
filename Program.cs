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
        Description = "Car stuck on interstate",
        Emergency = false,
    },
      new ServiceTicket
    {
        Id = 2,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "Hip is dislocated",
        Emergency = true,
        DateCompleted = new DateTime(2023,12,2)
    },
      new ServiceTicket
    {
        Id = 3,
        CustomerId = 1,
        EmployeeId = 2,
        Description = "'Tis but a scratch",
        Emergency = false,
    },
      new ServiceTicket
    {
        Id = 4,
        CustomerId = 2,
        EmployeeId = 3,
        Description = "Need reinforcements",
        Emergency = true,
        DateCompleted = new DateTime(2022,12,20)
    },
    new ServiceTicket
    {
        Id = 5,
        CustomerId = 2,
        EmployeeId = null,
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

app.MapGet("/api/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/api/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/api/employee", () =>
{
    return employees;
});

app.MapGet("/api/employee/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/api/customer", () =>
{
    return customers;
});

app.MapGet("/api/customer/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(e => e.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(customer);
});

app.MapPost("/api/servicetickets", (ServiceTicket serviceTicket) =>
{
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
}
);

app.MapDelete("/api/servicetickets/{id}", (int id) =>
{
    ServiceTicket ticketToRemove = serviceTickets.FirstOrDefault(st => st.Id == id);
    serviceTickets.Remove(ticketToRemove);
});

app.MapPut("/api/servicetickets/{id}", (int id, ServiceTicket updatedServiceTicket) =>
{
    ServiceTicket existingServiceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    existingServiceTicket.EmployeeId = updatedServiceTicket.EmployeeId;

    if (existingServiceTicket == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(existingServiceTicket);

});

app.MapPost("/api/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
    return Results.Ok("This ticket is now completed");
});

app.MapGet("/api/servicetickets/incompleteEmergencies", () =>
{
    List<ServiceTicket> IncompleteEmergencies = serviceTickets.Where(st => st.DateCompleted == DateTime.MinValue && st.Emergency == true).ToList();
    if (IncompleteEmergencies.Count == 0)
    {
        return Results.NotFound("No emergencies found");
    }
    return Results.Ok(IncompleteEmergencies);
});

app.MapGet("/api/servicetickets/unassigned", () =>
{
    List<ServiceTicket> UnassignedTickets = serviceTickets.Where(st => st.EmployeeId == null).ToList();
    if (UnassignedTickets.Count == 0)
    {
        return Results.NotFound("All tickets have been assigned");
    }
    return Results.Ok(UnassignedTickets);
}
);

app.MapGet("/api/customer/inactive", () =>
{
    List<Customer> inactiveCustomers = customers.Where(c => serviceTickets.Any(st => st.CustomerId == c.Id && (st.DateCompleted == DateTime.MinValue || st.DateCompleted == DateTime.Now.AddYears(-1)))).ToList();

    if (inactiveCustomers != null)
    {
        return Results.Ok(inactiveCustomers);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapGet("/api/employee/available", () =>
{
    List<Employee> unassignedEmployees = employees.Where(e => !serviceTickets.Any(st => st.EmployeeId == e.Id && st.DateCompleted == DateTime.MinValue)).ToList();
    return unassignedEmployees;

});


app.MapGet("/api/employee/{id}/customers", (int id) =>
{

    Employee employee = employees.FirstOrDefault(e => e.Id == id);

    if (employee == null)
    {
        return Results.NotFound("Employee not found");
    }

    List<ServiceTicket> employeeServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();

    if (employeeServiceTickets.Count == 0)
    {
        return Results.Ok("This employee has no customers.");
    }

    List<int> customerIds = employeeServiceTickets.Select(st => st.CustomerId).Distinct().ToList();

    List<Customer> customersAssignedToEmployee = customers.Where(c => customerIds.Contains(c.Id)).ToList();

    return Results.Ok(customersAssignedToEmployee);
});


app.MapGet("/api/employee/topEmployee", () =>
{
    DateTime lastMonth = DateTime.Now.AddMonths(-1);
    Employee employeeOfMonth = employees.OrderByDescending(e => serviceTickets.Count(st => st.EmployeeId == e.Id && st.DateCompleted.HasValue && st.DateCompleted.Value.Month == lastMonth.Month)).FirstOrDefault();

    return Results.Ok(employeeOfMonth);
});

app.MapGet("/api/servicetickets/review", () =>
{
    List<ServiceTicket> completedTickets = serviceTickets.Where(st => st.DateCompleted.HasValue).OrderBy(st => st.DateCompleted).ToList();

    foreach (var ticket in completedTickets)
    {
        ticket.Customer = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
        ticket.Employee = employees.FirstOrDefault(e => e.Id == ticket.EmployeeId);
    }

    return Results.Ok(completedTickets);
});

app.Run();