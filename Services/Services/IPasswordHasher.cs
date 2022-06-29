namespace Services
{
    public interface IPasswordHasher
    {
         public string Hash(string password, int iterations);

         public bool Verify(string password, string hashedPassword);
    }
}
