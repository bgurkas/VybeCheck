namespace VybeCheck.Services;

public interface IPasswordService
{
    public string Hash(string plainText);
    public bool Verify(string plainText, string hash);
}