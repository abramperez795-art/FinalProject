using NLog;
using System.ComponentModel.DataAnnotations;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");


do
{Console.WriteLine("Enter your selection:");
  Console.WriteLine("1) Display all blogs");
  Console.WriteLine("2) Add Blog");
  Console.WriteLine("5) Delete Blog");
    Console.WriteLine("6) Edit Blog");
  Console.WriteLine("Enter to quit");

  string? choice = Console.ReadLine();
  Console.Clear();

  logger.Info("Option {choice} selected", choice);

  if (choice == "1")
  {
    // display blogs
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
    // Add blog
   var db = new DataContext();
    Blog? blog = InputBlog(db, logger);
    if (blog != null)
    {
     //blog.BlogId = BlogId;
      db.AddBlog(blog);
      logger.Info("Blog added - {name}", blog.Name);
    }
  }

  else if (choice == "5")
  {
    // delete blog
    Console.WriteLine("Choose the blog to delete:");
     var db = new DataContext();
    var blog = GetBlog(db);
    if (blog != null)
    {

      // delete blog
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
    // edit blog
    Console.WriteLine("Choose the blog to edit:");
    var db = new DataContext();
    var blog = GetBlog(db);
    if (blog != null)
    {
        // input blog
      Blog? UpdatedBlog = InputBlog(db, logger);
      if (UpdatedBlog != null)
      {
        UpdatedBlog.BlogId = blog.BlogId;
        db.EditBlog(UpdatedBlog);
        logger.Info($"Blog (id: {blog.BlogId}) updated");
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


  }
  else if (String.IsNullOrEmpty(choice))
  {
    break;
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
  List<ValidationResult> results = [];

  var isValid = Validator.TryValidateObject(blog, context, results, true);
  if (isValid)
  {
    // check for unique name
    if (db.Blogs.Any(b => b.Name == blog.Name))
    {
      // generate validation error
      isValid = false;
      results.Add(new ValidationResult("Blog name exists", ["Name"]));
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
}{
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