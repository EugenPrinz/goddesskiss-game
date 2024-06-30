using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Encryption_Rijndae
{
	internal class Program
	{
		private const string KEY = "IU is Korea Best Singer! really!";

		private const string JSON_KEY = "JSON134c4dabedcd462bad9d775873de";

		public static string XOR(string data, int value)
		{
			string text = string.Empty;
			for (int i = 0; i < data.Length; i++)
			{
				text += (char)(data[i] ^ value);
			}
			return text;
		}

		public static void Object_EncryptToFile(string filePath, object obj)
		{
			Encoding @default = Encoding.Default;
			byte[] bytes = @default.GetBytes("JSON134c4dabedcd462bad9d775873de");
			byte[] bytes2 = @default.GetBytes("JSON134c4dabedcd462bad9d775873de");
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.Zeros;
			rijndaelManaged.BlockSize = 256;
			using (FileStream stream = new FileStream(filePath, FileMode.Create))
			{
				using ICryptoTransform transform = rijndaelManaged.CreateEncryptor(bytes, bytes2);
				using CryptoStream serializationStream = new CryptoStream(stream, transform, CryptoStreamMode.Write);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(serializationStream, obj);
			}
			rijndaelManaged.Clear();
		}

		public static object Object_DecryptFromFile(string filePath)
		{
			Encoding @default = Encoding.Default;
			byte[] bytes = @default.GetBytes("JSON134c4dabedcd462bad9d775873de");
			byte[] bytes2 = @default.GetBytes("JSON134c4dabedcd462bad9d775873de");
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.Zeros;
			rijndaelManaged.BlockSize = 256;
			object result;
			using (FileStream stream = new FileStream(filePath, FileMode.Open))
			{
				using ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, bytes2);
				using CryptoStream serializationStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				result = binaryFormatter.Deserialize(serializationStream);
			}
			rijndaelManaged.Clear();
			return result;
		}

		public static object Object_DecryptFromMemory(byte[] buffer)
		{
			Encoding @default = Encoding.Default;
			byte[] bytes = @default.GetBytes("JSON134c4dabedcd462bad9d775873de");
			byte[] bytes2 = @default.GetBytes("JSON134c4dabedcd462bad9d775873de");
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.Zeros;
			rijndaelManaged.BlockSize = 256;
			object result;
			using (MemoryStream stream = new MemoryStream(buffer))
			{
				using ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, bytes2);
				using CryptoStream serializationStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				result = binaryFormatter.Deserialize(serializationStream);
			}
			rijndaelManaged.Clear();
			return result;
		}

		public static string JSON_Decrypt(string s)
		{
			Encoding @default = Encoding.Default;
			byte[] bytes = @default.GetBytes("JSON134c4dabedcd462bad9d775873de");
			byte[] bytes2 = @default.GetBytes("JSON134c4dabedcd462bad9d775873de");
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.Zeros;
			rijndaelManaged.BlockSize = 256;
			string result;
			using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(s)))
			{
				using ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, bytes2);
				using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
				using StreamReader streamReader = new StreamReader(stream2);
				result = streamReader.ReadToEnd();
			}
			rijndaelManaged.Clear();
			return result;
		}

		public static string Encrypt_Session(string s)
		{
			Encoding @default = Encoding.Default;
			byte[] bytes = @default.GetBytes("Zb*!" + M00_Init.KEY + M01_Title.KEY + XOR(M02_WorldMap.KEY, 14) + M03_Battle.KEY + M04_Tutorial.KEY + XOR(RoLocalUser.KEY, 30) + RemoteObjectManager.KEY);
			byte[] rgbIV = bytes;
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.Zeros;
			rijndaelManaged.BlockSize = 256;
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ICryptoTransform transform = rijndaelManaged.CreateEncryptor(bytes, rgbIV))
				{
					using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
					using StreamWriter streamWriter = new StreamWriter(stream);
					streamWriter.Write(s);
				}
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			rijndaelManaged.Clear();
			return result;
		}

		public static string Encrypt(string s, bool first = false)
		{
			Encoding @default = Encoding.Default;
			byte[] array = null;
			byte[] array2 = null;
			if (first)
			{
				array = @default.GetBytes("IU is Korea Best Singer! really!");
				array2 = @default.GetBytes("IU is Korea Best Singer! really!");
			}
			else
			{
				array = @default.GetBytes("Zb*!" + M00_Init.KEY + M01_Title.KEY + XOR(M02_WorldMap.KEY, 14) + M03_Battle.KEY + M04_Tutorial.KEY + XOR(RoLocalUser.KEY, 30) + RemoteObjectManager.KEY);
				array2 = array;
			}
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.Zeros;
			rijndaelManaged.BlockSize = 256;
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ICryptoTransform transform = rijndaelManaged.CreateEncryptor(array, array2))
				{
					using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
					using StreamWriter streamWriter = new StreamWriter(stream);
					streamWriter.Write(s);
				}
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			rijndaelManaged.Clear();
			return result;
		}

		public static string Decrypt(string s, bool first = false)
		{
			Encoding @default = Encoding.Default;
			byte[] array = null;
			byte[] array2 = null;
			if (first)
			{
				array = @default.GetBytes("IU is Korea Best Singer! really!");
				array2 = @default.GetBytes("IU is Korea Best Singer! really!");
			}
			else
			{
				array = @default.GetBytes("Zb*!" + M00_Init.KEY + M01_Title.KEY + XOR(M02_WorldMap.KEY, 14) + M03_Battle.KEY + M04_Tutorial.KEY + XOR(RoLocalUser.KEY, 30) + RemoteObjectManager.KEY);
				array2 = array;
			}
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.Zeros;
			rijndaelManaged.BlockSize = 256;
			string result;
			using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(s)))
			{
				using ICryptoTransform transform = rijndaelManaged.CreateDecryptor(array, array2);
				using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
				using StreamReader streamReader = new StreamReader(stream2);
				result = streamReader.ReadToEnd();
			}
			rijndaelManaged.Clear();
			return result;
		}
	}
}
