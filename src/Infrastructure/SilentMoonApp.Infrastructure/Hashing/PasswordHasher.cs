using SilentMoonApp.Application.Abstractions.Hashing;
using System.Security.Cryptography;

namespace SilentMoonApp.Infrastructure.Hashing;

public class PasswordHasher : IPasswordHasher
{
	private const int saltSize = 16;           // 128 bits
	private const int hashSize = 32;           // 256 bits
	private const int iterations = 100_000;


	public string Hash(string password)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));


		byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

		byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password,
											    salt,
											    iterations,
											    HashAlgorithmName.SHA256,
											    hashSize);

		string saltString = Convert.ToBase64String(salt);

		string hashString = Convert.ToBase64String(hash);

		return $"{iterations}:{saltString}:{hashString}";
	}


	public bool Verify(string password, string hashedPassword)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
		ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword, nameof(hashedPassword));


		string[] parts = hashedPassword.Split(':');


		if (parts.Length != 3)
			return false;
		//throw new FormatException("Invalid hashed password format.");

		if (!int.TryParse(parts[0], out int iterations))
			return false;
		//throw new FormatException("Invalid hashed password format.");


		try
		{
			byte[] salt = Convert.FromBase64String(parts[1]);
			byte[] hash = Convert.FromBase64String(parts[2]);

			byte[] hashToVerify = Rfc2898DeriveBytes.Pbkdf2(password,
															salt,
															iterations,
															HashAlgorithmName.SHA256,
															hash.Length);

			return CryptographicOperations.FixedTimeEquals(hashToVerify, hash);
		}

		catch (FormatException)
		{
			return false;
			//throw new FormatException("Invalid hashed password format.");
		}

	}
}
