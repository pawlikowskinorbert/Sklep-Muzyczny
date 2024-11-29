using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Specifications;

namespace Project.Helpers
{
	public static class SeedData
	{
		public static async Task InitializeAsync(IServiceProvider serviceProvider)
		{
			using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

			using var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
			using var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			if (!await context.Users.AnyAsync())
			{
				await SeedRolesAsync(roleManager);
				await SeedUsersAsync(userManager);
			}

			if (await context.Products.AnyAsync())
			{
				return;
			}

			await SeedCategoriesAsync(context);
			await SeedProductTypesAsync(context);
			await SeedBrandsAsync(context);
			await SeedSpecificationDefinitionsAsync(context);
			await SeedProductsAsync(context);

		}

		private static async Task SeedCategoriesAsync(ApplicationDbContext context)
		{
			var categories = new[]
			{
				new Category {Name = "Guitars", PhotoUrl="../assets/Categories/Category-Guitars.jpg"},
				new Category {Name = "Drums", PhotoUrl="../assets/Categories/Category-Drums.jpg"},
				new Category {Name = "Pianos And Keyboards", PhotoUrl="../assets/Categories/Category-Pianos-And-Keyboards.jpg"},
				new Category {Name = "Bass Guitars", PhotoUrl="../assets/Categories/Category-Bass-Guitars.jpg"}
			};

			context.Categories.AddRange(categories);
			await context.SaveChangesAsync();
		}



		private static async Task SeedProductTypesAsync(ApplicationDbContext context)
		{
			var categories = await context.Categories.ToListAsync();

			var productTypes = new[]
			{
				new ProductType {Name="Acoustic Guitars", CategoryId = categories.First(c =>c.Name == "Guitars").Id, PhotoUrl="../assets/ProductTypes/Type-Acoustic-Guitar.jpg"},
				new ProductType {Name="Electric Guitars", CategoryId = categories.First(c =>c.Name == "Guitars").Id, PhotoUrl="../assets/ProductTypes/Type-Electric-Guitar.jpg"},
				new ProductType {Name="Acoustic Bass", CategoryId = categories.First(c =>c.Name == "Bass Guitars").Id, PhotoUrl="../assets/ProductTypes/Type-Acoustic-Bass.jpg"},
				new ProductType {Name="Electric Bass", CategoryId = categories.First(c =>c.Name == "Bass Guitars").Id, PhotoUrl="../assets/ProductTypes/Type-Electric-Bass.jpg"},
				new ProductType {Name="Acoustic Drums", CategoryId = categories.First(c =>c.Name == "Drums").Id, PhotoUrl="../assets/ProductTypes/Type-Acoustic-Drums.jpg"},
				new ProductType {Name="Electric Drums", CategoryId = categories.First(c =>c.Name == "Drums").Id, PhotoUrl="../assets/ProductTypes/Type-Electric-Drums.jpg"},
				new ProductType {Name="Grand Pianos", CategoryId = categories.First(c =>c.Name == "Pianos And Keyboards").Id, PhotoUrl="../assets/ProductTypes/Type-Grand-Piano.jpg"},
				new ProductType {Name="Keyboards", CategoryId = categories.First(c =>c.Name == "Pianos And Keyboards").Id, PhotoUrl="../assets/ProductTypes/Type-Keyboard-Piano.jpg"},

			};

			context.ProductTypes.AddRange(productTypes);
			await context.SaveChangesAsync();
		}

		private static async Task SeedBrandsAsync(ApplicationDbContext context)
		{
			var brands = new[]
			{
				new Brand { Name = "Yamaha", PhotoUrl="../assets/Brands/Brand-Yamaha.jpg" },
				new Brand { Name = "Fender", PhotoUrl="../assets/Brands/Brand-Fender.jpg" },
				new Brand { Name = "Gibson", PhotoUrl="../assets/Brands/Brand-Gibson.jpg"},
				new Brand { Name = "Jackson", PhotoUrl="../assets/Brands/Brand-Jackson.jpg" },
				new Brand { Name = "Tama", PhotoUrl="../assets/Brands/Brand-Tama.jpg" },
				new Brand { Name = "Pearl", PhotoUrl="../assets/Brands/Brand-Pearl.jpg" },
			};

			context.Brands.AddRange(brands);
			await context.SaveChangesAsync();
		}

		private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			string[] roleNames = { "User", "Admin", "Moderator" };

			foreach (var role in roleNames)
			{
				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}
		}

		private static async Task SeedSpecificationDefinitionsAsync(ApplicationDbContext context)
		{
			var productTypes = await context.ProductTypes.ToListAsync();

			var specificationDefinitions = new List<SpecificationDefinition>();

			var acousticGuitars = productTypes.First(pt => pt.Name == "Acoustic Guitars");

			specificationDefinitions.AddRange(new[]
			{
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "IsRightHanded", DisplayName = "Praworęczna / Leworęczna", DataType = "enum", IsRequired = true, Options = "Praworęczna, Leworęczna" },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "Size", DisplayName = "Rozmiar", DataType = "enum", IsRequired = true, Options = "4/4 (powyżej 10 lat), 3/4, 1/2" },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "TopMaterial", DisplayName = "Płyta wierzchnia", DataType = "enum", IsRequired = true, Options = "Świerk, Mahoniowy, Cedr" },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "BackSidesMaterial", DisplayName = "Spód / boki", DataType = "enum", IsRequired = true, Options = "Mahoń, Palisander, Limba" },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "Neck", DisplayName = "Gryf", DataType = "enum", IsRequired = true, Options = "Nato, Palisander, Mahoniowy" },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "Fingerboard", DisplayName = "Podstrunnica", DataType = "enum", IsRequired = true, Options = "Ciemny orzech, Jasny orzech, Palisander" },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "Cutaway", DisplayName = "Cutaway", DataType = "boolean", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "NumberOfStrings", DisplayName = "Ilość strun", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "Electronics", DisplayName = "Elektronika", DataType = "boolean", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "Color", DisplayName = "Kolor", DataType = "enum", IsRequired = true, Options = "Naturalny, Czerwony, Czarny, Biały" },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "Finish", DisplayName = "Wykończenie", DataType = "enum", IsRequired = true, Options = "Wysoki połysk, Matowy, Satin" },
				new SpecificationDefinition { ProductTypeId = acousticGuitars.Id, Name = "MusicStyles", DisplayName = "Style muzyczne", DataType = "string", IsRequired = false },
			});

			var electricGuitars = productTypes.First(pt => pt.Name == "Electric Guitars");
			specificationDefinitions.AddRange(new[]
			{
				new SpecificationDefinition { ProductTypeId = electricGuitars.Id, Name = "IsRightHanded", DisplayName = "Praworęczna / Leworęczna", DataType = "enum", IsRequired = true, Options = "Praworęczna, Leworęczna" },
				new SpecificationDefinition { ProductTypeId = electricGuitars.Id, Name = "BodyMaterial", DisplayName = "Materiał korpusu", DataType = "enum", IsRequired = true, Options = "Olcha, Mahoń, Lipowe, Klon" },
				new SpecificationDefinition { ProductTypeId = electricGuitars.Id, Name = "NeckMaterial", DisplayName = "Materiał gryfu", DataType = "enum", IsRequired = true, Options = "Klon, Palisander, Mahoń" },
				new SpecificationDefinition { ProductTypeId = electricGuitars.Id, Name = "FingerboardMaterial", DisplayName = "Materiał podstrunnicy", DataType = "enum", IsRequired = true, Options = "Palisander, Klon, Heban" },
				new SpecificationDefinition { ProductTypeId = electricGuitars.Id, Name = "NumberOfFrets", DisplayName = "Ilość progów", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = electricGuitars.Id, Name = "PickupConfiguration", DisplayName = "Konfiguracja przetworników", DataType = "enum", IsRequired = true, Options = "HSS, HH, HSH, SSS" },
				new SpecificationDefinition { ProductTypeId = electricGuitars.Id, Name = "Bridge", DisplayName = "Mostek", DataType = "enum", IsRequired = true, Options = "Fixed, Tremolo, Floyd Rose" },
				new SpecificationDefinition { ProductTypeId = electricGuitars.Id, Name = "Color", DisplayName = "Kolor", DataType = "enum", IsRequired = true, Options = "Czarny, Biały, Czerwony, Niebieski, Naturalny" },
				new SpecificationDefinition { ProductTypeId = electricGuitars.Id, Name = "Finish", DisplayName = "Wykończenie", DataType = "enum", IsRequired = true, Options = "Wysoki połysk, Matowy, Satin" },
			});
			var acousticBass = productTypes.First(pt => pt.Name == "Acoustic Bass");
			specificationDefinitions.AddRange(new[]
			{
				new SpecificationDefinition { ProductTypeId = acousticBass.Id, Name = "IsRightHanded", DisplayName = "Praworęczna / Leworęczna", DataType = "enum", IsRequired = true, Options = "Praworęczna, Leworęczna" },
				new SpecificationDefinition { ProductTypeId = acousticBass.Id, Name = "BodyMaterial", DisplayName = "Materiał korpusu", DataType = "enum", IsRequired = true, Options = "Świerk, Mahoń, Klon" },
				new SpecificationDefinition { ProductTypeId = acousticBass.Id, Name = "NumberOfStrings", DisplayName = "Ilość strun", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = acousticBass.Id, Name = "Electronics", DisplayName = "Elektronika", DataType = "boolean", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = acousticBass.Id, Name = "Fretless", DisplayName = "Bezprogowy", DataType = "boolean", IsRequired = false },
			});


			var electricBass = productTypes.First(pt => pt.Name == "Electric Bass");
			specificationDefinitions.AddRange(new[]
			{
				new SpecificationDefinition { ProductTypeId = electricBass.Id, Name = "IsRightHanded", DisplayName = "Praworęczny / Leworęczny", DataType = "enum", IsRequired = true, Options = "Praworęczny, Leworęczny" },
				new SpecificationDefinition { ProductTypeId = electricBass.Id, Name = "BodyMaterial", DisplayName = "Materiał korpusu", DataType = "enum", IsRequired = true, Options = "Olcha, Mahoń, Lipowe, Klon" },
				new SpecificationDefinition { ProductTypeId = electricBass.Id, Name = "NeckMaterial", DisplayName = "Materiał gryfu", DataType = "enum", IsRequired = true, Options = "Klon, Palisander, Mahoń" },
				new SpecificationDefinition { ProductTypeId = electricBass.Id, Name = "FingerboardMaterial", DisplayName = "Materiał podstrunnicy", DataType = "enum", IsRequired = true, Options = "Palisander, Klon, Heban" },
				new SpecificationDefinition { ProductTypeId = electricBass.Id, Name = "NumberOfStrings", DisplayName = "Ilość strun", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = electricBass.Id, Name = "Fretless", DisplayName = "Bezprogowa", DataType = "boolean", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = electricBass.Id, Name = "PickupType", DisplayName = "Typ przetworników", DataType = "enum", IsRequired = true, Options = "Single Coil, Humbucker" },
			});

			var acousticDrums = productTypes.First(pt => pt.Name == "Acoustic Drums");
			specificationDefinitions.AddRange(new[]
			{
				new SpecificationDefinition { ProductTypeId = acousticDrums.Id, Name = "NumberOfPieces", DisplayName = "Ilość elementów", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = acousticDrums.Id, Name = "ShellMaterial", DisplayName = "Materiał korpusów", DataType = "enum", IsRequired = true, Options = "Klon, Mahoń, Brzoza" },
				new SpecificationDefinition { ProductTypeId = acousticDrums.Id, Name = "IncludedHardware", DisplayName = "W zestawie osprzęt", DataType = "boolean", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = acousticDrums.Id, Name = "IncludedCymbals", DisplayName = "W zestawie talerze", DataType = "boolean", IsRequired = true },
			});

			var electricDrums = productTypes.First(pt => pt.Name == "Electric Drums");
			specificationDefinitions.AddRange(new[]
			{
				new SpecificationDefinition { ProductTypeId = electricDrums.Id, Name = "NumberOfPads", DisplayName = "Ilość padów", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = electricDrums.Id, Name = "IncludesKickPad", DisplayName = "W zestawie pad stopy", DataType = "boolean", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = electricDrums.Id, Name = "ModuleSounds", DisplayName = "Ilość brzmień w module", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = electricDrums.Id, Name = "USBConnectivity", DisplayName = "Łączność USB", DataType = "boolean", IsRequired = true },
			});

			var grandPianos = productTypes.First(pt => pt.Name == "Grand Pianos");
			specificationDefinitions.AddRange(new[]
			{
				new SpecificationDefinition { ProductTypeId = grandPianos.Id, Name = "NumberOfKeys", DisplayName = "Ilość klawiszy", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = grandPianos.Id, Name = "Length", DisplayName = "Długość (cm)", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = grandPianos.Id, Name = "Finish", DisplayName = "Wykończenie", DataType = "enum", IsRequired = true, Options = "Wysoki połysk, Matowy" },
				new SpecificationDefinition { ProductTypeId = grandPianos.Id, Name = "Color", DisplayName = "Kolor", DataType = "enum", IsRequired = true, Options = "Czarny, Biały, Brązowy" },
			});

			var keyboards = productTypes.First(pt => pt.Name == "Keyboards");
			specificationDefinitions.AddRange(new[]
			{
				new SpecificationDefinition { ProductTypeId = keyboards.Id, Name = "NumberOfKeys", DisplayName = "Ilość klawiszy", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = keyboards.Id, Name = "TouchSensitive", DisplayName = "Dynamika klawiatury", DataType = "boolean", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = keyboards.Id, Name = "Polyphony", DisplayName = "Polifonia", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = keyboards.Id, Name = "NumberOfVoices", DisplayName = "Ilość brzmień", DataType = "number", IsRequired = true },
				new SpecificationDefinition { ProductTypeId = keyboards.Id, Name = "USBConnectivity", DisplayName = "Łączność USB", DataType = "boolean", IsRequired = true },
			});


			context.SpecificationDefinitions.AddRange(specificationDefinitions);
			await context.SaveChangesAsync();
		}

		private static async Task SeedUsersAsync(UserManager<AppUser> userManager)
		{


			var users = new[]
			{
				new  {UserName = "Admin", FirstName = "Admin", LastName = "Admin", Email= "admin@123", Password = "Admin@123", Role = "Admin"},
				new  {UserName = "Pudzian", FirstName = "Marcin", LastName = "Pudzianowski", Email= "pudzian@123", Password = "Pudzian@123", Role = "User"},
				new  {UserName = "Malysz", FirstName = "Adam", LastName = "Malysz", Email= "malysz@123", Password = "Malysz@123" , Role = "User" },
				new  {UserName = "Kubica", FirstName = "Robert", LastName = "Kubica", Email= "kubica@123", Password = "Kubica@123" , Role = "Moderator"},
			};


			foreach (var userInfo in users)
			{
				if (await userManager.FindByEmailAsync(userInfo.Email) == null)
				{
					var user = new AppUser { UserName = userInfo.UserName, FirstName = userInfo.FirstName, LastName = userInfo.LastName, Email = userInfo.Email };
					var result = await userManager.CreateAsync(user, userInfo.Password);

					if (result.Succeeded)
					{
						await userManager.AddToRoleAsync(user, userInfo.Role);
					}
				}
			}
		}

		private static async Task SeedProductsAsync(ApplicationDbContext context)
		{
			var brands = await context.Brands.ToListAsync();
			var productTypes = await context.ProductTypes.ToListAsync();
			var specificationDefinitions = await context.SpecificationDefinitions
				.Include(sd => sd.ProductType)
				.ToListAsync();



			Console.WriteLine($"Znaleziono {brands.Count} marek.");
			Console.WriteLine($"Znaleziono {productTypes.Count} typów produktów.");
			Console.WriteLine($"Znaleziono {specificationDefinitions.Count} definicji specyfikacji.");

			var products = new[]
			{
		// **Acoustic Guitars**
		new Product
		{
			Name = "Fender FA-25",
			Description = "Solid-top acoustic guitar with authentic sound.",
			Price = 199.99M,
			QuantityInStoct = 10,
			ProductTypeId = productTypes.First(pt => pt.Name == "Acoustic Guitars").Id,
			BrandId = brands.First(b => b.Name == "Fender").Id,
			PhotoUrl = "../assets/Products/Acoustic-Guitar-FenderFA25.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "IsRightHanded"), Value = "Praworęczna" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Size"), Value = "4/4 (powyżej 10 lat)" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "TopMaterial"), Value = "Świerk" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "BackSidesMaterial"), Value = "Mahoń" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Neck"), Value = "Nato" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Fingerboard"), Value = "Ciemny orzech" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Cutaway"), Value = "false" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "NumberOfStrings"), Value = "6" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Electronics"), Value = "false" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Color"), Value = "Naturalny" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Finish"), Value = "Wysoki połysk" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "MusicStyles"), Value = "Klasyczna, Jazz" },
			}


		},
		new Product
		{
			Name = "Gibson CD-60S",
			Description = "Affordable dreadnought acoustic guitar with great tone.",
			Price = 229.99M,
			QuantityInStoct = 8,
			ProductTypeId = productTypes.First(pt => pt.Name == "Acoustic Guitars").Id,
			BrandId = brands.First(b => b.Name == "Gibson").Id,
			PhotoUrl = "../assets/Products/Acoustic-Guitar-GibsonJ45.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "IsRightHanded"), Value = "Praworęczna" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Size"), Value = "4/4 (powyżej 10 lat)" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "TopMaterial"), Value = "Cedr" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "BackSidesMaterial"), Value = "Palisander" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Neck"), Value = "Mahoniowy" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Fingerboard"), Value = "Palisander" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Cutaway"), Value = "true" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "NumberOfStrings"), Value = "6" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Electronics"), Value = "true" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Color"), Value = "Czarny" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "Finish"), Value = "Matowy" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Guitars", "MusicStyles"), Value = "Rock, Blues" },
			}
		},
		// **Electric Guitars**
		new Product
		{
			Name = "Fender Stratocaster",
			Description = "Classic electric guitar known for its bright sound.",
			Price = 699.99M,
			QuantityInStoct = 5,
			ProductTypeId = productTypes.First(pt => pt.Name == "Electric Guitars").Id,
			BrandId = brands.First(b => b.Name == "Fender").Id,
			PhotoUrl = "../assets/Products/Electric-Guitar-FenderStratocaster.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "IsRightHanded"), Value = "Praworęczna" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "BodyMaterial"), Value = "Olcha" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "NeckMaterial"), Value = "Klon" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "FingerboardMaterial"), Value = "Palisander" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "NumberOfFrets"), Value = "21" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "PickupConfiguration"), Value = "SSS" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Bridge"), Value = "Tremolo" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Color"), Value = "Czarny" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Finish"), Value = "Wysoki połysk" },
			}
		},
		new Product
		{
			Name = "Gibson Les Paul Standard",
			Description = "Iconic electric guitar with rich, warm tones.",
			Price = 2499.99M,
			QuantityInStoct = 3,
			ProductTypeId = productTypes.First(pt => pt.Name == "Electric Guitars").Id,
			BrandId = brands.First(b => b.Name == "Gibson").Id,
			PhotoUrl = "../assets/Products/Electric-Guitar-GibsonLP.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "IsRightHanded"), Value = "Praworęczna" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "BodyMaterial"), Value = "Mahoń" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "NeckMaterial"), Value = "Mahoń" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "FingerboardMaterial"), Value = "Heban" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "NumberOfFrets"), Value = "22" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "PickupConfiguration"), Value = "HH" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Bridge"), Value = "Fixed" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Color"), Value = "Czerwony" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Finish"), Value = "Wysoki połysk" },
			}
		},
		new Product
		{
			Name = "Jackson JS22 Dinky",
			Description = "Affordable electric guitar with fast neck and high-output pickups.",
			Price = 199.99M,
			QuantityInStoct = 7,
			ProductTypeId = productTypes.First(pt => pt.Name == "Electric Guitars").Id,
			BrandId = brands.First(b => b.Name == "Jackson").Id,
			PhotoUrl = "../assets/Products/Electric-Guitar-JacksonPDX2.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "IsRightHanded"), Value = "Praworęczna" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "BodyMaterial"), Value = "Mahoń" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "NeckMaterial"), Value = "Mahoń" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "FingerboardMaterial"), Value = "Heban" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "NumberOfFrets"), Value = "22" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "PickupConfiguration"), Value = "HH" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Bridge"), Value = "Fixed" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Color"), Value = "Czerwony" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Finish"), Value = "Wysoki połysk" },
			}
		},
		new Product
		{
			Name = "Yamaha Standard Plus",
			Description = "Affordable electric guitar with fast neck and high-output pickups.",
			Price = 599.99M,
			QuantityInStoct = 10,
			ProductTypeId = productTypes.First(pt => pt.Name == "Electric Guitars").Id,
			BrandId = brands.First(b => b.Name == "Yamaha").Id,
			PhotoUrl = "../assets/Products/Electric-Guitar-Yamaha-1.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "IsRightHanded"), Value = "Praworęczna" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "BodyMaterial"), Value = "Mahoń" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "NeckMaterial"), Value = "Mahoń" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "FingerboardMaterial"), Value = "Heban" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "NumberOfFrets"), Value = "22" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "PickupConfiguration"), Value = "HH" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Bridge"), Value = "Fixed" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Color"), Value = "Czerwony" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Guitars", "Finish"), Value = "Wysoki połysk" },
			}
		},
		// **Acoustic Bass**
		new Product
		{
			Name = "Yamaha Bass",
			Description = "Versatile and affordable acoustic bass guitar.",
			Price = 249.99M,
			QuantityInStoct = 6,
			ProductTypeId = productTypes.First(pt => pt.Name == "Acoustic Bass").Id,
			BrandId = brands.First(b => b.Name == "Yamaha").Id,
			PhotoUrl = "../assets/Products/Acoustic-Bass-Yamaha1.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Bass", "IsRightHanded"), Value = "Praworęczna" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Bass", "BodyMaterial"), Value = "Świerk" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Bass", "NumberOfStrings"), Value = "4" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Bass", "Fretless"), Value = "false" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Bass", "Electronics"), Value = "true" },
			}
		},
		// **Electric Bass**
		new Product
		{
			Name = "Yamaha Precision Bass",
			Description = "Industry standard for bass guitars.",
			Price = 799.99M,
			QuantityInStoct = 5,
			ProductTypeId = productTypes.First(pt => pt.Name == "Electric Bass").Id,
			BrandId = brands.First(b => b.Name == "Yamaha").Id,
			PhotoUrl = "../assets/Products/Electric-Bass-Yamaha1.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "IsRightHanded"), Value = "Praworęczna" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "BodyMaterial"), Value = "Olcha" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "NeckMaterial"), Value = "Klon" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "FingerboardMaterial"), Value = "Palisander" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "NumberOfStrings"), Value = "4" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "Fretless"), Value = "false" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "PickupType"), Value = "Single Coil" },
			}
		},
		new Product
		{
			Name = "Gibson Bass Super",
			Description = "Versatile electric bass with punchy tone.",
			Price = 249.99M,
			QuantityInStoct = 7,
			ProductTypeId = productTypes.First(pt => pt.Name == "Electric Bass").Id,
			BrandId = brands.First(b => b.Name == "Gibson").Id,
			PhotoUrl = "../assets/Products/Electric-Bass-Gibson1.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "IsRightHanded"), Value = "Praworęczna" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "BodyMaterial"), Value = "Olcha" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "NeckMaterial"), Value = "Klon" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "FingerboardMaterial"), Value = "Palisander" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "NumberOfStrings"), Value = "4" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "Fretless"), Value = "false" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Bass", "PickupType"), Value = "Single Coil" },
			}
		},
		// **Acoustic Drums**
		new Product
		{
			Name = "Pearl Export EXX Drum Set",
			Description = "Complete 5-piece drum set with cymbals.",
			Price = 699.99M,
			QuantityInStoct = 3,
			ProductTypeId = productTypes.First(pt => pt.Name == "Acoustic Drums").Id,
			BrandId = brands.First(b => b.Name == "Pearl").Id,
			PhotoUrl = "../assets/Products/Acoustic-Drums-Pearl1.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Drums", "NumberOfPieces"), Value = "5" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Drums", "ShellMaterial"), Value = "Mahoń" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Drums", "IncludedHardware"), Value = "true" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Drums", "IncludedCymbals"), Value = "true" },
			}
		},
		new Product
		{
			Name = "Tama Imperialstar Drum Set",
			Description = "High-quality acoustic drum set suitable for beginners and pros.",
			Price = 799.99M,
			QuantityInStoct = 2,
			ProductTypeId = productTypes.First(pt => pt.Name == "Acoustic Drums").Id,
			BrandId = brands.First(b => b.Name == "Tama").Id,
			PhotoUrl = "../assets/Products/Acoustic-Drums-TamaStar1.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Drums", "NumberOfPieces"), Value = "6" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Drums", "ShellMaterial"), Value = "Klon" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Drums", "IncludedHardware"), Value = "true" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Acoustic Drums", "IncludedCymbals"), Value = "false" },
			}
		},
		// **Electric Drums**
		new Product
		{
			Name = "Pearl DTX402K Electronic Drum Kit",
			Description = "Compact electronic drum kit for home practice.",
			Price = 499.99M,
			QuantityInStoct = 4,
			ProductTypeId = productTypes.First(pt => pt.Name == "Electric Drums").Id,
			BrandId = brands.First(b => b.Name == "Pearl").Id,
			PhotoUrl = "../assets/Products/Electric-Drums-Pearl1.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Drums", "NumberOfPads"), Value = "8" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Drums", "IncludesKickPad"), Value = "true" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Drums", "ModuleSounds"), Value = "287" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Drums", "USBConnectivity"), Value = "true" },
			}
		},
		new Product
		{
			Name = "Pearl e/Merge Electronic Drum Set",
			Description = "Advanced electronic drum kit with realistic feel.",
			Price = 3499.99M,
			QuantityInStoct = 1,
			ProductTypeId = productTypes.First(pt => pt.Name == "Electric Drums").Id,
			BrandId = brands.First(b => b.Name == "Pearl").Id,
			PhotoUrl = "../assets/Products/Electric-Drums-Pearl2.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Drums", "NumberOfPads"), Value = "10" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Drums", "IncludesKickPad"), Value = "true" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Drums", "ModuleSounds"), Value = "450" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Electric Drums", "USBConnectivity"), Value = "true" },
			}
		},
		// **Grand Pianos**
		new Product
		{
			Name = "Yamaha CFX Concert Grand Piano",
			Description = "Premium concert grand piano with exceptional sound.",
			Price = 179999.99M,
			QuantityInStoct = 1,
			ProductTypeId = productTypes.First(pt => pt.Name == "Grand Pianos").Id,
			BrandId = brands.First(b => b.Name == "Yamaha").Id,
			PhotoUrl = "../assets/Products/Grand-Piano-Yamaha1.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Grand Pianos", "NumberOfKeys"), Value = "88" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Grand Pianos", "Length"), Value = "275" }, // cm
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Grand Pianos", "Finish"), Value = "Wysoki połysk" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Grand Pianos", "Color"), Value = "Czarny" },
			}
		},
		new Product
		{
			Name = "Yamaha CFX Concert Grand Piano",
			Description = "Premium concert grand piano with exceptional sound.",
			Price = 200000.99M,
			QuantityInStoct = 1,
			ProductTypeId = productTypes.First(pt => pt.Name == "Grand Pianos").Id,
			BrandId = brands.First(b => b.Name == "Yamaha").Id,
			PhotoUrl = "../assets/Products/Grand-Piano-Yamaha2.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Grand Pianos", "NumberOfKeys"), Value = "88" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Grand Pianos", "Length"), Value = "275" }, // cm
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Grand Pianos", "Finish"), Value = "Wysoki połysk" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Grand Pianos", "Color"), Value = "Czarny" },
			}
		},
		// **Keyboards**
		new Product
		{
			Name = "Yamaha PSR-E373 Keyboard",
			Description = "Portable keyboard with wide range of features.",
			Price = 199.99M,
			QuantityInStoct = 10,
			ProductTypeId = productTypes.First(pt => pt.Name == "Keyboards").Id,
			BrandId = brands.First(b => b.Name == "Yamaha").Id,
			PhotoUrl = "../assets/Products/Keyboard-Yamaha1.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "NumberOfKeys"), Value = "61" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "TouchSensitive"), Value = "true" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "Polyphony"), Value = "48" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "NumberOfVoices"), Value = "622" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "USBConnectivity"), Value = "true" },
			}
		},
		new Product
		{
			Name = "Yamaha DGX-660 Portable Grand",
			Description = "88-key portable grand piano with authentic piano feel.",
			Price = 799.99M,
			QuantityInStoct = 5,
			ProductTypeId = productTypes.First(pt => pt.Name == "Keyboards").Id,
			BrandId = brands.First(b => b.Name == "Yamaha").Id,
			PhotoUrl = "../assets/Products/Keyboard-Yamaha2.jpg",
			ProductDetails = new List<ProductDetails>
			{
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "NumberOfKeys"), Value = "61" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "TouchSensitive"), Value = "true" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "Polyphony"), Value = "48" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "NumberOfVoices"), Value = "622" },
				new ProductDetails { SpecificationDefinitionId = GetSpecDefId(specificationDefinitions, "Keyboards", "USBConnectivity"), Value = "true" },
			}
		}
	};

			context.Products.AddRange(products);
			await context.SaveChangesAsync();
		}

		private static int GetSpecDefId(List<SpecificationDefinition> specs, string productTypeName, string specName)
		{
			var matchingSpecs = specs.Where(sd => sd.ProductType.Name == productTypeName).ToList();
			Console.WriteLine($"Znaleziono {matchingSpecs.Count} specyfikacji dla typu produktu '{productTypeName}'.");

			var spec = matchingSpecs.FirstOrDefault(sd => sd.Name == specName);

			if (spec == null)
			{
				Console.WriteLine($"Dostępne specyfikacje dla '{productTypeName}':");
				foreach (var s in matchingSpecs)
				{
					Console.WriteLine($"- {s.Name}");
				}
				throw new Exception($"Nie znaleziono definicji specyfikacji: ProductType='{productTypeName}', SpecName='{specName}'. Upewnij się, że dane są poprawnie zseedowane.");
			}

			return spec.Id;
		}

	}


}