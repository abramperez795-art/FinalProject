using NLog;
using System.ComponentModel.DataAnnotations;
string path = Directory.GetCurrentDirectory() + "//nlog.config";



var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();


logger.Info("Program started");



do
{
    Console.WriteLine("Enter your selection:");
    Console.WriteLine("1) Display all blogs");
    Console.WriteLine("2) Add Blog");
    Console.WriteLine("5) Delete Blog");
    Console.WriteLine("6) Edit Blog");
    Console.WriteLine("7) Display all products");
    Console.WriteLine("8) Display single product");
    Console.WriteLine("9) Add product");
    Console.WriteLine("10) Edit product");
    Console.WriteLine("11) Delete product");
    Console.WriteLine("12) Display categories");
    Console.WriteLine("13) Display categories with active products");
    Console.WriteLine("14) Add category");
    Console.WriteLine("15) Edit category");
    Console.WriteLine("16) Delete category");
    Console.WriteLine("Enter to quit");


    string? choice = Console.ReadLine();
    Console.Clear();


    logger.Info("Option {choice} selected", choice);


    if (choice == "1")
    {
        var db = new DataContext();
        var query = db.Blogs.OrderBy(b => b.Name);
        Console.WriteLine($"{query.Count()} Blogs returned");
        foreach (var item in query)
        {
            Console.WriteLine(item.Name);
        }
    }
    else if (choice == "2")
    {
        var db = new DataContext();
        Blog? blog = InputBlog(db, logger);
        if (blog != null)
        {
            db.AddBlog(blog);
            logger.Info("Blog added - {name}", blog.Name);
        }
    }
    else if (choice == "5")
    {
        Console.WriteLine("Choose the blog to delete:");
        var db = new DataContext();
        var blog = GetBlog(db);
        if (blog != null)
        {
            db.DeleteBlog(blog);
            logger.Info($"Blog (id: {blog.BlogId}) deleted");
        }
        else
        {
            logger.Error("Blog is null");
        }
    }
    else if (choice == "6")
    {
        Console.WriteLine("Choose the blog to edit:");
        var db = new DataContext();
        var blog = GetBlog(db);
        if (blog != null)
        {
            Blog? UpdatedBlog = InputBlog(db, logger);
            if (UpdatedBlog != null)
            {
                UpdatedBlog.BlogId = blog.BlogId;
                db.EditBlog(UpdatedBlog);
                logger.Info($"Blog (id: {blog.BlogId}) updated");
            }
        }
    }


    else if (choice == "7")
    {
        var db = new DataContext();
        Console.WriteLine("1) All  2) Active only  3) Discontinued only");
        string? filter = Console.ReadLine();
        var products = filter switch
        {
            "2" => db.Products.Where(p => !p.IsDiscontinued).OrderBy(p => p.ProductName),
            "3" => db.Products.Where(p => p.IsDiscontinued).OrderBy(p => p.ProductName),
            _ => db.Products.OrderBy(p => p.ProductName)
        };
        Console.WriteLine($"{products.Count()} Products returned");
        foreach (var p in products)
            Console.WriteLine($"{p.ProductId}: {p.ProductName} {(p.IsDiscontinued ? "[DISCONTINUED]" : "[ACTIVE]")}");
    }



    else if (choice == "8")
    {
        var db = new DataContext();
        DisplaySingleProduct(db);
    }
    else if (choice == "9")
    {
        var db = new DataContext();
        AddProduct(db, logger);
    }
    else if (choice == "10")
    {
        var db = new DataContext();
        EditProduct(db, logger);
    }
    else if (choice == "11")
    {
        Console.WriteLine("Choose the product to delete:");
        var db = new DataContext();
        DeleteProduct(db, logger);
    }
    else if (choice == "12")
    {
        var db = new DataContext();
        DisplayCategories(db);
    }
    else if (choice == "13")
    {
        var db = new DataContext();
        DisplayCategoriesWithActiveProducts(db);
    }
    else if (choice == "14")
    {
        var db = new DataContext();
        AddCategory(db, logger);
    }
    else if (choice == "15")
    {
        var db = new DataContext();
        EditCategory(db, logger);
    }
    else if (choice == "16")
    {
        Console.WriteLine("Choose category to display:");
        var db = new DataContext();
        DisplaySingleCategoryWithProducts(db);
    }
    else if (string.IsNullOrEmpty(choice))
    {
        logger.Info("Program ended");
        break;
    }
    else
    {
        Console.WriteLine("Invalid choice");
    }


    Console.WriteLine();


} while (true);


logger.Info("Program ended");


static Blog? GetBlog(DataContext db)
{
    // display all blogs
    var blogs = db.Blogs.OrderBy(b => b.BlogId);
    foreach (Blog b in blogs)
    {
        Console.WriteLine($"{b.BlogId}: {b.Name}");
    }
    if (int.TryParse(Console.ReadLine(), out int BlogId))
    {
        Blog blog = db.Blogs.FirstOrDefault(b => b.BlogId == BlogId)!;
        return blog;
    }
    return null;
}
static Blog? InputBlog(DataContext db, NLog.Logger logger)
{
    Blog blog = new();
    Console.WriteLine("Enter the Blog name");
    blog.Name = Console.ReadLine();


    ValidationContext context = new(blog, null, null);
    List<ValidationResult> results = new List<ValidationResult>();


    var isValid = Validator.TryValidateObject(blog, context, results, true);
    if (isValid)
    {
        // check for unique name
        if (db.Blogs.Any(b => b.Name == blog.Name))
        {
            // generate validation error
            isValid = false;
            results.Add(new ValidationResult("Blog name exists", new[] { "Name" }));
        }
        else
        {
            logger.Info("Validation passed");
        }
    }
    if (!isValid)
    {
        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
        return null;
    }
    return blog;
}



static Product? GetProduct(DataContext db)
{
    var products = db.Products.OrderBy(p => p.ProductId);
    foreach (Product p in products)
        Console.WriteLine($"{p.ProductId}: {p.ProductName}");


    if (int.TryParse(Console.ReadLine(), out int ProductId))
    {
        return db.Products.FirstOrDefault(p => p.ProductId == ProductId);
    }
    return null;
}


static void DisplaySingleProduct(DataContext db)
{
    Console.WriteLine("Enter ProductId:");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        Product? product = db.Products.Find(id);
        if (product != null)
        {
            Console.WriteLine($"ProductId:    {product.ProductId}");
            Console.WriteLine($"ProductName:  {product.ProductName}");
            Console.WriteLine($"Discontinued: {(product.IsDiscontinued ? "Yes" : "No")}");
        }
        else
        {
            Console.WriteLine("Product not found.");
        }
    }
    else
    {
        Console.WriteLine("Invalid ProductId.");
    }


    Console.WriteLine("Press Enter to return to menu...");
    Console.ReadLine();
}


static void AddProduct(DataContext db, NLog.Logger logger)
{
    Product product = new();
    Console.WriteLine("Enter the Product name:");
    product.ProductName = Console.ReadLine();


    product.IsDiscontinued = false;


    db.Products.Add(product);
    db.SaveChanges();


    logger.Info("Product added - {product.ProductName}", product.ProductName);
    Console.WriteLine("Product added.");
}


static void EditProduct(DataContext db, NLog.Logger logger)
{
    Console.WriteLine("Choose product to edit:");
    var product = GetProduct(db);  // Add this helper method
    if (product != null)
    {
        Console.WriteLine($"Current: {product.ProductName}");
        Console.WriteLine("New name:");
        product.ProductName = Console.ReadLine();
        Console.WriteLine("Discontinued? (y/n):");
        product.IsDiscontinued = Console.ReadLine()?.ToLower() == "y";
        db.SaveChanges();
        logger.Info($"Product (id: {product.ProductId}) updated");
        Console.WriteLine("Product updated.");
    }
}
static void DeleteProduct(DataContext db, NLog.Logger logger)
{
    // show all products
    var products = db.Products.OrderBy(p => p.ProductId);
    foreach (Product p in products)
    {
        Console.WriteLine($"{p.ProductId}: {p.ProductName}");
    }


    Console.WriteLine("Enter the ProductId to delete:");
    if (int.TryParse(Console.ReadLine(), out int productId))
    {
        Product? product = db.Products.FirstOrDefault(p => p.ProductId == productId);
        if (product != null)
        {
            db.Products.Remove(product);
            db.SaveChanges();
            logger.Info($"Product (id: {product.ProductId}) deleted");
            Console.WriteLine("Product deleted.");
        }
        else
        {
            Console.WriteLine("Product not found.");
            logger.Error("Product not found for deletion");
        }
    }
    else
    {
        Console.WriteLine("Invalid ProductId.");
    }
}





static void DisplayCategories(DataContext db)
{
    var query = db.Categories.OrderBy(c => c.CategoryName);
    Console.WriteLine($"{query.Count()} Categories returned");
    foreach (var item in query)
        Console.WriteLine(item.CategoryName);
}



static void DisplayCategoriesWithActiveProducts(DataContext db)
{
    var categories = db.Categories
        .OrderBy(c => c.CategoryName)
        .ToList();


    Console.WriteLine($"{categories.Count} Categories returned");


   
    foreach (var c in categories)
    {
        Console.WriteLine($"{c.CategoryName} - active products");
    }
}





static Category? GetCategory(DataContext db)
{
    var categories = db.Categories.OrderBy(c => c.CategoryId);
    foreach (Category c in categories)
        Console.WriteLine($"{c.CategoryId}: {c.CategoryName}");
    if (int.TryParse(Console.ReadLine(), out int CategoryId))
    {
        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == CategoryId)!;
        return category;
    }
    return null;
}


static Category? InputCategory(DataContext db, NLog.Logger logger)
{
    Category category = new();
    Console.WriteLine("Enter the Category name:");
    category.CategoryName = Console.ReadLine();


    ValidationContext context = new(category, null, null);
    List<ValidationResult> results = new();
    var isValid = Validator.TryValidateObject(category, context, results, true);


    if (isValid)
    {
        if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
        {
            isValid = false;
            results.Add(new ValidationResult("Category name exists", new[] { "CategoryName" }));
        }
        else logger.Info("Validation passed");
    }


    if (!isValid)
    {
        foreach (var result in results)
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        return null;
    }
    return category;
}


static void AddCategory(DataContext db, NLog.Logger logger)
{
    Category? category = InputCategory(db, logger);
    if (category != null)
    {
        db.AddCategory(category);
        logger.Info("Category added - {name}", category.CategoryName);
    }
}


static void EditCategory(DataContext db, NLog.Logger logger)
{
    Console.WriteLine("Choose the category to edit:");
    var category = GetCategory(db);
    if (category != null)
    {
        Category? updatedCategory = InputCategory(db, logger);
        if (updatedCategory != null)
        {
            updatedCategory.CategoryId = category.CategoryId;
            db.EditCategory(updatedCategory);
            logger.Info($"Category (id: {category.CategoryId}) updated");
        }
    }
}



static void DisplaySingleCategoryWithProducts(DataContext db)
{
    Console.WriteLine("Enter CategoryId:");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        Category? category = db.Categories.Find(id);
        if (category != null)
        {
            Console.WriteLine($"{category.CategoryId}: {category.CategoryName}");


            // list all active products
            var activeProducts = db.Products
                .Where(p => !p.IsDiscontinued)
                .OrderBy(p => p.ProductName);


            Console.WriteLine($"\n{activeProducts.Count()} active products:");
            foreach (var p in activeProducts)
                Console.WriteLine($"  - {p.ProductName}");
        }
        else
        {
            Console.WriteLine("Category not found.");
        }
    }
    else
    {
        Console.WriteLine("Invalid CategoryId.");
    }


    Console.WriteLine("Press Enter to return to menu...");
    Console.ReadLine();
}