namespace SilentMoonApp.Application.Abstractions.Hashing;

public interface IHasher
{
	string Hash(string rawCode);

	bool Verify(string rawCode,
				string hashedCode);
}
