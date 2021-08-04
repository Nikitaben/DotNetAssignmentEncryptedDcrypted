using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace DotNetAssignmentEncryptedDcrypted
{
    public class Customer
    {
        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("creditcard")]
        public string creditcard { get; set; }

        [XmlElement("password")]
        public string password { get; set; }
    }
    [XmlRoot("customers")]
    public class Customers
    {
        [XmlElement("customer")]
        public List<Customer> customers { get; set; }
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            string xmlFile = "UnsecuredData.xml";
            Customers obj = FromXmlFile<Customers>(xmlFile);

            foreach (var c in obj.customers)
            {
                Console.WriteLine(c.creditcard + " " + c.name);
            }
            List<Customer> customerList = new List<Customer>
            {
                new Customer{ name = "Bob Smith", creditcard = "1234-5678-9012-3456", password = "Pa$$w0rd"}
            };

            ToXmlFile("customers.xml", obj);

            //string sk = ProtectedClass.GenerateSecretKey();
            string sk = @"5[[CaSOPZUvLX@Qmb_6>c:;tDegoaP^w";
            Console.WriteLine($"sk = {sk}");

            string eStr = ProtectedClass.EncryptString(sk, "1234-5678-9012-3456");
            Console.WriteLine(eStr);

            string dStr = ProtectedClass.DecryptString(sk, eStr);
            Console.WriteLine(dStr);


            Console.WriteLine("===================");

            string password = "Pa$$word";
            string hashed = ProtectedClass.toMD5(password);
            string hashed2 = ProtectedClass.SaltAndHash(hashed);
            string hashed3 = string.Empty;


            using (SHA256 sha256Hash = SHA256.Create())
            {
                hashed3 = ProtectedClass.GetHash(sha256Hash, password);
                Console.WriteLine($"The SHA256 hash of {password} is: {hashed3}.");
            }


            string userPassword = "Pa$$word";
            string hashedUserPAssword = ProtectedClass.toMD5(userPassword);
            string hashedUserPassword2 = ProtectedClass.SaltAndHash(userPassword);

            Console.WriteLine(hashed);
            Console.WriteLine(hashed2);
            Console.WriteLine(hashed3);
            if (hashed2.Equals(hashedUserPassword2))
            {
                Console.WriteLine("signed in!!!");
            }
            else
            {
                Console.WriteLine("Incorrect password!!!");
            }

            

        }
        public static T FromXmlFile<T>(string file)
        {
            XmlSerializer xmls = new XmlSerializer(typeof(T));
            var xmlContent = File.ReadAllText(file);
            using (StringReader sr = new StringReader(xmlContent))
            {
                return (T)xmls.Deserialize(sr);
            }
        }
        public static void ToXmlFile<T>(string file, T obj)
        {
            using (StringWriter sw =
                new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(T));
                xmls.Serialize(sw, obj);
                File.WriteAllText(file, sw.ToString());
            }
        }
    }
}
