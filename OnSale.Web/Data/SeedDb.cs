using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;
using OnSale.Common.Enums;
using OnSale.Web.Data.Entities;
using OnSale.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly Random _random;


        public SeedDb(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheFairesAsync();
            await CheckUserAsync("1010", "Leidy", "Suarez", "leidyjoha13@hotmail.com", "355 654 67 76", "Calle 15", UserType.Admin);
            await CheckUserAsync("1011", "Juan", "Henao", "juanHenao@yopmail.com", "355 4466 666", "Calle 12", UserType.User);
            await CheckCowsAsync();

        }

        private async Task CheckCowsAsync()
        {
            if (!_context.Cows.Any())
            {
                User user = await _userHelper.GetUserAsync("juanHenao@yopmail.com");
                City medellin = await _context.Cities.FirstOrDefaultAsync(m => m.Name == "Medellín");
                City cali = await _context.Cities.FirstOrDefaultAsync(c => c.Name == "Calí");

                Fair ganadoGordo = await _context.Faires.FirstOrDefaultAsync(c => c.Name == "Ganado Gordo");
                Fair ganadoFlaco = await _context.Faires.FirstOrDefaultAsync(c => c.Name == "Ganado Flaco");
                Fair ganadoIndustrial = await _context.Faires.FirstOrDefaultAsync(c => c.Name == "Ganado Industrial");
                Fair revoltura = await _context.Faires.FirstOrDefaultAsync(c => c.Name == "Revoltura");


                string description = "This cow is great and its a lot meats";
                await AddCowAsync(ganadoGordo, description, "La Topa", 2500000M, new string[] { }, user, medellin, Sex.male);
                await AddCowAsync(ganadoFlaco, description, "Charamusca", 95000M, new string[] { }, user, medellin, Sex.male);
                await AddCowAsync(ganadoIndustrial, description, "La tomasa", 3500000M, new string[] { }, user, cali, Sex.female);
                await AddCowAsync(revoltura, description, "Carlina", 250000M, new string[] {}, user, cali, Sex.female);
                await AddCowAsync(ganadoFlaco, description, "Nieves", 26000000M, new string[] {}, user, cali, Sex.female);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddCowAsync(Fair fair, string description, string name, decimal price, string[] images, User user, City city, Sex sex)
        {
            Cow cow = new Cow
            {

                Name = name,
                Description = description,
                Owner = user,
                Fair = fair,
                city = city,
                IsActive = true,
                Price = price,
                sex = sex,
                ProductImages = new List<CowImage>(),
                 
                Qualifications = GetRandomQualifications(description, user)
            };

            foreach (string image in images)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images", $"{image}.png");
                Guid imageId = await _blobHelper.UploadBlobAsync(path, "products");
                cow.ProductImages.Add(new CowImage { ImageId = imageId });
            }

            _context.Cows.Add(cow);
        }

        private ICollection<Qualification> GetRandomQualifications(string description, User user)
        {
            List<Qualification> qualifications = new List<Qualification>();
            for (int i = 0; i < 10; i++)
            {
                qualifications.Add(new Qualification
                {
                    Date = DateTime.UtcNow,
                    Remarks = description,
                    Score = _random.Next(1, 5),
                    User = user
                });
            }

            return qualifications;
        }

        private async Task CheFairesAsync()
        {
            if (!_context.Faires.Any())
            {
                await AddCategoryAsync("Ganado Gordo");
                await AddCategoryAsync("Ganado Flaco");
                await AddCategoryAsync("Ganado Industrial");
                await AddCategoryAsync("Revoltura");
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddCategoryAsync(string name)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images", $"{name}.png");
            Guid imageId = await _blobHelper.UploadBlobAsync(path, "categories");
            _context.Faires.Add(new Fair { Name = name, ImageId = imageId });
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    Departments = new List<Department>
                {
                    new Department
                    {
                        Name = "Antioquia",
                        Cities = new List<City>
                        {
                            new City { Name = "Medellín" },
                            new City { Name = "Envigado" },
                            new City { Name = "Itagüí" }
                        }
                    },
                    new Department
                    {
                        Name = "Bogotá",
                        Cities = new List<City>
                        {
                            new City { Name = "Bogotá" }
                        }
                    },
                    new Department
                    {
                        Name = "Valle del Cauca",
                        Cities = new List<City>
                        {
                            new City { Name = "Calí" },
                            new City { Name = "Buenaventura" },
                            new City { Name = "Palmira" }
                        }
                    }
                }
                });
                _context.Countries.Add(new Country
                {
                    Name = "USA",
                    Departments = new List<Department>
                {
                    new Department
                    {
                        Name = "California",
                        Cities = new List<City>
                        {
                            new City { Name = "Los Angeles" },
                            new City { Name = "San Diego" },
                            new City { Name = "San Francisco" }
                        }
                    },
                    new Department
                    {
                        Name = "Illinois",
                        Cities = new List<City>
                        {
                            new City { Name = "Chicago" },
                            new City { Name = "Springfield" }
                        }
                    }
                }
                });
                await _context.SaveChangesAsync();
            }
        }
    }

}
