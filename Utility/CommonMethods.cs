using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tangehrine.WebLayer.Utility
{
    public static class CommonMethods
    {
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public static string EncryptData(string textData, string Encryptionkey)
        {
            RijndaelManaged objrij = new RijndaelManaged();
            //set the mode for operation of the algorithm   
            objrij.Mode = CipherMode.CBC;
            //set the padding mode used in the algorithm.   
            objrij.Padding = PaddingMode.PKCS7;
            //set the size, in bits, for the secret key.   
            objrij.KeySize = 0x80;
            //set the block size in bits for the cryptographic operation.    
            objrij.BlockSize = 0x80;
            //set the symmetric key that is used for encryption & decryption.    
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            //set the initialization vector (IV) for the symmetric algorithm    
            byte[] EncryptionkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);

            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;

            //Creates a symmetric AES object with the current key and initialization vector IV.    
            ICryptoTransform objtransform = objrij.CreateEncryptor();
            byte[] textDataByte = Encoding.UTF8.GetBytes(textData);
            //Final transform the test string.  
            return Convert.ToBase64String(objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));
        }

        public static string DecryptData(string EncryptedText, string Encryptionkey)
        {
            RijndaelManaged objrij = new RijndaelManaged();
            objrij.Mode = CipherMode.CBC;
            objrij.Padding = PaddingMode.PKCS7;

            objrij.KeySize = 0x80;
            objrij.BlockSize = 0x80;
            byte[] encryptedTextByte = Convert.FromBase64String(EncryptedText);
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            byte[] EncryptionkeyBytes = new byte[0x10];
            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);
            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;
            byte[] TextByte = objrij.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);
            return Encoding.UTF8.GetString(TextByte);  //it will return readable string  
        }

        public static string DecodeFrom64(string encodedData)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(encodedData);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message);

            }
        }

        private static string CheckServerPath(string currentPath, string folderPath, string fileName)
        {
            var fullPath = Path.Combine(currentPath, folderPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            return Path.Combine(fullPath, fileName);
        }

        private static bool CheckFileExtension(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".jpeg");
        }

        public static async Task<string> WriteFile(string rootpath, string folderName, string moduleName, IFormFile file)
        {
            string fileName = "";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                fileName = moduleName + "_" + DateTime.Now.Ticks + extension;

                var pathBuilt = "";
                if (extension == ".mp4")
                {
                    pathBuilt = Path.Combine(rootpath + "/", folderName);
                }
                else if (extension == ".pdf")
                {
                    pathBuilt = Path.Combine(rootpath + "/", folderName);

                }
                else
                {
                    //pathBuilt = Path.Combine(ImageConstant.returnImages+"/", folderName);
                    pathBuilt = Path.Combine(rootpath + "/", folderName);
                }

                if (!Directory.Exists(pathBuilt))
                    Directory.CreateDirectory(pathBuilt);
                var path = "";
                if (extension == ".mp4")
                {
                    //path = Path.Combine(ImageConstant.returnVideos, folderName, fileName);
                    path = Path.Combine(rootpath + "/", folderName + "/", fileName);
                }
                else if (extension == ".pdf")
                {
                    path = Path.Combine(rootpath + "/", folderName + "/", fileName);

                }
                else
                {
                    // path = Path.Combine(ImageConstant.returnOthers+"/",folderName+"/",fileName);
                    path = Path.Combine(rootpath + "/", folderName + "/", fileName);
                }


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

            }
            catch (Exception)
            {
                //log error
            }

            return fileName;
        }

        public static bool DeleteFile(string rootpath, string folderName, string fileName)
        {
            bool isFileDeleted = false;
            try
            {
                var extension = Path.GetExtension(fileName);


                var path = "";
                if (extension == ".mp4")
                {
                    path = Path.Combine(rootpath + "/", folderName + "/", fileName);
                }
                else
                {
                    path = Path.Combine(rootpath + "/", folderName + "/", fileName);
                }


                if (File.Exists(path))
                {
                    File.Delete(path);
                    isFileDeleted = true;
                }

            }
            catch (Exception)
            {
                //log error
            }

            return isFileDeleted;
        }

        public static string CovertDateFormate(string date)
        {
            string[] arrayOfDate = date.Split("-");
            string newDate = arrayOfDate[1] + "-" + arrayOfDate[0] + "-" + arrayOfDate[2];
            string dateString = Convert.ToDateTime(newDate).ToString();
            return dateString;            
        }

    }
}
