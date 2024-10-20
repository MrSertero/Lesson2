using Bogus;
using Lesson2.Data;
using Lesson2.Data.Entities;
using Lesson2.Interfaces;
using Lesson2.Mapper;
using Lesson2.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDBContext>(opt =>
	opt.UseNpgsql(builder.Configuration.GetConnectionString("MyConnectionDB")));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IImageWorker, ImageWorker>();
builder.Services.AddAutoMapper(typeof(AppMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

//���������� items � DB ���� ���� �����
using (var serviceScore = app.Services.CreateScope())
{
	var context = serviceScore.ServiceProvider.GetService<AppDBContext>();
	var imageWorker = serviceScore.ServiceProvider.GetService<IImageWorker>();

	context.Database.Migrate(); //������������ ������ ������� �� DB, ���� �� ��� ����

	if (!context.Categories.Any())
	{
		var image = imageWorker.Save("https://rivnepost.rv.ua/img/650/korisnoi-kovbasi-ne-buvae-hastroenterolohi-nazvali_20240612_4163.jpg");
        var kovbasa = new CategoryEntity
		{
			Name = "�������",
			Image = image,
			Description = "��� ����� ����������� �� ������� ������� �� ����������. " +
			"������� ��������, �� �� ��������, ���� ����� ������� �� ����� 50 ����� �� ����."
		};
        
		image = imageWorker.Save("https://www.vsesmak.com.ua/sites/default/files/styles/large/public/field/image/syrnaya_gora_5330_1900_21.jpg?itok=RPUrRskl");
		var cheese = new CategoryEntity
		{
			Name = "����",
			Image = image,
			Description = "C�� � ���� � ���������� ������ �� ������ ����. " +
			"���� �� � ������, � �������, � ��������. �� ����� �������, �� �����, " +
			"�� ��������� �� ��������� ������������ ������� ��� � ��������."
		};

        image = imageWorker.Save("https://nugget-markets-01.s3.us-west-1.amazonaws.com/nugget/p/bakery-breads-group-8mi639ru7yet2udb.jpg");
        var bread = new CategoryEntity
		{
			Name = "���",
			Image = image,
			Description = "� ������� ����� ���������� ����������� ������� ����� ����, " +
			"�� ����� �� �������� ������ ����� ���� � ���������, �������������� ���."
		};

        context.Categories.Add(kovbasa);
		context.Categories.Add(cheese);
		context.Categories.Add(bread);
		context.SaveChanges();
	}

    if (!context.Products.Any())
    {
        var categories = context.Categories.ToList();

        var fakerProduct = new Faker<ProductEntity>("uk")
                    .RuleFor(u => u.Name, (f, u) => f.Commerce.Product())
                    .RuleFor(u => u.Price, (f, u) => decimal.Parse(f.Commerce.Price()))
                    .RuleFor(u => u.Category, (f, u) => f.PickRandom(categories));

        string url = "https://picsum.photos/1200/800?product";

        var products = fakerProduct.GenerateLazy(30);

        Random r = new Random();

        foreach (var product in products)
        {
            context.Add(product);
            context.SaveChanges();
            int imageCount = r.Next(3, 5);
            for (int i = 0; i < imageCount; i++)
            {
                var imageName = imageWorker.Save(url);
                var imageProduct = new ProductImageEntity
                {
                    Product = product,
                    Image = imageName,
                    Priority = i
                };
                context.Add(imageProduct);
                context.SaveChanges();
            }
        }
    }
}
	app.Run();
