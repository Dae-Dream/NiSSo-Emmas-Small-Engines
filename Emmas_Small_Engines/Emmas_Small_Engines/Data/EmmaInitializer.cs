// Name:            Angel Mario Colorado Chairez
// Last Modified:   February 20, 2023
// Description:     Code to seed the database tables


using Emmas_Small_Engines.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Emmas_Small_Engines.Data
{
    public static class EmmaInitializer
    {
        private const int NUM_CUSTOMERS = 200;  // Number of Customers to be created
        private const int NUM_EMPLOYEES = 6;    // Number of Employees to be created
        private const int NUM_INVOICES = 1000;  // Number of Invoices to be created
        private const int NUM_ORDER_REQ = 5;    // Number of Order Requests to be created
        private const int NUM_EXT_ORDER = 2200; // Starting number for the External Order Number
        private const int MAX_ORD_REQ_PROD = 3; // Maximum number of products per Order Request Inventory
        private const int MAX_INVOICE_PROD = 10;// Maximum number of products per Invoice
        private const int DAYS_BEFORE = 4800;   // Number of days before to start the Invoices
        private const int NUM_EMP_LOGIN = 300; // Maximum number of Seed Login Details
        private const int NUM_SALES_REP = 12;   // Maximum number of Sales Reports

        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            EmmaSmallEngineContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<EmmaSmallEngineContext>();

            try
            {
                // Delete the database if you need to apply a new Migration
                //context.Database.EnsureDeleted();

                // Create the database if it does not exist
                context.Database.Migrate();

                //This approach to seeding data uses int and string arrays with loops to
                //create the data using random values
                Random random = new Random();

                //Prepare some string arrays for building objects
                string[] firstNames = new string[] { "Sergei", "Janine", "Anna", "Erik", "Heloise", "Alyssa", "Enrico", "Claude", "Franz", "Terrence", "Johnny", "Katie", "Alondra", "Ellis", "Ronin", "Alec", "Adeline", "Kendrick", "Jasmine", "Maya", "Amira", "Darren", "Maverick", "Jordan", "Michael", "Charlotte", "Solomon", "Ross", "Miles", "Darlene", "Ronnie", "Damian", "Shane", "Cristina", "Hallie", "Bella", "Hazel", "Julio", "Reece", "Aileen", "Sophia", "Fletcher", "Albert", "Rachael", "Ryan", "Sabrina", "Chloe", "Martin", "James", "Xander", "Emerson", "Ronan", "Jessica", "Gretchen", "Danny", "Pamela", "Brenda", "Catherine", "Jeremy", "Nathalia", "Trenton", "Dustin", "Paulina", "Dawson", "Davin", "Grace", "Jack", "Leo", "Desmond", "Giuliana", "Elisabeth", "Kinsley", "Saige" };
                int firstNamesCount = firstNames.Length;
                string[] lastNames = new string[] { "Rachmaninov", "Debussy", "Satie", "Prokofiev", "Bach", "Fedorova", "Vivaldi", "Horowitz", "Liszt", "Strauss", "Schubert", "Brahms", "Mendelssohn", "Schumann", "Wagner", "Verdi", "Colorado", "Shelton", "Larsen", "Pratt", "Cole", "Rasmussen", "Merritt", "Horton", "Marshall", "Stanton", "Lutz", "Schmitt", "Harrison", "Ford", "Clarke", "Harris", "Ryan", "Pham", "Harrell", "Whitney", "Keith", "Marks", "Black", "Mitchell", "Giles", "Strickland", "Preston", "Hendricks", "Haydn", "Jagger", "Wilkinson", "Kaiser", "Petersen", "Perry", "Quinn", "Conner", "Krause", "Wise", "Roth", "Brennan", "O'Connor", "Graves", "Perkins", "Mahoney", "Fox", "Byrd", "Oneill", "Weiss", "Copeland", "Johnson", "Ware", "Erickson", "Lyons", "Russo", "Barry", "Hatfield", "Johns", "Dudley", "Tyler", "Vincent", "Garner", "Lee", "David", "Randall", "Stein", "Heath", "Snyder", "Blanchard", "Johnston", "O'Donnell", "Walls", "Herring", "Alexander", "Ballard", "Hayes", "Patrick", "Donovan", "Austin", "Roberson", "Cook", "Frazier", "Hutchinson", "Pace", "Sheppard", "Townsend", "Carpenter" };
                int lastNamesCount = lastNames.Length;
                string[] streetsNames = new string[] { "Maple Street", "Main Street", "Park Street", "Church Street", "Jasper Avenue", "King Street", "Pine Street", "Oak Street", "Lakeview", "Queen Street", "James Street", "Stoney Creek", "Woodland" };
                string[] suppliersNames = new string[] { "Husqvarna", "Cub Cadet", "ACME", "Collins", "Honda", "Beringer", "Goldfren", "Safran", "Hutchinson", "Rexnor" };
                string[] emailsNames = new string[] { "info", "contact", "billing", "accounting", "sales" };

                // Cities and Provinces arrays have to be the same size and also match the City[i] with the Province[i]
                string[] citiesNames = new string[] { "Welland", "Calgary", "Vancouver", "Guelph", "Edmonton", "Reed Deer", "Toronto", "Victoria", "Arctic Bay", "Winnipeg" };
                string[] provincesNames = new string[] { "Ontario", "Alberta", "British Columbia", "Ontario", "Alberta", "Alberta", "Ontario", "British Columbia", "Nunavut", "Manitoba" };

                // Inventory items
                string[] inventoryItems = new string[] { "GPS Anti-Theft for Robotic Mower", "Lithium-ion Battery Charger", "Shredder & Chipper Blade", "RePlacement Wheel", "Grass Bag", "Engine Primer Bulbs", "Axle", "Drive shafts", "OHV Horizontal Replacement Gas Engine", "Engine Muffler", "Fuel Stabilizer", "Air filter", "Starter Cord", "Engine Spark Plug", "Throttle Control", "Muffler Assembly" };
                //string[] inventoryItems = new string[] { "GPS Anti-Theft for Robotic Mower", "RePlacement Belt", "Lithium-ion Battery Charger", "Power Share Pro Battery", "Shredder & Chipper Blade", "RePlacement Wheel", "Grass Bag", "Engine Primer Bulbs", "Lithium-Ion Battery", "Quantum engine Air Filter", "Anti-Collision System for Robotic Mower", "Universal handle mounting kit", "Axle", "Drive shafts", "OHV Horizontal Replacement Gas Engine", "Engine Muffler", "Replacement Gas CaP", "Blade Sharpening & Balancing Kit", "Sharpening Kit", "Fuel Stabilizer", "Air filter", "Fuel Filter", "Starter Cord", "Engine Spark Plug", "Key", "Cordless Lawn Mower Replacement Blade", "Throttle Control", "Muffler Assembly", "Inner Tire Tube", "Brake Cable" };

                // Payment Methods
                string[] paymentMethods = new string[] { "Cash", "Debit", "Credit", "Cheque" };

                // Customer
                if (!context.Customers.Any())
                {
                    // Generates a set of random strings, this way it will avoid to repeat Customer Names
                    HashSet<string> randomCustomers = new HashSet<string>();

                    while (randomCustomers.Count != NUM_CUSTOMERS)  // Until the count reaches the number
                    {
                        string fName = firstNames[random.Next(firstNamesCount)];
                        string lName = lastNames[random.Next(lastNamesCount)];

                        // If the Customer is successfully added to the HashSet it means is not duplicated
                        if (randomCustomers.Add(fName + lName))
                        {
                            // Generates a random number to pick a city and province
                            int numCiPr = random.Next(citiesNames.Length);

                            // Then creates the Customer
                            Customer c = new Customer()
                            {
                                FirstName = fName,
                                LastName = lName,

                                // For the phone, needed one more digit than a random int can generate, so concatenated 2 together as strings and then converted
                                Phone = random.Next(2, 10).ToString() + random.Next(213214131, 989898989).ToString(),

                                // Generates a random number for the street and picks one random street from the array
                                Address = random.Next(1, 60).ToString() + " " + streetsNames[random.Next(streetsNames.Length)],

                                // Pick one random city and province from the arrays
                                City = citiesNames[numCiPr],
                                Province = provincesNames[numCiPr],

                                // Generates a random postal code with [A - Z] and [0 - 9]
                                Postal = (char)random.Next(65, 91) + random.Next(10).ToString() + (char)random.Next(65, 91) + random.Next(10).ToString() + (char)random.Next(65, 91) + random.Next(10).ToString()
                            };
                            context.Customers.Add(c);
                        }
                    }
                    context.SaveChanges();
                }

                // Supplier
                if (!context.Suppliers.Any())
                {   // Loops through the arrays of names and builds the Supplier as we go

                    foreach (string sn in suppliersNames)
                    {   // Generates a random number to pick a city and province
                        int numCiPr = random.Next(citiesNames.Length);
                        Supplier s = new Supplier()
                        {
                            Name = sn,

                            // For the phone, needed one more digit than a random int can generate, so concatenated 2 together as strings and then converted
                            Phone = random.Next(2, 10).ToString() + random.Next(213214131, 989898989).ToString(),

                            // Generates a random e-mail address with the Supplier name as domain
                            Email = emailsNames[random.Next(emailsNames.Length)].ToLower() + "@" + String.Concat(sn.Where(c => !Char.IsWhiteSpace(c))).ToLower() + ".ca",

                            // Generates a random number for the street and picks one random street from the array
                            Address = random.Next(1, 500).ToString() + " " + streetsNames[random.Next(streetsNames.Length)],

                            // Pick one random city and province from the arrays
                            City = citiesNames[numCiPr],
                            Province = provincesNames[numCiPr],

                            // Generates a random postal code with [A - Z] and [0 - 9]
                            Postal = (char)random.Next(65, 91) + random.Next(10).ToString() + (char)random.Next(65, 91) + random.Next(10).ToString() + (char)random.Next(65, 91) + random.Next(10).ToString()
                        };
                        context.Suppliers.Add(s);
                    }
                    context.SaveChanges();
                }

                // Inventory
                if (!context.Inventories.Any())
                {
                    // Generates a set of random UPCs, this way it will avoid to repeat UPCs
                    HashSet<int> randomUPCs = new HashSet<int>();
                    int aNumber;

                    foreach (string i in inventoryItems)        // Loops through all the items
                    {
                        while (true)    // "Infinite" loop
                        {
                            aNumber = random.Next(100000000);   // Generates a random number

                            if (randomUPCs.Add(aNumber))        // If the number is not duplicated, will be added to the Set and will create the Object
                            {
                                // Generates a random price
                                double randPrice = random.Next(150) + Convert.ToDouble(random.Next(100)) / 100;

                                string qty = random.Next(1, 9).ToString();

                                if (qty != "1")
                                    qty += "-pack";

                                // Then creates the Inventory
                                Inventory inv = new Inventory()
                                {
                                    UPC = aNumber.ToString("00000000"),

                                    Name = i,

                                    Size = (random.Next(1, 17) * 2).ToString() + "\"",

                                    Quantity = qty,

                                    AdjustPrice = Math.Round(randPrice, 2),

                                    // From 10 to 50% extra to the original price
                                    MarkupPrice = Math.Round((randPrice * (1 + Convert.ToDouble(random.Next(10, 50 + 1)) / 100)), 2),

                                    // Get a random boolean
                                    Current = Convert.ToBoolean(random.Next(2)),

                                    Stock = random.Next(400)
                                };
                                context.Inventories.Add(inv);
                                break;      // Exits the while loop
                            }
                        }
                    }
                    context.SaveChanges();
                }

                // 2nd stage
                // Employee
                if (!context.Employees.Any())
                {
                    // Generates a set of random strings, this way it will avoid to repeat Employee Names
                    HashSet<string> randomEmployees = new HashSet<string>();

                    while (randomEmployees.Count != NUM_EMPLOYEES)  // Until the count reaches the number
                    {
                        string fName = firstNames[random.Next(firstNamesCount)];
                        string lName = lastNames[random.Next(lastNamesCount)];

                        // If the Employee is successfully added to the HashSet it means is not duplicated
                        if (randomEmployees.Add(fName + lName))
                        {   // Then creates the Employee
                            Employee e = new Employee()
                            {
                                FirstName = fName,
                                LastName = lName,
                                UserName = (fName + "." + lName + "@" + "emmas.ca").ToLower(),
                                Password = "password"
                            };
                            context.Employees.Add(e);
                        }
                    }
                    context.SaveChanges();
                }

                // Payment
                if (!context.Payments.Any())
                {
                    foreach (string p in paymentMethods)
                    {
                        Payment pay = new Payment()
                        {
                            Type = p,
                            Description = $"No description for {p}"
                        };
                        context.Payments.Add(pay);
                    }
                    context.SaveChanges();
                }

                // Create collection of the Primary Keys of the Customers
                int[] customerIDs = context.Customers.Select(c => c.ID).ToArray();
                int customerIDCount = customerIDs.Count();

                // Create collection of the Primary Keys of the Employees
                int[] employeeIDs = context.Employees.Select(e => e.ID).ToArray();
                int employeeIDCount = employeeIDs.Count();

                // OrderRequest
                if (!context.OrderRequests.Any())
                {
                    int c = NUM_EXT_ORDER;

                    for (int i = 1; i <= NUM_ORDER_REQ; i++)
                    {
                        int rand = random.Next(30, 180 + 1);

                        c++;

                        DateTime reqDate = DateTime.Today.AddDays(-random.Next(30, 180 + 1)).AddSeconds(random.Next(8 * 3600, 17 * 3600));

                        OrderRequest or = new OrderRequest()
                        {
                            CustomerID = customerIDs[random.Next(customerIDCount)],
                            Description = "No OrderRequest description",
                            SentDate = reqDate,                                     // Between 30 and 180 days ago
                            ReceiveDate = reqDate.AddDays(random.Next(3, 28 + 1)),    // Between 3 and 28 days after the SentDate
                            ExternalOrderNumber = $"EM-{c:0000}"
                        };
                        context.OrderRequests.Add(or);
                    }
                    context.SaveChanges();
                }

                // OrderRequestInventory
                int[] orderRequestIDs = context.OrderRequests.Select(o => o.ID).ToArray();
                string[] upcIDs = context.Inventories.Select(u => u.UPC).ToArray();
                int upcIDCount = upcIDs.Count();

                if (!context.OrderRequest_Inventories.Any())
                {
                    int idx = 0;

                    foreach (int o in orderRequestIDs)  // Loops through all the OrderRequests
                    {
                        // If you change the NUM_ORDER_REQ and the number of Products in the Inventory, you might need
                        //  to change the limits of the following random number
                        int maxNum = random.Next(1, MAX_ORD_REQ_PROD + 1);

                        for (int i = 1; i <= maxNum; i++, idx++)
                        {
                            OrderRequest_Inventory ori = new OrderRequest_Inventory()
                            {
                                OrderRequestID = o,
                                OrderAmount = random.Next(1, 3),
                                InventoryID = upcIDs[idx]
                            };
                            context.OrderRequest_Inventories.Add(ori);
                        }
                    }
                    context.SaveChanges();
                }

                int[] supplierIDs = context.Suppliers.Select(u => u.ID).ToArray();
                int supplierIDCount = supplierIDs.Count();

                // Price
                if (!context.Prices.Any())
                {
                    foreach (string u in upcIDs)    // Loops through all the UPCs
                    {
                        for (int i = 0; i <= random.Next(6); i++)    // Creates a "history" of prices for the UPC
                        {
                            int rand = random.Next(7, 1500 + 1);

                            Price pr = new Price()
                            {
                                InventoryID = u,
                                Cost = random.Next(100, 20000) / 100,
                                Date = DateTime.Today.AddDays(-rand).AddSeconds(random.Next(8 * 3600, 17 * 3600)),                   // Between 7 and 1,500 days ago
                                Count = random.Next(1, 50 + 1),
                                SupplierID = supplierIDs[random.Next(supplierIDCount)]  // Selects a random Supplier
                            };
                            context.Prices.Add(pr);
                        }
                    }
                    context.SaveChanges();
                }

                // Apparently the following 3 tables has to be populated on the fly:
                //  - Invoices
                //  - InvoicePayments
                //  - InvoiceLines

                // Cause the 2 last tables depend on the Invoices, which has a field (Subtotal) calculated
                //  from its InvoiceLines

                // Invoice
                //  The Subtotal is initialized with a negative value to note that it has to be updated from the Controller
                if (!context.Invoices.Any())
                {
                    for (int i = 1; i <= NUM_INVOICES; i++)
                    {
                        Invoice inv = new Invoice()
                        {
                            Date = DateTime.Today.AddDays(-random.Next(DAYS_BEFORE)).AddSeconds(random.Next(8 * 3600, 17 * 3600)),   // Between Today and DAYS_BEFORE
                            Appreciation = Convert.ToDouble(random.Next(10, 30 + 1)) / 100, // Between 10 and 30%
                            Description = $"No Invoice description {i:0000}",
                            Subtotal = -400,
                            CustomerID = customerIDs[random.Next(customerIDCount)],
                            EmployeeID = employeeIDs[random.Next(employeeIDCount)],
                        };
                        context.Invoices.Add(inv);
                    }
                    context.SaveChanges();
                }

                int[] invoiceIDs = context.Invoices.Select(i => i.ID).ToArray();
                int[] paymentIDs = context.Payments.Select(p => p.ID).ToArray();
                int paymentIDCount = paymentIDs.Count();

                // InvoicePayment
                if (!context.InvoicePayments.Any())
                {
                    foreach (int i in invoiceIDs)
                    {
                        InvoicePayment ip = new InvoicePayment()
                        {
                            InvoiceID = i,
                            PaymentID = paymentIDs[random.Next(paymentIDCount)]
                        };
                        context.InvoicePayments.Add(ip);
                    }
                    context.SaveChanges();
                }

                // InvoiceLine
                if (!context.InvoiceLines.Any())
                {
                    foreach (int i in invoiceIDs)
                    {
                        // Generates a set of random strings, this way it will avoid to repeat Products
                        HashSet<string> randomUpcIDs = new HashSet<string>();

                        int maxNum = random.Next(2, MAX_INVOICE_PROD + 1);

                        // From 2 to MAX_INVOICE_PROD products per Invoice
                        while (randomUpcIDs.Count != maxNum)  // Until the count reaches the number
                        {
                            randomUpcIDs.Add(upcIDs[random.Next(upcIDCount)]);
                        }

                        foreach (string u in randomUpcIDs)
                        {
                            InvoiceLine il = new InvoiceLine()
                            {
                                InvoiceID = i,
                                InventoryID = u,
                                Quantity = random.Next(1, 6),
                                SalePrice = Convert.ToDouble(random.Next(1000, 100000)) / 100,
                            };
                            context.InvoiceLines.Add(il);
                        }
                    }
                    context.SaveChanges();
                }
                // EmpLogins
                if (!context.EmpLogins.Any())
                {
                    for (int i = 1; i <= NUM_EMP_LOGIN; i++)
                    {
                        DateTime endDate = DateTime.Today.AddDays(-random.Next(75)).Date;
                        endDate = endDate.AddHours(18);
                        DateTime startDate = endDate.AddHours(-random.Next(6, 12));

                    EmpLogin el = new EmpLogin()
                    {

                        WorkDate = startDate,
                        SignIn = startDate,
                        SignOut = endDate,
                        EmployeeID = employeeIDs[random.Next(employeeIDCount)],
                        HourlyReportID = null

                    };
                        context.EmpLogins.Add(el);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
