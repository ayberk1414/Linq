using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates02
{
    class Program
    {
        public static void Main()
        {

            List<Category> categories = new List<Category>() {
                new Category { Name = "Elektronik", Id = 1 },
                new Category { Name = "Beyaz Eşya", Id = 2 },
            };

            List<Product> products = new List<Product>()
            {
                new Product { Id = 1, Name = "Samsung S21 5g", Price = 15000, CategoryId = 1 },
                new Product { Id = 2, Name = "Iphone 14 pro max", Price = 80000, CategoryId = 1 },
                new Product { Id = 3, Name = "Iphone 14", Price = 65000, CategoryId = 1 },
                new Product { Id = 4, Name = "Buzdolabı", Price = 10000, CategoryId = 2 },
                new Product { Id = 5, Name = "Çamaşır Makinesi", Price = 6000, CategoryId = 2 }
            };
            Random random = new Random();
            List<Sales> salesData = new List<Sales>();

            foreach (var product in products)
            {
                int randomSalesAmount = random.Next(1, 100); // 1 ile 100 arasında rastgele bir miktar oluşturur
                salesData.Add(new Sales { ProductId = product.Id, Amount = randomSalesAmount });
            }

            // Kategorisi Beyaz Eşya olan ürünlerin Adları ile beraber, satış toplamları:
            var whiteGoodsSales = from s in salesData
                                  join p in products on s.ProductId equals p.Id
                                  where categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name == "Beyaz Eşya"
                                  group s by p.Name into g
                                  select new { ProductName = g.Key, TotalSales = g.Sum(s => s.Amount) };

            foreach (var item in whiteGoodsSales)
            {
                Console.WriteLine($"{item.ProductName} - {item.TotalSales}");
            }

            // En çok satışı olan 3 ürünün adı ve kategorisini:
            var top3SellingProducts = salesData
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSales = g.Sum(s => s.Amount),
                })
                .OrderByDescending(p => p.TotalSales)
                .Take(3)
                .Join(products, p => p.ProductId, pr => pr.Id, (p, pr) => new
                {
                    ProductName = pr.Name,
                    CategoryName = categories.FirstOrDefault(c => c.Id == pr.CategoryId)?.Name,
                });

            foreach (var item in top3SellingProducts)
            {
                Console.WriteLine($"Product: {item.ProductName} - Category: {item.CategoryName}");
            }

            // En az satılan 2 ürünün adı ve kategorisi:
            var bottom2SellingProducts = salesData
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSales = g.Sum(s => s.Amount),
                })
                .OrderBy(p => p.TotalSales)
                .Take(2)
                .Join(products, p => p.ProductId, pr => pr.Id, (p, pr) => new
                {
                    ProductName = pr.Name,
                    CategoryName = categories.FirstOrDefault(c => c.Id == pr.CategoryId)?.Name,
                });

            foreach (var item in bottom2SellingProducts)
            {
                Console.WriteLine($"Product: {item.ProductName} - Category: {item.CategoryName}");
            }

            // Kategorilere göre ortalama satış fiyatı:
            var averageSalesPriceByCategory = salesData
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new
                {
                    CategoryId = p.CategoryId,
                    Amount = s.Amount,
                })
                .GroupBy(s => s.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    AveragePrice = g.Average(s => s.Amount),
                })
                .Join(categories, c => c.CategoryId, cat => cat.Id, (c, cat) => new
                {
                    CategoryName = cat.Name,
                    AveragePrice = c.AveragePrice,
                });

            foreach (var item in averageSalesPriceByCategory)
            {
                Console.WriteLine($"{item.CategoryName} -> {item.AveragePrice}");
            }
        }

        public class Sales
        {
            public int Amount { get; set; }
            public int ProductId { get; set; }
        }

        public class ProductCategoryListObject
        {
            public string ProductName { get; set; }
            public string CategoryName { get; set; }
        }

        class Product
        {
            public int Id { get; set; }
            public int Price { get; set; }
            public string Name { get; set; }
            public int CategoryId { get; set; }
        }

        class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
