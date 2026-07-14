
User user = new User(
    1,
    "",
    "ali@test.com"
);
Console.WriteLine($"{user.Name}  {user.Email}");
UserService u2 = new UserService();
u2.AddUser(user);
Console.WriteLine(u2.GetUser(2)); //This line will cause problem tounderstand for new users, u2.GetUser(user.Id)=user object tahthas an attribute Name

// ploymorphism code
IUserService service = new UserService();// upcasting (implicit casting for the real polymorphism)


//handle nulls


public class User
{
  public int Id { get; set; }
  public string Name { get; set; }

  public string Email { get; set; }
  public User(int id, string name, string email)
  {
    Id=id;
    Name=name;
    Email=email;

  }
}
public interface IUserService
{
  User GetUser(int id);

  void AddUser(User user);
}

public class UserService: IUserService
{
 private List<User> _user = new List<User>();

 public void AddUser(User user)
  {
    if(string.IsNullOrWhiteSpace(user.Name))
  {
      throw new ArgumentException(
          "Name is required"
      );
  }
    _user.Add(user);
  }
 public User GetUser(int id)
  {
    var user = _user.FirstOrDefault(x => x.Id == id);
    if (user == null)
    {
      Console.WriteLine("no user exist!");
    }
    
    return user;
    
  }
}
// User user = new User();
// user.Email="hammadtechleadz@gmail.com";
// user.Name="Hammad";
// user.Id=1;


































// // Array
// string[] ar = {"BMW" ,"OOdi" ,"GWM", "toyata"};
// foreach (string i in ar)
// {
//   Console.WriteLine(i);
// }


// // for i in ar python equivalent














// This means we are using calles fromt he system name space
// using System;


// namespace HelloWorld
// {
//   class Program
//   {
//     static void Main(string[] args)
//     {
//       Console.WriteLine("Hello World!"); //This is args comment
//       Console.Write("Hello World! ");
//       Console.Write("I will print on the same line.");   
//     }
//   }
// }
// /* this is multi
// lie comment*/


















// Console.WriteLine("Hello, World!");

// string? age = null;
// Console.WriteLine(age);

// // Collecions
// List<string> users = new List<string>();
// // Modern list
// List<string> users1 = new();
// Dictionary<string, int> d = new Dictionary<string, int>(); 
// HashSet<int> h = new HashSet<int>();
// int[] a = new int[5];

// Methods
//  void Greet(string name)
//  {
//     Console.WriteLine($"hello {name}");
// }
// Greet("Ali");
// int Add (int a, int b)
// {
//     return a+b;
// }
// Console.WriteLine(Add(1,2));
// int result = MathHelper.Square(5);
// Console.WriteLine(result);
// class MathHelper
// {
//     public static int Square(int x)
//     {
//         return x * x;
//     }
// }

// Property






